# Como iniciar o projeto (Caderneta de Obras Digital)

Este projeto tem dois scripts prontos na raiz da pasta que fazem tudo
automaticamente. Você não precisa digitar comando nenhum manualmente.

## Pré-requisitos (instalar uma única vez neste computador)

Antes de usar os scripts, instale:

1. **.NET SDK 10** — https://dotnet.microsoft.com/download
2. **Docker Desktop** — https://www.docker.com/products/docker-desktop/
   - No Windows, se o Docker não abrir (erro "Docker Desktop is unable to
     start"), abra um PowerShell e rode `wsl --install`, reinicie o
     computador e abra o Docker Desktop de novo.
3. **Bun** (opcional, mas recomendado) — https://bun.sh
   - Se não instalar o Bun, os scripts usam automaticamente o `npm`
     (que já vem junto com o Node.js — https://nodejs.org).

## Passo 1 — Configuração inicial (só na primeira vez neste PC)

Dê **duplo-clique** no arquivo:

```
primeira-configuracao.bat
```

Ele vai, sozinho:
- Instalar as dependências do frontend e do backend.
- Subir o banco de dados (Postgres) e o armazenamento de arquivos (MinIO).
- Criar as tabelas do banco (migrations).
- Aplicar as regras de segurança do banco (triggers de imutabilidade).

Isso demora alguns minutos na primeira vez (baixa imagens do Docker,
pacotes do .NET e do frontend). Espere terminar e aparecer a mensagem
"Configuração concluída!".

> Só rode esse arquivo **uma vez** por computador. Rodar de novo não
> quebra nada, mas não é necessário.

## Passo 2 — Iniciar o sistema (sempre que for usar)

Dê **duplo-clique** no arquivo:

```
iniciar-projeto.bat
```

Ele abre 3 janelas de terminal automaticamente, uma para cada parte do
sistema:

1. **Banco de dados** (Postgres + MinIO, via Docker)
2. **Backend** (API em .NET)
3. **Frontend** (a tela do sistema)

Espere uns 20 a 30 segundos (as janelas mostram o progresso) e depois
acesse no navegador:

```
http://localhost:8080
```

**Não feche essas 3 janelas** enquanto estiver usando o sistema — é lá
que ele está rodando. Para desligar o sistema, é só fechar as 3 janelas
(ou apertar Ctrl+C dentro de cada uma).

## Login inicial

Na primeira vez que o sistema sobe neste computador, ele já cria
automaticamente o primeiro usuário Administrador:

| CPF | Senha |
|---|---|
| `000.000.000-00` | `TrocarNoPrimeiroLogin123` |

A partir daí, esse Administrador cadastra os demais usuários
(Engenheiros/Arquitetos e Proprietários) pela própria tela do sistema,
em **Usuários**.

## Enviando o projeto para outro computador

Para levar o projeto para outra máquina, é só:

1. Fechar as 3 janelas de terminal (se estiverem abertas).
2. Compactar (zip) a pasta inteira do projeto.

   Você **pode deletar antes de zipar** (não são necessárias e só
   deixam o zip mais pesado — são geradas de novo automaticamente):
   - `node_modules/`
   - `.output/`
   - `backend/src/*/bin` e `backend/src/*/obj`

3. Enviar o `.zip`.
4. Na máquina nova: extrair, instalar os pré-requisitos acima, e rodar
   `primeira-configuracao.bat` seguido de `iniciar-projeto.bat`.

## Problemas comuns

- **"Porta já em uso" / o navegador não abre nada**: alguma instância
  anterior do backend ou do frontend ainda está rodando. Feche todas as
  janelas de terminal do projeto e rode `iniciar-projeto.bat` de novo.
- **Docker não sobe**: confirme que o Docker Desktop está aberto (ícone
  na bandeja do sistema) antes de rodar os scripts.
- **Erro ao aplicar migrations**: normalmente é porque o Postgres ainda
  não tinha terminado de subir. Rode `primeira-configuracao.bat` de novo.
