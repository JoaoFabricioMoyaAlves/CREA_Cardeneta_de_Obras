# Documentos A4 e nova assinatura — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Substituir o desenho na tela por visualização do documento + CPF e senha, com documentos A4 montados de dados congelados, e abertura assinada por profissional e Administrador.

**Architecture:** Um único `AssinarDocumentoUseCase` substitui os três use cases atuais e fala com um `IDocumentoHandler` por tipo de documento. Dados ficam como JSON em texto em `documentos_assinaveis`, e o hash é calculado sobre esse texto exato. O frontend renderiza modelos React versionados e imprime pelo navegador.

**Tech Stack:** .NET 10, EF Core + Npgsql, xUnit + Moq; TanStack Start, React, Tailwind; Postgres 17.

**Spec:** `docs/superpowers/specs/2026-11-08-assinatura-documentos-design.md`

## Global Constraints
- Branch `feat/assinatura-documentos`. Deploy só depois de testar tudo localmente, com backup do banco.
- Senha errada retorna 400 (`ValidationAppException`), nunca 401.
- As rotas antigas `POST .../assinar` são removidas, pois não pedem senha.
- JSON do documento em coluna `text`, nunca `jsonb` (o `jsonb` reordena campos e quebraria o hash).
- Assinaturas antigas nunca são alteradas. As novas colunas são anuláveis.
- Sem senhas, tokens ou segredos em arquivos do repositório: ele é público.

## Review Focus
1. Documento muda entre abrir e assinar → 409 com pedido para rever (Task 3).
2. Dois signatários congelando ao mesmo tempo → violação do índice único vira mensagem de "tente novamente", não erro 500 (Task 3).
3. Decisões técnicas longas no relato → texto continua na página seguinte, com lateral em todas as folhas (Task 6).
4. Foto adicionada depois da primeira assinatura → recusada, para não invalidar o hash (Task 2).
5. IPv6 longo ou IP "desconhecido" na faixa lateral → sem estourar a folha (Task 5).

---

### Task 1: Banco e domínio

**Files:**
- Modify: `backend/src/CadernetaObras.Domain/Enums/PapelAssinatura.cs` (`Administrador = 3`)
- Modify: `Domain/Entities/Obra.cs` (`Estado` = "SP"), `AssinaturaObra.cs`, `AssinaturaRelato.cs`, `AssinaturaTermoConclusao.cs` (`string? HashAssinatura`)
- Create: `Domain/Enums/TipoDocumento.cs` (`AberturaObra = 1, RelatoVisita = 2, TermoConclusao = 3`)
- Create: `Domain/Entities/DocumentoAssinavel.cs`, `Domain/Interfaces/IDocumentoAssinavelRepository.cs`
- Create: `Infrastructure/Persistence/Configurations/DocumentoAssinavelConfiguration.cs`, `Infrastructure/Persistence/Repositories/DocumentoAssinavelRepository.cs`
- Modify: `AppDbContext.cs`, `Infrastructure/DependencyInjection.cs`, configurações de Obra e assinaturas
- Modify: `backend/scripts/immutability-triggers.sql` (append-only em `documentos_assinaveis`)
- Create: migration `AssinaturaDocumentos`

**Interfaces:**
- Produces: `DocumentoAssinavel { int Id; TipoDocumento Tipo; int ReferenciaId; int ModeloVersao; string DadosJson; string HashDocumento; DateTime CriadoEm }`, tabela `documentos_assinaveis`, índice único `(Tipo, ReferenciaId)`, `Tipo` como string.
- Produces: `IDocumentoAssinavelRepository { Task<DocumentoAssinavel?> ObterAsync(TipoDocumento tipo, int referenciaId, CancellationToken ct = default); void Adicionar(DocumentoAssinavel documento); }`
- `Obra.Estado`: máx. 2, obrigatório, default no banco `'SP'`. `HashAssinatura`: máx. 64, anulável.

- [ ] Criar entidades, configurações e repositório.
- [ ] Rodar `dotnet ef migrations add AssinaturaDocumentos`; conferir que a migration só adiciona colunas e a tabela.
- [ ] Aplicar localmente com `dotnet ef database update` e o script de gatilhos. Esperado: `to_jsonb` das obras antigas inalterado além de `Estado`.
- [ ] `dotnet build` e `dotnet test` passando. Commit.

### Task 2: Montagem dos documentos

