# Documentos A4 e nova assinatura eletrônica — Design

## Por quê
O pessoal do CREA de Adamantina pediu que a assinatura deixe de ser um desenho na
tela: quem assina deve ler o documento e confirmar a própria identidade. A
abertura da caderneta passa a ser validada por um funcionário do CREA, e não mais
pelo proprietário. Os documentos não devem ocupar armazenamento como PDF.

## Objetivos
1. Três documentos em HTML/CSS no tamanho A4: abertura (recibo), relato de visita e
   termo de conclusão. Os PDFs de Penápolis são apenas referência visual.
2. Nada de PDF guardado: uma tabela guarda os dados do documento, e o modelo é
   preenchido ao visualizar ou imprimir.
3. Assinar = ver o documento na tela + confirmar CPF e senha. O desenho na tela sai.
4. No documento: "Assinado digitalmente" no campo de assinatura e, na lateral da
   folha, hash da assinatura, data, hora e IP.
5. Abertura assinada por Eng/Arq/Téc e depois por um Administrador. Relato e termo
   continuam com Eng/Arq/Téc e Proprietário.
6. Cidade com padrão Adamantina e novo campo Estado com padrão SP, ambos editáveis.
7. Gatilhos de imutabilidade ativos também em produção.

## Fora do escopo
Certificado ICP-Brasil, armazenar PDFs, alterar assinaturas antigas, recusa da
abertura pelo Administrador (fica para uma próxima etapa) e remover o QuestPDF.

## Decisões do usuário
- Logos: CREA-SP, CRT-SP e CAU/SP, com espaço reservado para as entidades de
  Adamantina até as logos serem enviadas.
- Qualquer Administrador pode assinar a abertura, sempre depois do Eng/Arq/Téc, e
  todos os Administradores recebem o aviso.
- Fotos do relato em página anexa, com o hash de cada imagem no conteúdo assinado.
- A4 em pé, com a faixa de hash na vertical. O IP fica visível a quem abrir o documento.

## Regras
- **Congelamento:** os dados são congelados na primeira assinatura pelo novo fluxo.
  A partir daí o documento não muda, mesmo que cadastros mudem.
- **O que foi visto é o que foi assinado:** a assinatura envia o hash do documento
  exibido. Se for diferente do atual, o servidor recusa e pede para rever.
- **Versão do modelo:** cada documento guarda a versão usada. Modelo publicado não é
  alterado; mudança visual vira uma nova versão.
- **Documentos antigos:** assinaturas existentes não são alteradas. Documento com
  assinatura e sem dados congelados é montado com os dados atuais e identificado como
  anterior ao novo modelo.
- **Abertura pendente:** obra que só tem assinatura do profissional passa a aguardar
  o Administrador. Assinatura antiga de proprietário continua aparecendo no documento.
- **Senha errada:** não desconecta o usuário. Após 5 erros em 15 minutos, a assinatura
  fica bloqueada para aquele usuário, e cada falha vai para a auditoria.
- **Carimbo de tempo:** continua igual e não bloqueia a assinatura se a autoridade
  externa não responder.
