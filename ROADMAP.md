# ROADMAP — Sistema de Caderneta de Obras Digital (CREA)

> **Este arquivo é a fonte de verdade do progresso do projeto.**
> Se você é uma IA (ou dev) retomando este projeto, leia primeiro a seção **"STATUS ATUAL"** abaixo antes de qualquer coisa. Atualize essa seção sempre que uma fase for concluída.

---

## STATUS ATUAL

- **Fase em andamento:** Backend das Fases 2–7 escrito e compilando (código completo), aguardando infraestrutura local para testar de ponta a ponta.
- **Última atualização:** 2026-08-15
- **Próximo passo imediato — BLOQUEIO ATIVO:** o usuário optou por adiar a correção do Docker/WSL. Para retomar: rodar `wsl --install` num PowerShell como administrador (baixa uma distro Linux; exige reiniciar o PC), abrir o Docker Desktop, confirmar que inicia sem erro, e então:
  1. `cd backend && docker compose up -d`
  2. `dotnet ef database update --project src/CadernetaObras.Infrastructure --startup-project src/CadernetaObras.Api`
  3. `docker exec -i caderneta-postgres psql -U caderneta_app -d gestao_obras < scripts/immutability-triggers.sql`
  4. `dotnet run --project src/CadernetaObras.Api` e testar `POST /api/auth/login` com o usuário de bootstrap (ver `backend/README.md`)
  5. Depois disso: conectar o frontend React à API real (troca de `mock-data.ts` por chamadas HTTP) e expor os endpoints de PDF que faltam.
