import type { DadosTermo, DocumentoResponse } from "@/lib/api/documentos";
import { BlocoAssinaturasDocumento, Cabecalho, Campo, dataPorExtenso, FolhaA4 } from "../FolhaA4";

// Modelo publicado: não alterar. Mudança visual vira TermoConclusaoV2.
export function TermoConclusaoV1({ documento }: { documento: DocumentoResponse }) {
  const d = documento.dados as DadosTermo;

  return (
    <FolhaA4 documento={documento}>
      <div className="doc-moldura">
        <Cabecalho numero={d.numeroCaderneta} />
        <h1 className="doc-titulo">TERMO DE CONCLUSÃO</h1>

        <div className="doc-linha">
          <Campo rotulo="Profissional:" valor={d.profissional.nome} />
          <Campo rotulo="Nº de Registro" valor={d.profissional.registro} />
        </div>
        <div className="doc-linha">
          <Campo rotulo="Empresa:" valor={d.empresa.nome} />
          <Campo rotulo="CNPJ" valor={d.empresa.cnpj} />
        </div>
        <div className="doc-linha">
          <Campo rotulo="Proprietário:" valor={d.proprietario.nome} />
          <Campo rotulo="CPF nº" valor={d.proprietario.cpf} />
        </div>
        <div className="doc-linha">
          <Campo rotulo="Telefone do Proprietário:" valor={d.proprietario.telefone} />
        </div>
        <div className="doc-linha">
          <Campo rotulo="Local da Obra:" valor={d.localObra} />
        </div>
        <div className="doc-linha">
          <Campo rotulo="Cidade:" valor={d.cidade} />
          <Campo rotulo="Estado:" valor={d.estado} />
        </div>

        <h2 className="doc-titulo" style={{ fontSize: "14pt" }}>
          DECLARAÇÃO
        </h2>
        <p style={{ textAlign: "justify", textIndent: "10mm" }}>
          Nós, abaixo assinados, proprietário e profissional responsável, pela execução da obra
          acima apontada declaramos que
        </p>
        <div className="doc-texto-livre" style={{ margin: "2mm 0" }}>
          {d.declaracao}
        </div>
        <p style={{ textAlign: "justify" }}>
          Temos ainda conhecimento na íntegra das sanções prescritas nas Legislações Federal,
          Estadual e Municipal vigentes.
        </p>

        <p className="doc-data">
          {d.cidade}, {dataPorExtenso(d.dataConclusao)}.
        </p>

        <BlocoAssinaturasDocumento
          documento={documento}
          espacos={[
            {
              papel: "Engenheiro",
              rotulo: "Assinatura do Profissional",
              nome: d.profissional.nome,
            },
            {
              papel: "Proprietario",
              rotulo: "Assinatura do Proprietário",
              nome: d.proprietario.nome,
            },
          ]}
        />
      </div>
    </FolhaA4>
  );
}