**Files:**
- Create: `Application/Documentos/DadosDocumento.cs`, `IDocumentoHandler.cs`, `DocumentoJson.cs`, `AberturaObraHandler.cs`, `RelatoVisitaHandler.cs`, `TermoConclusaoHandler.cs`
- Modify: `Application/Registros/AdicionarImagemUseCase.cs`, `Application/Obras/{Dtos,ObraMapper,CriarObraUseCase}.cs`
- Test: `backend/tests/CadernetaObras.Tests/Application/DocumentoHandlersTests.cs`

**Interfaces:**
- Produces: `DocumentoJson.Serializar<T>(T dados) -> string` (camelCase, ordem de declaração) e `IHashService.GerarHashSha256` sobre o texto.
- Produces `IDocumentoHandler`:
  - `TipoDocumento Tipo`
  - `Task<ContextoDocumento> CarregarAsync(int referenciaId, CancellationToken ct)`, com `ContextoDocumento { Obra Obra; Usuario Profissional; Usuario Proprietario; bool Pendente; IReadOnlyList<AssinaturaInfo> Assinaturas; }`
  - `Task<string> MontarDadosJsonAsync(ContextoDocumento contexto, CancellationToken ct)`
  - `PapelAssinatura DeterminarPapel(ContextoDocumento contexto, Guid usuarioId, PerfilUsuario perfil)`
  - `void AdicionarAssinatura(ContextoDocumento contexto, NovaAssinatura nova)`
  - `void ConcluirSeCompleto(ContextoDocumento contexto, Guid usuarioId)`
  - `void Notificar(ContextoDocumento contexto, PapelAssinatura papel)`
- `AssinaturaInfo(PapelAssinatura Papel, Guid UsuarioId, DateTime Data, string CodHash, string? HashAssinatura, string Ip, bool Carimbo, string? Autoridade, DateTime? DataCarimbo)`
- `NovaAssinatura(Guid UsuarioId, PapelAssinatura Papel, DateTime Data, string CodHash, string HashAssinatura, string Ip, string UserAgent, CarimboTempo? Carimbo)`

Conteúdo dos dados:
- Abertura: número da caderneta, profissional (nome, título, registro), empresa e CNPJ, proprietário (nome, CPF, telefone), local, cidade, estado, as 4 áreas e o total, tipo e outros, atividades técnicas, R.T., valor, data do recibo.
- Relato: mesmo cabeçalho reduzido, sequência do relato na obra, data, fases, posição, decisões, fotos `{id, nome, sha256}`.
- Termo: profissional, empresa, proprietário, telefone, local, cidade, estado, número, data, declaração.

- [ ] Testes: `Abertura_Admin_SoAposProfissional` (`ForbiddenException` sem a assinatura do profissional), `Abertura_Proprietario_NaoAssina`, `Abertura_CompletaComProfissionalEAdmin_AtivaObraEGravaIdAdministrador`, `Abertura_ComAssinaturaAntigaDeProprietario_AindaExigeAdmin`, `Relato_CompletoComProfissionalEProprietario_FicaAssinado`, `Termo_Completo_FinalizaObra`, `Relato_HashDaFotoEntraNosDados`, `MontarDadosJson_MesmaEntrada_MesmoTexto`, `AdicionarImagem_ComRelatoJaAssinadoPorAlguem_Recusa`.
- [ ] Rodar: falham por tipos inexistentes.
- [ ] Implementar handlers. Abertura notifica todos os Administradores via `ListarPorPerfilAsync` e o novo `INotificacaoService.AberturaAguardandoAdministrador(Usuario admin, Usuario profissional, Obra obra, string caminho)`. Relato e termo mantêm `EngenheiroAssinou`.
- [ ] `CriarObraUseCase`: gravar `Estado`, com `CriarObraRequest.Estado` opcional (nulo vira "SP"). Trocar a notificação ao proprietário de pedido de assinatura por aviso informativo; `DocumentoCriado` da abertura deixa de pedir assinatura.
- [ ] Testes passando. Commit.

### Task 3: Assinatura única e leitura do documento