- **O que já existe no backend (`backend/`, ver `backend/README.md` para detalhes):** solução .NET 10 em Clean Architecture completa (Domain/Application/Infrastructure/Api/Tests), login JWT + Argon2id, CRUD de Obra/Registro/TermoConclusao, motor de assinatura digital real (hash SHA-256 do conteúdo + IP + user-agent + timestamp do servidor, tudo capturado no servidor), triggers Postgres de imutabilidade, geração de PDF (sem endpoint exposto ainda), upload de fotos para MinIO, cadastro de usuário com os 4 tipos e senha secreta de Admin, seed do primeiro Administrador via config. 12 testes unitários passando. **Ainda não testado contra um banco real** por causa do bloqueio de Docker/WSL acima — o frontend continua rodando 100% em cima de `mock-data.ts`, nada está de fato integrado ainda.
- **Decisões já tomadas (não reabrir sem motivo forte):**
  - Banco de dados: migrar de SQL Server (rascunho original) para **PostgreSQL**, **self-hosted na VPS própria** (sem Supabase).
  - Backend: **.NET (C#) com Clean Architecture** (Domain / Application / Infrastructure / API em camadas) — ver seção 3.6.
  - Frontend: **React 19 + Vite + TanStack Router/Start + TailwindCSS v4 + shadcn/ui (Radix)**, como PWA responsivo (tablet-first, depois notebook/desktop), consumindo a API .NET via HTTP (sem acesso direto ao banco pelo cliente). Obs: o Lovable gerou com **TanStack Router**, não `react-router-dom` como o prompt original pedia — mantido, pois é equivalente e já está funcionando (roteamento por arquivo em `src/routes/`).
  - Auth: implementada no backend .NET (JWT + hash de senha com Argon2id/BCrypt via biblioteca .NET), sem Supabase Auth.
  - Storage de arquivos (fotos, PDFs): **MinIO** (self-hosted, compatível com S3) rodando na mesma VPS, ou disco da VPS com API própria — decidir na Fase 3.
  - Geração de PDF: biblioteca C# (ex: QuestPDF) rodando no backend .NET.
  - Deploy: containers Docker na VPS própria (Postgres, MinIO, API .NET), reverse proxy Nginx. Frontend pode ficar na mesma VPS ou em provedor separado (decidir na Fase 10).
  - Assinatura digital: nível **"assinatura eletrônica avançada"** (Lei 14.063/2020) — hash SHA-256 do conteúdo do registro + login + IP + user-agent + timestamp do servidor. Sem certificado ICP-Brasil por enquanto.
  - Paleta de cores: 70% azul / 20% amarelo / 10% branco (ver seção Design System).
  - Arquitetura de pastas do frontend: por *feature*, não por tipo de arquivo (ver seção Arquitetura).
  - Início do desenvolvimento visual: **Lovable**, começando pela tela de Login (o Lovable gera só o front — a integração com a API .NET vem depois, localmente).
  - **Modelo de permissões (RBAC) definido — ver seção 3.5.** Tabela `Usuarios` unificada (Admin/Engenheiro/Proprietário logam da mesma forma). Toda entidade assinável (Obra, Relato_Visita, Termo_Conclusao) exige assinatura do Engenheiro **e** do Proprietário para ficar válida/imutável. Autorização por perfil aplicada na camada Application do backend .NET; imutabilidade reforçada também por trigger no Postgres.
  - **Integrações de IA planejadas (webhooks, MCP, agente interno) — ver seção 3.7.** O sistema deve expor o máximo possível de funções via webhook e MCP para agentes de IA externos, além de ter um agente de IA embutido no próprio sistema que só acessa os dados que o usuário logado tem permissão de ver (respeita o RBAC da seção 3.5).
  - Prompt do Lovable (seção 5) agora cobre **todas as telas visuais principais de uma vez** (Fases 1 e 2 do roadmap foram unificadas), não só o Login.

---

## 1. Contexto do projeto

Sistema web (TCC — Centro Universitário de Adamantina) para digitalizar a Caderneta de Obras usada por profissionais registrados no CREA Regional. Documentos de referência na pasta:
- `Cópia de JOAO FABRICIO - Criando o Manual do Sistema14_03_2026 (1).docx` — Especificação de Requisitos (ERS): requisitos funcionais/não funcionais, casos de uso.
- `gemini-code-1780270152642 (1).sql` — schema original em SQL Server (será adaptado para Postgres).

Entidades centrais: `Obra` (caderneta), `Profissional`, `Proprietario`, `Usuarios`, `Relato_Visita`, `Termo_Conclusao`, tabelas de `Assinatura*` e `Imagem`.

Uso majoritário em **tablet** (em campo, na obra) e **notebook** (escritório) — interface deve ser touch-first e responsiva.

---

## 2. Design System

### Paleta de cores (70% azul / 20% amarelo / 10% branco)

| Papel | Cor | Hex | Uso |
|---|---|---|---|
| Azul primário (dominante) | Azul institucional escuro | `#1E3A8A` | Header, sidebar, botões primários, textos de destaque |
| Azul secundário | Azul médio | `#2563EB` | Links, ícones ativos, estados hover |
| Azul claro | Azul muito claro | `#DBEAFE` | Backgrounds de cards, estados selecionados |
| Amarelo (destaque/ação) | Âmbar | `#F59E0B` | Botões de ação crítica (assinar, confirmar), alertas, badges de status |
| Amarelo claro | Âmbar claro | `#FEF3C7` | Backgrounds de aviso/pendência |
| Branco/neutro | Branco puro | `#FFFFFF` | Fundo principal de telas, cards |
| Texto/neutro escuro | Cinza-azulado | `#1F2937` | Texto principal (contraste, não é "cor" da marca mas necessário) |

Regra de proporção visual: fundo e estrutura em azul (header, nav, botões padrão), amarelo reservado para **ações que exigem atenção** (assinar, alertas, pendências) — nunca usar amarelo como cor de fundo geral, senão perde o efeito de destaque.

### Componentes base (via shadcn/ui)
Button, Input, Card, Dialog (modais de assinatura), Table (listagem de cadernetas/registros), Badge (status: Em andamento / Finalizada / Pendente assinatura), Tabs, Toast (feedback de sucesso/erro), Canvas de assinatura (componente customizado).

---

## 3. Arquitetura de pastas — Frontend (Clean, por feature)

Objetivo: qualquer pessoa (ou IA) deve conseguir abrir a pasta `src/features/<nome>` e entender uma funcionalidade inteira sem pular entre 5 pastas diferentes.

```
frontend/
└── src/
    ├── app/                        # Rotas (React Router)
    │   ├── login/
    │   ├── cadernetas/
    │   ├── cadernetas/[id]/
    │   ├── cadernetas/[id]/registros/[id]/
    │   └── layout.tsx
    │
    ├── features/                   # Lógica de negócio por domínio
    │   ├── auth/
    │   │   ├── components/         # LoginForm, etc.
    │   │   ├── hooks/               # useAuth, useSession
    │   │   ├── services/            # login(), logout() -> chama a API .NET
    │   │   └── types.ts
    │   ├── obras/                  # Cadernetas / Obra
    │   ├── registros/              # Relato_Visita
    │   ├── termo-conclusao/
    │   ├── assinatura/              # Captura (canvas) + chamada ao endpoint de assinatura
    │   └── pdf/                     # Download/visualização de PDFs gerados pelo backend
    │
    ├── components/                  # Componentes de UI genéricos, sem lógica de negócio
    │   ├── ui/                      # shadcn/ui (button, input, card, dialog...)
    │   └── layout/                  # Header, Sidebar, PageContainer
    │
    ├── lib/                         # Infraestrutura compartilhada
    │   ├── api/                     # client.ts (axios/fetch com baseURL da API .NET + interceptor de JWT)
    │   ├── utils.ts
    │   └── constants.ts             # cores, enums de status, enums de fase de obra
    │
    ├── hooks/                       # Hooks globais (não específicos de uma feature)
    └── types/                       # Tipos globais compartilhados (espelham DTOs da API)
```

**Regra de ouro:** se um componente/hook/service só é usado por uma feature, ele mora dentro da pasta daquela feature. Só sobe para `components/` ou `hooks/` globais se for reaproveitado por 2+ features. O frontend **nunca** acessa o Postgres diretamente — sempre via API .NET.

## 3.6 Arquitetura de pastas — Backend (.NET, Clean Architecture)

Camadas separadas por responsabilidade e por dependência (Domain não depende de nada; API depende de tudo). Isso facilita muito o debug: um bug de regra de negócio está em `Application`, um bug de banco está em `Infrastructure`, um bug de rota/HTTP está em `API`.

```
backend/
└── src/
    ├── CadernetaObras.Domain/            # Núcleo — sem dependência de nada externo
    │   ├── Entities/                     # Obra, RelatoVisita, TermoConclusao, Usuario, Assinatura, Imagem
    │   ├── Enums/                        # PerfilUsuario, StatusObra, StatusRegistro
    │   └── Interfaces/                   # IObraRepository, IAssinaturaRepository (contratos, sem implementação)
    │
    ├── CadernetaObras.Application/       # Casos de uso / regras de negócio
    │   ├── Obras/                        # CriarObraUseCase, ListarObrasUseCase...
    │   ├── Registros/                    # CriarRelatoVisitaUseCase...
    │   ├── Assinaturas/                  # AssinarEntidadeUseCase (gera hash, valida dupla assinatura)
    │   ├── Auth/                         # LoginUseCase, GerarTokenUseCase
    │   ├── DTOs/                         # Objetos de entrada/saída da API
    │   └── Interfaces/                   # IHashService, IPdfService, IStorageService (contratos)
    │
    ├── CadernetaObras.Infrastructure/    # Implementações concretas
    │   ├── Persistence/                  # DbContext (EF Core), Migrations, Repositories
    │   ├── Auth/                         # Implementação JWT, Argon2id/BCrypt
    │   ├── Storage/                      # Implementação MinIO/S3
    │   └── Pdf/                          # Implementação QuestPDF
    │
    ├── CadernetaObras.API/               # Camada HTTP (a única que "conversa" com o frontend)
    │   ├── Controllers/                  # ObrasController, RegistrosController, AuthController...
    │   ├── Middlewares/                  # Autenticação, tratamento global de erros
    │   └── Program.cs
    │
    └── tests/
        └── CadernetaObras.Tests/         # Testes unitários (Application) e de integração (API)
```

**Regra de ouro:** `Domain` não importa nada de fora. `Application` só conhece interfaces (nunca EF Core, nunca MinIO diretamente). `Infrastructure` implementa essas interfaces. `API` só orquestra chamadas para `Application`. Isso permite trocar Postgres, storage ou biblioteca de PDF no futuro sem tocar em regra de negócio.

---

## 3.5 Modelo de Permissões (RBAC) e regras de negócio

### Perfis (papel `perfil` na tabela `Usuarios`)

| Perfil | Pode criar | Pode visualizar | Pode assinar | Pode editar/excluir após assinatura |
|---|---|---|---|---|
| **Administrador** (CREA) | Obra (define engenheiro + proprietário responsáveis) | Todas as Obras e registros | Não assina nada | **Nunca** — nada assinado pode ser apagado/editado por ninguém, nem Admin |
| **Engenheiro/Arquiteto** | Relato_Visita (registro diário, com fotos) das Obras em que está atribuído | Apenas as Obras em que está atribuído + seus registros | Obra, Relato_Visita, Termo_Conclusao (nas obras em que participa) | Nunca |
| **Proprietário** | Nada | Apenas a Obra em que está atribuído + registros dessa obra | Obra, Relato_Visita, Termo_Conclusao (apenas leitura + assinatura, nunca cria conteúdo) | Nunca |

### Regras de estado / validade

- **Obra**: criada pelo Administrador em estado `pendente_assinatura`. Só se torna `ativa` quando **Engenheiro E Proprietário** assinaram a abertura. Antes disso, obra existe mas não é "válida/utilizável" (não pode receber Relato_Visita).
- **Relato_Visita (registro diário)**: só o Engenheiro cria (com fotos). Fica `pendente_assinatura` até Engenheiro **e** Proprietário assinarem. Após ambas as assinaturas → `imutável` (bloqueio total de UPDATE/DELETE, inclusive para Admin).
- **Termo_Conclusao**: idem — Engenheiro finaliza a caderneta, mas só é válido após assinatura de **ambos** (Engenheiro + Proprietário).
- **Regra transversal**: nenhum registro que já possui assinatura pode ser alterado ou excluído por **nenhum perfil**, incluindo Administrador. Isso deve ser garantido em duas camadas (defesa em profundidade):
  1. **Camada Application (.NET)**: toda operação de UPDATE/DELETE passa por um use case que valida perfil do usuário autenticado (via JWT) e verifica se a entidade já possui assinatura antes de permitir a ação.
  2. **Trigger no Postgres** (`BEFORE UPDATE OR DELETE`) que verifica se existe assinatura vinculada e lança exceção — assim, mesmo um bug na Application layer não consegue burlar a regra, porque o próprio banco recusa.

### Ajustes no schema original (SQL Server → Postgres)

1. **Unificar `Usuarios` + `Profissional` + `Proprietario` em uma única tabela `Usuarios`:**
   ```
   Usuarios (
     id_usuario UUID PK (igual ao auth.users do Supabase),
     nome VARCHAR NOT NULL,
     cpf VARCHAR UNIQUE NOT NULL,
     telefone VARCHAR NOT NULL,
     perfil ENUM('administrador','engenheiro','proprietario') NOT NULL,
     titulo_profissional VARCHAR NULL,   -- só para perfil = engenheiro
     numero_registro VARCHAR NULL        -- só para perfil = engenheiro (registro CREA)
   )
   ```
   (senha/autenticação fica a cargo do Supabase Auth, não fica em texto na tabela)

2. **Obra**: `id_profissional` e `id_proprietario` passam a ser FK para `Usuarios` (perfil restrito via constraint/validação). Adicionar `id_administrador` (quem criou), `status` (`pendente_assinatura` / `ativa` / `finalizada`) e `valor_obra` (`DECIMAL(14,2) NOT NULL` — valor total orçado/contratado da obra, em R$).

3. **Assinatura_obra / Assinatura (relato) / Assinatura_termo_conclusao**: adicionar `id_usuario` (FK Usuarios, quem assinou) + `papel` (engenheiro/proprietario, derivado do perfil no momento da assinatura) + `user_agent`. Constraint `UNIQUE(entidade_id, id_usuario)` para impedir assinatura duplicada. A entidade só vira `ativa`/`imutável` quando existir 1 assinatura de cada papel exigido.

4. **Relato_Visita**: adicionar `status` (`pendente_assinatura` / `assinado`) para facilitar consulta sem precisar contar assinaturas toda hora.

---

## 3.7 Integrações de IA (Webhooks, MCP, Agente Interno)

Essa camada só faz sentido **depois** que o núcleo (obras, registros, assinaturas, permissões) estiver funcionando — ela se apoia inteiramente nos use cases já existentes na camada `Application`, então não implementar antes da Fase 8.

### Webhooks (eventos do sistema → sistemas externos)
- Disparados a partir de **domain events** já naturais ao fluxo (ex: `ObraCriada`, `ObraAtivada` [dupla assinatura completa], `RegistroVisitaCriado`, `RegistroVisitaAssinado`, `CadernetaFinalizada`).
- Implementado como um módulo dentro de `Application`/`Infrastructure`: um `IEventDispatcher` que a cada evento de domínio chama os webhooks cadastrados (URL + segredo compartilhado para assinatura HMAC do payload, evitando spoofing).
- Cadastro de webhooks fica restrito ao perfil Administrador (é integração institucional, não individual).

### MCP Server (agentes de IA externos, ex: Claude, ChatGPT com MCP)
- Um servidor MCP separado (processo próprio) que expõe como *tools* os mesmos use cases da camada `Application` — nunca acesso direto ao Postgres.
- Cada chamada ao MCP exige autenticação (token vinculado a um `Usuario` real) e **respeita o mesmo RBAC da seção 3.5** — um agente externo autenticado como Engenheiro só consegue consultar/agir dentro das obras daquele Engenheiro, nunca visualizar dados de outras.
- Prioridade inicial: tools de **leitura/consulta e análise** (ex: "resumir progresso da obra X", "listar registros pendentes de assinatura") — tools de escrita (criar registro, assinar) só depois de validado o fluxo de leitura, por segurança.

### Agente de IA interno (assistente embutido no próprio sistema)
- Um chat/assistente dentro da aplicação, disponível para todos os perfis, mas **escopado**: as perguntas do usuário são respondidas usando só os use cases de leitura que aquele usuário já teria permissão de chamar manualmente (reaproveita a mesma autorização da API, nunca uma rota "especial" com mais acesso).
- Tecnicamente: a IA (via API da Anthropic) recebe *tool calling* restrito às mesmas queries permitidas ao perfil do usuário autenticado — o agente nunca roda SQL livre, sempre passa pelos use cases da `Application`.
- Reserva-se espaço visual para esse assistente já no design (botão/painel flutuante), mas a lógica funcional entra só na Fase 8 do roadmap.

---

## 4. Roadmap de desenvolvimento (fases)

### Fase 0 — Planejamento ✅ CONCLUÍDA
- [x] Análise da ERS e schema SQL original
- [x] Pesquisa de stack, segurança e mecanismo de assinatura digital
- [x] Definição de paleta de cores e arquitetura de pastas
- [x] Criação deste roadmap

### Fase 1 — Design visual completo no Lovable (todas as telas, sem lógica de backend)
- [x] Rodar o prompt da seção 5 no Lovable (cobre Login, Design System e todas as telas principais de uma vez)
- [x] Validar todas as telas (visual, responsividade tablet/desktop, os 3 estados de perfil) — testado via Playwright nos 3 perfis, zero erros de console/HTTP
- [x] Validar componentes base do design system gerados (botões, inputs, cards, badges de status)
- [x] Exportar/conectar o projeto Lovable ao GitHub — remoto `origin` aponta para `CREA_Cardeneta_de_Obras` (repo oficial), remoto `lovable` mantido à parte para futuras gerações
- [x] Reorganizar os arquivos gerados pelo Lovable de acordo com a estrutura de pastas da seção 3 — o Lovable já seguiu a estrutura pedida
- [x] Extras incorporados durante a Fase 1 (além do prompt original): recuperação de senha, cadastro de usuário com 4 tipos (Admin/Engenheiro/Arquiteto/Proprietário) e campo "Valor da obra"

### Fase 2 — Infraestrutura de backend
- [x] Criar solução .NET seguindo a estrutura Clean Architecture da seção 3.6 (`Domain`, `Application`, `Infrastructure`, `API`) — pasta `backend/`, build limpo, 12 testes unitários passando
- [x] Migrar schema SQL (SQL Server → PostgreSQL) já com o modelo unificado de `Usuarios` + `perfil` (ver seção 3.5), usando EF Core Migrations — migration `InitialCreate` gerada
- [x] Criar triggers `BEFORE UPDATE OR DELETE` de imutabilidade em Obra, Relato_Visita e Termo_Conclusao (bloqueio mesmo para Admin, uma vez assinado) — `backend/scripts/immutability-triggers.sql`
- [x] Configurar Docker Compose local (Postgres + MinIO) para desenvolvimento — `backend/docker-compose.yml`
- [ ] **BLOQUEADO:** rodar o stack local de verdade (`docker compose up`) e aplicar a migration — Docker Desktop não inicia nesta máquina porque o WSL2 não tem nenhuma distribuição instalada (`wsl --status` confirma). Precisa rodar `wsl --install` (exige reboot) antes de continuar. Todo o código já está pronto e compila, só falta essa etapa de infraestrutura local.
- [ ] Provisionar PostgreSQL na VPS (via Docker, container isolado) — **usar `root_plan` antes de `root_execute`** ao mexer na VPS, conforme protocolo da ferramenta de administração
- [ ] Provisionar MinIO (S3-compatible) na mesma VPS, container separado, para fotos e PDFs

### Fase 3 — Autenticação e perfis
- [x] Implementar `AuthController` (.NET): login com CPF/senha, hash via Argon2id, emissão de JWT — `POST /api/auth/login`
- [x] Autorização por perfil na API — cada use case da Application valida `ICurrentUserService.Perfil` (Admin / Engenheiro / Proprietário) antes de agir, em vez de só `[Authorize(Roles=...)]` genérico
- [x] Cadastro de usuário via API (`POST /api/usuarios`, só Administrador) com os 4 tipos (Admin/Engenheiro/Arquiteto/Proprietário) e senha secreta de dev para novo Admin, espelhando o formulário do frontend
- [x] Seed do primeiro Administrador via `Bootstrap:AdminCpf`/`AdminSenha` na configuração (resolve o problema de "quem cadastra o primeiro Admin")
- [ ] Conectar tela de Login do frontend à API real (RF08) — ainda usa `mock-data.ts`
- [ ] Middleware de proteção de rotas no frontend por perfil (menus e ações diferentes por perfil) — já existe visualmente com `PerfilProvider`, falta trocar a fonte de verdade do mock para o JWT real
- [ ] Testar o fluxo fim a fim (aguardando desbloqueio do Docker/WSL da Fase 2)

### Fase 4 — CRUD de Obras/Cadernetas
- [x] Administrador cria Caderneta (RF01) com cálculo automático de área total edificada, atribuindo Engenheiro + Proprietário responsáveis (RF02) — `POST /api/obras`, `CriarObraUseCase`
- [x] Obra nasce em `PendenteAssinatura` — só vira `Ativa` após dupla assinatura (Engenheiro + Proprietário) — `AssinarObraUseCase`
- [x] Listagem filtrada por perfil: Admin vê tudo, Engenheiro/Proprietário veem só as obras em que estão atribuídos — `GET /api/obras`, `ObraRepository.ListarVisiveisAsync`
- [ ] Testado contra um Postgres real (aguardando Fase 2 desbloquear)

### Fase 5 — Registros de Visita
- [x] Engenheiro cria Relato de Visita com fases de serviço (RF03) — Admin e Proprietário não criam — `POST /api/registros`, valida `obra.Status == Ativa` e que o usuário é o profissional atribuído
- [x] Upload de imagens vinculadas ao relato (só Engenheiro, no momento da criação, obra ainda `PendenteAssinatura` do registro) — `POST /api/registros/{id}/imagens` → MinIO
- [ ] Ocorrências vinculadas ao relato (RF04) — ainda não modelado como entidade separada; hoje cabe dentro de `DecisoesOrientacoes`, avaliar se precisa de tabela própria
- [x] Proprietário só visualiza e assina, nunca cria/edita — reforçado tanto no use case quanto na ausência de endpoints de update/delete

### Fase 6 — Motor de Assinatura Digital
- [ ] Componente de captura via canvas (touch/mouse) — segue existindo só no frontend (visual); o backend não depende do desenho, só confirma a ação de assinar
- [x] Fluxo de dupla assinatura: Engenheiro assina → aguarda Proprietário assinar (ou ordem inversa) → só com as duas o registro vira `imutável` — implementado igual para Obra, Relato de Visita e Termo de Conclusão
- [x] Geração de hash SHA-256 do conteúdo do registro no momento de cada assinatura — `Sha256HashService` + conteúdo canônico montado por entidade
- [x] Registro de `usuario_id`, `papel`, IP, user-agent, timestamp do servidor por assinatura — capturados 100% no backend via `ICurrentUserService`, nunca enviados pelo cliente
- [x] Bloqueio de edição/exclusão após dupla assinatura confirmada (RF06, RNF04) — validado na Application layer (`EntidadeImutavelException`) e reforçado por trigger no Postgres (`scripts/immutability-triggers.sql`)
- [x] Testes unitários cobrindo o motor de assinatura (primeira assinatura, dupla assinatura ativa a obra, usuário não atribuído, assinatura duplicada, entidade já imutável) — `AssinarObraUseCaseTests`, 12/12 passando
- [ ] (Opcional/avançado) Integração com Timestamp Authority (RFC 3161) para reforçar não-repúdio
- [ ] Testado contra um Postgres real (aguardando Fase 2 desbloquear) — inclusive validar que o trigger do Postgres realmente barra um `UPDATE` direto

### Fase 7 — Geração de PDF
- [x] Template de PDF de registro individual (RF07) — `QuestPdfService.GerarPdfRegistro`, mas ainda **sem endpoint HTTP exposto**
- [x] Template de PDF da caderneta completa/finalizada — `QuestPdfService.GerarPdfCaderneta`, mesma pendência de endpoint
- [ ] Expor `GET /api/obras/{id}/pdf` e `GET /api/registros/{id}/pdf`
- [ ] Armazenamento do PDF gerado no MinIO (documento oficial imutável — RF05)

### Fase 8 — Auditoria e segurança
- [ ] Log de auditoria (data, hora, usuário, ação) — RF09
- [ ] Revisão das autorizações por perfil na Application layer + triggers de imutabilidade
- [ ] Checklist de segurança (HTTPS, variáveis de ambiente/secrets fora do código, rate limiting no login)

### Fase 9 — Integrações de IA (Webhooks, MCP, Agente Interno)
- [ ] Implementar `IEventDispatcher` e domain events (`ObraCriada`, `ObraAtivada`, `RegistroVisitaAssinado`, `CadernetaFinalizada`)
- [ ] Endpoint de cadastro/gestão de webhooks (só Administrador) + assinatura HMAC do payload
- [ ] Subir servidor MCP expondo use cases de leitura da `Application`, autenticado e respeitando o RBAC da seção 3.5
- [ ] Expandir MCP com tools de escrita (criar registro, etc.), só depois de validado o fluxo de leitura
- [ ] Implementar o agente de IA interno (chat no sistema) com tool calling restrito às queries permitidas ao perfil do usuário logado
- [ ] Conectar o botão/painel do assistente (já reservado visualmente desde a Fase 1) à lógica real

### Fase 10 — Testes e deploy
- [ ] Teste dos 4 fluxos principais de caso de uso (Login, Criar Caderneta, Novo Registro, Finalizar Caderneta)
- [ ] Teste de responsividade real em tablet
- [ ] Deploy do backend (.NET API + Postgres + MinIO) na VPS via Docker Compose, atrás de Nginx com HTTPS (Let's Encrypt)
- [ ] Deploy do frontend (build estático React) — na mesma VPS via Nginx, ou em provedor separado (Vercel/Netlify) apontando para a API da VPS
- [ ] Configurar backups automáticos do Postgres na VPS
- [ ] Documentação final para o manual do sistema (preencher os anexos/diagramas pendentes do DOCX)

---

## 5. Prompt para o Lovable (Fase 1 — todas as telas, apenas visual)

> Copie o bloco abaixo e cole no Lovable. Ele foi escrito para gerar **toda a camada visual do sistema** (todas as telas principais + design system), com dados mockados e sem lógica de backend real — a integração com a API .NET será feita depois, localmente.

```
Crie a interface visual completa de um sistema web chamado "Caderneta de Obras Digital", usado por profissionais do CREA (Administrador do CREA, Engenheiro/Arquiteto e Proprietário do imóvel) para gerenciar o acompanhamento de obras via tablet (uso principal, em campo) e notebook (uso secundário, escritório).

STACK OBRIGATÓRIA:
- React + Vite + TypeScript
- TailwindCSS
- shadcn/ui para os componentes (Button, Input, Card, Label, Toast, Badge, Dialog, Tabs, Textarea, Checkbox, Select)
- react-router-dom para rotas
- Layout responsivo, com prioridade para tablet (768px–1024px) e depois desktop/notebook (1280px+)
- Use dados mockados (arrays fixos no código) para simular obras, registros e usuários — NÃO conectar a nenhum banco de dados ou API real

CONTEXTO DE PERFIS (importante para o design, mesmo sem autenticação real):
Existem 3 perfis com visões diferentes do sistema:
1. Administrador (CREA): vê TODAS as obras, cria novas obras, atribui Engenheiro e Proprietário, não assina nada.
2. Engenheiro/Arquiteto: vê só as obras em que foi atribuído, cria registros diários (com fotos), assina obras/registros/termo de conclusão.
3. Proprietário: vê só a obra em que foi atribuído, é somente leitura + assinatura (nunca cria nada).

Para permitir visualizar as 3 experiências sem lógica de auth real, adicione um seletor de perfil simples (ex: um dropdown discreto no canto, "Visualizando como: Administrador / Engenheiro / Proprietário") que troca os dados mockados e os botões/menus disponíveis na tela.

TELAS A CRIAR:

1. LOGIN
   Logo/nome do sistema, campo CPF, campo senha, botão "Entrar", estado de erro visual ("credenciais inválidas"), indicação de carregamento no botão.

2. DASHBOARD / MENU DE CADERNETAS (pós-login)
   Listagem de obras em cards ou tabela, com: número da caderneta, local da obra, status (badge: "Pendente assinatura" / "Ativa" / "Finalizada"), nome do engenheiro e proprietário responsáveis.
   - Se perfil = Administrador: botão destacado "Criar Nova Obra" e vê todas as obras.
   - Se perfil = Engenheiro: vê só obras atribuídas a ele.
   - Se perfil = Proprietário: vê só a(s) obra(s) dele, sem nenhum botão de criação.

3. CRIAR NOVA OBRA (acessível só ao Administrador)
   Formulário com: seleção de Proprietário (select), seleção de Engenheiro/Arquiteto responsável (select), local da obra, número RT, tipo de edificação, áreas em m² (construir/ampliar/reformar/regularizar — com total calculado automaticamente e exibido em destaque), checkboxes de atividade técnica (direção/execução/fiscalização/projeto), campo empresa/CNPJ (opcional), data do recibo de abertura.
   Ao final do formulário, uma seção "Assinaturas necessárias" mostrando 2 cartões: "Engenheiro — Aguardando assinatura" e "Proprietário — Aguardando assinatura", cada um com um botão "Assinar" (visual, abre um modal com uma área de canvas em branco simulando assinatura manuscrita + botão "Confirmar assinatura").

4. MENU DE VISUALIZAÇÃO DA OBRA (detalhe de uma caderneta)
   Header com dados da obra (local, responsáveis, status, áreas). Abaixo, lista de "Registros de Visita" (cards com data, fase da obra marcada, badge de status "Assinado" / "Pendente assinatura", miniatura de fotos).
   - Botão "Novo Registro" visível só para Engenheiro.
   - Botão "Finalizar Caderneta" visível só para Engenheiro, e só habilitado quando a obra está "Ativa".
   - Proprietário e Administrador veem tudo em modo leitura.

5. NOVO REGISTRO (Relato de Visita) — formulário acessado só pelo Engenheiro
   Campos: data da visita, posição da obra (select), textarea "decisões e orientações", checkboxes das 9 fases de serviço (serviços preliminares, fundação, alvenaria, superestrutura, cobertura, esquadrias/instalações, revestimento, pintura, serviços complementares), área de upload de fotos (grid de preview, pode ser só visual/mock), e a mesma seção de "Assinaturas necessárias" (Engenheiro + Proprietário) do item 3.

6. DETALHE DE UM REGISTRO (somente leitura)
   Mostra todos os dados preenchidos, galeria de fotos, fases marcadas como concluídas visualmente, e um bloco "Assinaturas" com nome de quem assinou, data/hora, e um ícone de "verificado" com hash truncado (ex: "a1b2c3...f9e8").

7. FINALIZAR CADERNETA (Termo de Conclusão) — acessado só pelo Engenheiro
   Formulário simples: data de conclusão, campo de declaração (textarea), e a mesma seção de dupla assinatura.

8. COMPONENTE GLOBAL: Botão flutuante de "Assistente IA"
   Um botão circular flutuante no canto inferior direito (ícone de chat/sparkles), presente em todas as telas pós-login, que ao clicar abre um painel lateral de chat (pode ser só visual, com uma mensagem inicial tipo "Olá! Posso te ajudar a consultar informações das suas obras." e um campo de input desabilitado/mock). Esse espaço será conectado a um agente de IA real futuramente — por enquanto é só a interface.

ESTRUTURA DE PASTAS (siga exatamente):

src/
  app/                    -> páginas/rotas (login, dashboard, obras/[id], obras/[id]/registros/[id], obras/nova, etc.)
  features/
    auth/components/       -> LoginForm.tsx
    obras/components/      -> ObraCard.tsx, ObraForm.tsx, ObraDetalhe.tsx
    registros/components/  -> RegistroForm.tsx, RegistroCard.tsx, RegistroDetalhe.tsx
    termo-conclusao/components/
    assinatura/components/ -> SignatureCanvas.tsx, AssinaturaStatusCard.tsx
    assistente-ia/components/ -> AssistenteButton.tsx, AssistentePanel.tsx
  components/
    ui/                    -> componentes shadcn
    layout/                -> Header.tsx, Sidebar.tsx, PageContainer.tsx, PerfilSwitcher.tsx
  lib/
    mock-data.ts           -> dados fictícios de obras, usuários, registros
    constants.ts           -> cores e constantes de tema

PALETA DE CORES (obrigatória, respeitar as proporções):
- Azul primário (dominante, ~70% da interface): #1E3A8A — usar em header, botões primários, textos de destaque
- Azul secundário: #2563EB — links, ícones, hover
- Azul claro: #DBEAFE — backgrounds de cards e seções
- Amarelo/âmbar (destaque, ~20%, USAR APENAS em ações importantes como "Assinar", "Confirmar", alertas e badges de pendência, nunca como cor de fundo geral): #F59E0B
- Amarelo claro: #FEF3C7 — fundo de avisos/pendências
- Branco (~10%, fundo principal de telas e cards): #FFFFFF
- Texto padrão: #1F2937

ESTILO VISUAL:
Profissional, sério, confiável (é um sistema de responsabilidade técnica de engenharia, não um app casual). Cantos levemente arredondados, sombras sutis, boa área de toque para os botões (mínimo 44px de altura) pensando em uso em tablet.

NÃO IMPLEMENTAR AINDA:
- Não conectar autenticação real, banco de dados ou qualquer API
- Não implementar lógica real de assinatura/hash — o canvas e o botão "Confirmar assinatura" são só visuais
- Não implementar o assistente de IA de verdade — só a interface do painel de chat
- Validações de formulário podem ser só básicas (campos obrigatórios vazios), sem regras de negócio complexas
```

---

## 6. Como retomar este projeto (para outra IA / sessão futura)

1. Leia a seção **STATUS ATUAL** no topo deste arquivo.
2. Confira quais checkboxes da seção **Roadmap de desenvolvimento** já estão marcados.
3. Não reabra as **decisões já tomadas** listadas no topo sem um motivo novo e explícito do usuário.
4. Ao concluir uma fase ou subtarefa, marque o checkbox correspondente e atualize "Última atualização" e "Próximo passo imediato" no topo deste arquivo.
5. Os arquivos de referência originais (ERS em .docx e schema .sql) continuam na mesma pasta e não devem ser apagados — são a fonte de verdade dos requisitos.
