import type { DadosAbertura, DocumentoResponse } from "@/lib/api/documentos";
import { formatarMoeda } from "@/lib/format";
import {
  BlocoAssinaturasDocumento,
  Cabecalho,
  Campo,
  dataPorExtenso,
  FolhaA4,
  Opcao,
} from "../FolhaA4";

// Modelo publicado: não alterar. Mudança visual vira AberturaObraV2. As listas
// ficam aqui, e não nas constantes do sistema, para o documento não mudar junto.
const TIPOS = [
  "Residencial unifamiliar",
  "Residencial multifamiliar",
  "Comercial",
  "Industrial",
  "Institucional",
];
const ATIVIDADES = ["Direção", "Execução", "Fiscalização", "Projeto"];

function area(valor: number) {
  return `${valor.toLocaleString("pt-BR", { minimumFractionDigits: 2 })} m²`;
}

export function AberturaObraV1({ documento }: { documento: DocumentoResponse }) {
  const d = documento.dados as DadosAbertura;
  const tipoNaLista = TIPOS.includes(d.tipoEdificacao);

  return (
    <FolhaA4 documento={documento}>
      <div className="doc-moldura">
        <Cabecalho numero={d.numeroCaderneta} />
        <div className="doc-titulo-linha">
          <h1 className="doc-titulo">RECIBO</h1>
          <strong>R$: {formatarMoeda(d.valorObra).replace("R$", "").trim()}</strong>
        </div>

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
        <div className="doc-linha">
          <Campo rotulo="Área a Construir:" valor={area(d.areaConstruirM2)} />
          <Campo rotulo="Área a Ampliar:" valor={area(d.areaAmpliarM2)} />
        </div>
        <div className="doc-linha">
          <Campo rotulo="Área a Reformar:" valor={area(d.areaReformarM2)} />
          <Campo rotulo="Área a Regularizar:" valor={area(d.areaRegularizarM2)} />
        </div>
        <div className="doc-linha">
          <Campo rotulo="Área Total Edificada:" valor={area(d.areaTotalEdificadaM2)} />
        </div>

        <p className="doc-subtitulo">Tipo de Edificação:</p>
        <div className="doc-opcoes">
          {TIPOS.map((tipo) => (
            <Opcao key={tipo} rotulo={tipo} marcada={d.tipoEdificacao === tipo} />
          ))}
        </div>
        <div className="doc-linha" style={{ marginTop: "2mm" }}>
          <Campo
            rotulo="Outros:"
            valor={d.tipoEdificacaoOutros ?? (tipoNaLista ? "" : d.tipoEdificacao)}
          />
        </div>

        <p className="doc-subtitulo">Atividade Técnica:</p>
        <div className="doc-opcoes">
          {ATIVIDADES.map((atividade) => (
            <Opcao
              key={atividade}
              rotulo={atividade}
              marcada={d.atividadesTecnicas.includes(atividade)}
            />
          ))}
        </div>

        <div className="doc-linha" style={{ marginTop: "4mm" }}>
          <Campo rotulo="R.T. Nº:" valor={d.numeroRt} />
        </div>

        <p className="doc-data">
          {d.cidade}, {dataPorExtenso(d.dataRecibo)}.
        </p>

        <BlocoAssinaturasDocumento
          documento={documento}
          espacos={[
            { papel: "Engenheiro", rotulo: "Assinatura do Profissional", nome: d.profissional.nome },
            { papel: "Administrador", rotulo: "Assinatura do CREA (Adamantina)", nome: "" },
          ]}
        />
      </div>
    </FolhaA4>
  );
}