**Files:**
- Create: `Application/Documentos/ObterDocumentoUseCase.cs`, `AssinarDocumentoUseCase.cs`, `Dtos.cs`
- Create: `Application/Interfaces/ILimiteTentativasAssinatura.cs`, `Infrastructure/Auth/LimiteTentativasAssinatura.cs` (singleton em memória)
- Create: `Api/Controllers/DocumentosController.cs`
- Delete: `Application/Assinaturas/*`, rotas `assinar` de Obras, Registros e Termos
- Modify: `Application/DependencyInjection.cs`, `Domain/Exceptions/DomainException.cs` (`DocumentoAlteradoException`)
- Test: `tests/.../Application/AssinarDocumentoUseCaseTests.cs`; remover `AssinarObraUseCaseTests.cs` depois de portar os cenários

**Interfaces:**
- `GET /api/documentos/{tipo}/{referenciaId}` → `DocumentoResponse(string Tipo, int ReferenciaId, int ModeloVersao, bool Congelado, bool AnteriorAoModelo, string HashDocumento, JsonElement Dados, List<AssinaturaDocumentoDto> Assinaturas, string? PapelQuePodeAssinar)`
- `AssinaturaDocumentoDto(string Papel, string Nome, DateTime Data, string Hash, string Ip, bool CarimboVerificado, string? CarimboAutoridade)`. `Hash` = `HashAssinatura ?? CodHash`.
- `POST /api/documentos/{tipo}/{referenciaId}/assinar` body `AssinarDocumentoRequest(string Cpf, string Senha, string HashDocumentoVisualizado)` → `DocumentoResponse`
- Rota usa `tipo` em kebab-case: `abertura-obra`, `relato-visita`, `termo-conclusao`.
- Visualização: Administrador, profissional da obra e proprietário da obra (mesma regra de `ObterObraUseCase`).
- `HashAssinatura` = SHA-256 de `"{hashDocumento}|{usuarioId}|{papel}|{dataUtc:O}|{ip}"`. `CodHash` = hash do documento.

Ordem em `ExecutarAsync`: carregar → pendente? → limite → CPF (só dígitos, igual ao do usuário logado) e senha → papel → duplicada → dados (congelados, ou monta) → comparar hash exibido → congelar se preciso → carimbo → assinar → concluir → salvar → notificar.

- [ ] Testes: `SenhaErrada_Recusa400_RegistraAuditoria_NaoAssina`, `CpfDeOutroUsuario_Recusa`, `SeisTentativas_BloqueiaAteComSenhaCerta`, `HashDiferente_LancaDocumentoAlterado`, `PrimeiraAssinatura_Congela`, `SegundaAssinatura_UsaCongelado_MesmoComCadastroAlterado`, `HashAssinatura_DifereEntreSignatarios`, `AssinaturaDuplicada_Recusa`, `CarimboIndisponivel_AindaAssina`, `CorridaNoCongelamento_MensagemTenteNovamente`, `Obter_ComAssinaturaSemCongelado_MarcaAnteriorAoModelo`, `Obter_UsuarioSemRelacao_403`.
- [ ] Rodar: falham.
- [ ] Implementar. Remover use cases e rotas antigas.
- [ ] `dotnet test` passando. Commit.

### Task 4: Regras no frontend

**Files:**
- Create: `src/lib/api/documentos.ts`, `src/features/documentos/tipos.ts`
- Modify: `src/lib/api/{obras,registros,termos,types}.ts`, `src/lib/constants.ts`, `src/features/obras/components/ObraForm.tsx`

**Interfaces:**
- Produces: `obterDocumento(tipo: TipoDocumentoApi, referenciaId: number)`, `assinarDocumento(tipo, referenciaId, { cpf, senha, hashDocumentoVisualizado })`, com `TipoDocumentoApi = "abertura-obra" | "relato-visita" | "termo-conclusao"`, `CIDADE_PADRAO = "Adamantina"`, `ESTADO_PADRAO = "SP"`.

- [ ] Remover `assinarObra`, `assinarRegistro`, `assinarTermo`.
- [ ] `ObraForm`: cidade iniciada em Adamantina, novo campo Estado com SP (2 letras, maiúsculas), enviado no payload.
- [ ] `tsc --noEmit` só com erros nos usos removidos, tratados na Task 7. Commit junto com a Task 7 se necessário.

### Task 5: Folha A4 e modelo da abertura — aprovação visual

**Files:**
- Create: `src/features/documentos/FolhaA4.tsx`, `documento.css`, `modelos/AberturaObraV1.tsx`, `modelos/index.ts`, `public/logos/` (CREA-SP, CRT-SP, CAU/SP só se o usuário fornecer; caso contrário, texto nos espaços reservados)
- Create: `src/routes/documentos.$tipo.$id.tsx`, `src/app/DocumentoPage.tsx`

