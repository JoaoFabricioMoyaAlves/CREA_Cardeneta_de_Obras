# Backend — Caderneta de Obras Digital

API .NET (Clean Architecture) que substitui os dados mockados do frontend por um banco PostgreSQL real, com autenticação JWT, motor de assinatura digital (hash SHA-256 + IP + user-agent + timestamp do servidor) e armazenamento de fotos/PDFs no MinIO.

> Ver `ROADMAP.md` na raiz do projeto para o contexto completo (decisões de arquitetura, RBAC, seção 3.6).

## Arquitetura

```
backend/
  CadernetaObras.sln
  src/
    CadernetaObras.Domain/          # Entidades, enums, interfaces de repositório — sem dependências externas
    CadernetaObras.Application/     # Use cases, DTOs, interfaces de serviço (IPasswordHasher, IStorageService...)
    CadernetaObras.Infrastructure/  # EF Core (Postgres), Argon2id, JWT, MinIO, QuestPDF — implementa as interfaces da Application
    CadernetaObras.Api/             # Controllers, Program.cs, middleware de erro, appsettings
  tests/
    CadernetaObras.Tests/
  scripts/
    immutability-triggers.sql       # Triggers Postgres que bloqueiam UPDATE/DELETE em entidades já assinadas
  docker-compose.yml                # Postgres + MinIO para desenvolvimento local
```

Fluxo de dependências (Clean Architecture): `Api` → `Infrastructure` → `Application` → `Domain`. O `Domain` não conhece nada de fora; a `Application` só conhece interfaces; a `Infrastructure` implementa essas interfaces; a `Api` só orquestra.

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) com o backend WSL2 funcionando
  - **No Windows, se o Docker Desktop não iniciar** com um erro tipo "Docker Desktop is unable to start": provavelmente o WSL2 não tem nenhuma distribuição instalada. Rode `wsl --status` num PowerShell — se aparecer "não tem distribuições instaladas", rode `wsl --install` (requer reiniciar o computador depois) e abra o Docker Desktop de novo.
- Ferramenta `dotnet-ef` (`dotnet tool install --global dotnet-ef`)

## Rodando localmente (para testar)

1. **Subir Postgres + MinIO:**
   ```bash
   cd backend
   docker compose up -d
   ```

2. **Aplicar as migrations** (cria as tabelas) — isso também acontece automaticamente ao rodar a API (`DbInitializer.SeedAdminAsync` chama `db.Database.MigrateAsync()`), mas pode ser feito manualmente:
   ```bash
   dotnet ef database update --project src/CadernetaObras.Infrastructure --startup-project src/CadernetaObras.Api
   ```

3. **Aplicar os triggers de imutabilidade** (rodar uma vez, depois que as tabelas existirem):
   ```bash
   docker exec -i caderneta-postgres psql -U caderneta_app -d gestao_obras < scripts/immutability-triggers.sql
   ```

4. **Rodar a API:**
   ```bash
   dotnet run --project src/CadernetaObras.Api
   ```
   A API sobe em `https://localhost:5xxx` (a porta exata aparece no console). No primeiro start, se `Bootstrap:AdminCpf`/`Bootstrap:AdminSenha` estiverem configurados em `appsettings.Development.json` (já vêm preenchidos com valores de teste), o primeiro usuário **Administrador** é criado automaticamente.

5. **Testar o login** (usuário de bootstrap, definido em `appsettings.Development.json`):
   ```bash
   curl -k -X POST https://localhost:5xxx/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"cpf":"000.000.000-00","senha":"TrocarNoPrimeiroLogin123"}'
   ```
   Isso retorna um JWT — use-o no header `Authorization: Bearer <token>` para chamar os demais endpoints.

## Endpoints principais

| Método | Rota | Quem pode | Descrição |
|---|---|---|---|
| POST | `/api/auth/login` | Público | Login (CPF + senha) → JWT |
| GET/POST | `/api/usuarios` | Administrador | Listar/cadastrar usuários (Admin exige `SenhaSecretaDev`) |
| GET/POST | `/api/obras` | Todos / Administrador | Listar (filtrado por perfil) / criar obra |
| GET | `/api/obras/{id}` | Envolvidos na obra | Detalhe |
| POST | `/api/obras/{id}/assinar` | Engenheiro/Proprietário da obra | Assina a abertura (gera hash+IP+timestamp) |
| GET/POST | `/api/obras/{obraId}/registros` | Envolvidos / Engenheiro | Listar / criar registro de visita |
| POST | `/api/registros/{id}/imagens` | Engenheiro | Upload de foto (multipart/form-data, campo `arquivo`) |
| POST | `/api/registros/{id}/assinar` | Engenheiro/Proprietário da obra | Assina o registro |
| POST | `/api/termos-conclusao` | Engenheiro | Emite termo de conclusão |
| POST | `/api/termos-conclusao/{id}/assinar` | Engenheiro/Proprietário | Assina o termo (finaliza a obra quando ambos assinam) |
| GET | `/health` | Público | Health check |

## O que esse backend já resolve (em relação ao protótipo visual)

- Login real com JWT + Argon2id (não é mais "qualquer CPF e senha 4+").
- Assinatura real: hash SHA-256 do conteúdo exato do registro, IP e user-agent capturados no servidor (nunca enviados pelo cliente), timestamp do servidor (`DateTime.UtcNow`, nunca do navegador).
- Imutabilidade em duas camadas: a Application recusa alterar/excluir uma vez assinado (e nem expõe endpoints de update/delete para registros assinados), e o Postgres tem triggers que bloqueiam fisicamente qualquer `UPDATE`/`DELETE` indevido — mesmo via acesso direto ao banco.
- RBAC aplicado em cada use case (Administrador/Engenheiro/Proprietário), não só na UI.

## O que ainda falta (próximos passos do ROADMAP.md)

- Testes automatizados (unitários dos use cases, principalmente do motor de assinatura).
- Geração de PDF está implementada (`QuestPdfService`) mas sem endpoint HTTP exposto ainda — falta um `GET /api/obras/{id}/pdf` e `GET /api/registros/{id}/pdf`.
- Envio de e-mail real (hoje a senha provisória do usuário criado é devolvida na resposta da API — em produção isso precisa ir por e-mail, nunca na resposta HTTP).
- Log de auditoria (Fase 8 do roadmap).
- Conectar o frontend React a esta API (hoje o frontend ainda usa `mock-data.ts`).
- Deploy na VPS via Docker Compose + Nginx + HTTPS (Fase 10).
- Webhooks/MCP/agente de IA (Fase 9).

## Segredos e configuração

`appsettings.Development.json` já vem com valores de desenvolvimento (senha de bootstrap, chave JWT, credenciais do MinIO/Postgres batendo com o `docker-compose.yml`) — **esses valores nunca devem ser usados em produção**. Em produção, configure tudo via variáveis de ambiente ou um cofre de segredos (nunca commitar `appsettings.Production.json` com segredos reais):

```
ConnectionStrings__Postgres
Jwt__SigningKey
Minio__AccessKey / Minio__SecretKey
SegurancaAdmin__SenhaSecretaDev
Bootstrap__AdminCpf / Bootstrap__AdminSenha (usar só uma vez e depois remover)
```
