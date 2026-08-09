# Obra Conectada

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

This project was built with [Lovable](https://lovable.dev).

## Build with Lovable

Continue developing this project in the [Lovable editor](https://lovable.dev/projects/f1d4a4c7-4bf7-4028-b7dc-21c01baf1fec).

- **Ship faster**: describe what you want to build and Lovable handles the code.
- **Stay in sync**: every change made in Lovable is committed straight to this repository.
- **Full ownership**: this code is yours. Push to `main` on GitHub and your changes sync back into Lovable, ready for your next prompt.

## Development

Prefer working locally? You need Node.js and npm — [install with nvm](https://github.com/nvm-sh/nvm#installing-and-updating).

```sh
git clone <this-repository-url>
cd <repository-name>
npm i
npm run dev
```