**Interfaces:**
- Produces: `FolhaA4({ assinaturas, children })`, com A4 de 210×297 mm e faixa vertical por assinatura com papel, nome, hash completo, data e hora em horário de Brasília, IP e carimbo.
- Produces: `ModeloDocumento({ documento }: { documento: DocumentoResponse })`, escolhendo por `tipo` + `modeloVersao`. Versão desconhecida mostra erro claro, nunca outro modelo.
- `BlocoAssinatura`: assinado → "Assinado digitalmente" + nome; pendente → "Aguardando assinatura"; assinatura antiga de proprietário aparece com o papel dela.

- [ ] Montar a folha e o modelo da abertura, com data como "Adamantina, 8 de novembro de 2026".
- [ ] `@media print`: esconder o layout do sistema, `@page { size: A4; margin: 0 }`, botão Imprimir chama `window.print()`.
- [ ] Conferir no navegador local com uma obra real local, tela e impressão em PDF, incluindo IPv6 longo.
- [ ] **Parar e mostrar ao usuário para aprovar o visual.** Commit.

### Task 6: Modelos de relato e termo

**Files:**
- Create: `src/features/documentos/modelos/RelatoVisitaV1.tsx`, `TermoConclusaoV1.tsx`
- Modify: `modelos/index.ts`

- [ ] Relato: número, data, fases, posição, decisões; decisões longas continuam na folha seguinte com a lateral repetida; fotos em página anexa pelo endpoint `/imagens/{id}/url`, com nome e hash.
- [ ] Termo: dados, DECLARAÇÃO, parágrafo de sanções do modelo, data e assinaturas.
- [ ] Conferir em tela e impressão com texto curto, texto longo e com fotos. Commit.

### Task 7: Tela de assinatura

**Files:**
- Create: `src/features/documentos/AssinarDocumentoDialog.tsx`
- Delete: `src/features/assinatura/components/SignatureCanvas.tsx`
- Modify: `src/features/assinatura/components/AssinaturaStatusCard.tsx`, `src/features/obras/components/ObraDetalhe.tsx`, `src/features/registros/components/RegistroDetalhe.tsx`

**Interfaces:**
- Consumes: `obterDocumento`, `assinarDocumento`, `ModeloDocumento`.
- `AssinaturaSlot.papel` inclui `"Administrador"`, com `tipo` e `referenciaId` no bloco.

- [ ] Diálogo: mostra o documento com rolagem, depois CPF e senha, e o botão "Assinar documento". Em celular a folha reduz sem rolagem horizontal da página.
- [ ] Erros: senha incorreta mantém o diálogo; documento alterado recarrega o documento e pede nova leitura; bloqueio mostra o aviso de 15 minutos.
- [ ] `ObraDetalhe`: slots de Profissional e Administrador (Administrador só pode assinar depois do profissional), assinatura antiga de proprietário como slot extra quando existir. Botão "Ver / imprimir documento" para os 3 tipos.
- [ ] Textos: "Eng/Arq/Téc" no card conforme o título; `AcessoRestrito` inalterado.
- [ ] `tsc --noEmit` limpo e `bun run build` passando. Commit.

### Task 8: Verificação local completa e deploy

- [ ] Fluxo local no navegador, com contas de teste locais: criar obra → profissional assina → Administrador assina → obra Ativa; relato com foto → dois assinam; termo → dois assinam → Finalizada. Conferir hash, data, IP e impressão dos 3.
- [ ] Testar senha errada, CPF de outro usuário e 6 tentativas.
- [ ] Testar a migration e os gatilhos em uma cópia do banco de produção restaurada localmente, incluindo a leitura dos documentos antigos.
- [ ] Mostrar o resultado ao usuário e pedir OK para merge e deploy.
- [ ] Deploy: backup do banco → merge em `main` e push → `git pull` na VPS → gerar `dotnet ef migrations script --idempotent` e aplicar → aplicar gatilhos → rebuild da API → publicar `.output` com `NITRO_PRESET=node-server` → reiniciar `crea.service`.
- [ ] Verificar produção: endpoints, 7+ gatilhos, contagens iguais, leitura da obra 2. Login fica para o usuário testar.
