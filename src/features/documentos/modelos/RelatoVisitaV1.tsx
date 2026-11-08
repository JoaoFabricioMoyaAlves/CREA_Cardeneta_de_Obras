import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import type { DadosRelato, DocumentoResponse, FotoDocumento } from "@/lib/api/documentos";
import { BlocoAssinaturasDocumento, Cabecalho, Campo, dataCurta, FolhaA4, Opcao } from "../FolhaA4";

// Modelo publicado: não alterar. Mudança visual vira RelatoVisitaV2.
const FASES = [
  "Serviços preliminares",
  "Fundação",
  "Alvenaria",
  "Superestrutura",
  "Cobertura",
  "Esquadrias e instalações",
  "Revestimento",
  "Pintura",
  "Serviços complementares",
];
const POSICOES = [
  "De acordo com o projeto",
  "Em desacordo com o projeto",
  "Em andamento",
  "Paralisada",
];

function FotoAnexa({ relatoId, foto }: { relatoId: number; foto: FotoDocumento }) {
  const query = useQuery({
    queryKey: ["documento-foto", relatoId, foto.id],
    queryFn: () => api.get<{ url: string }>(`/api/registros/${relatoId}/imagens/${foto.id}/url`),
    retry: false,
  });

  return (
    <figure className="doc-foto">
      {query.data && <img src={query.data.url} alt={foto.nome} />}
      {query.isError && <p>Não foi possível carregar a imagem.</p>}
      <figcaption>
        {foto.nome}
        {foto.sha256 && (
          <>
            <br />
            SHA-256: {foto.sha256}
          </>
        )}
      </figcaption>
    </figure>
  );
}

export function RelatoVisitaV1({ documento }: { documento: DocumentoResponse }) {
  const d = documento.dados as DadosRelato;
  const posicaoNaLista = POSICOES.includes(d.posicaoObra);

  return (
    <FolhaA4 documento={documento}>
      <div className="doc-moldura">
        <Cabecalho numero={d.numeroCaderneta} />
        <div className="doc-titulo-linha">
          <h1 className="doc-titulo">RELATO DE VISITA</h1>
          <span className="doc-numero">{String(d.sequencia).padStart(3, "0")}</span>
        </div>

        <div className="doc-linha">
          <Campo rotulo="Data:" valor={dataCurta(d.dataVisita)} />
          <Campo rotulo="Cidade:" valor={`${d.cidade} / ${d.estado}`} />
        </div>
        <div className="doc-linha">
          <Campo rotulo="Local da Obra:" valor={d.localObra} />
        </div>

        <p className="doc-subtitulo">Fases de serviço:</p>
        <div className="doc-opcoes">
          {FASES.map((fase) => (
            <Opcao key={fase} rotulo={fase} marcada={d.fases.includes(fase)} />
          ))}
        </div>

        <p className="doc-subtitulo">POSIÇÃO DA OBRA</p>
        <div className="doc-opcoes">
          {POSICOES.map((posicao) => (
            <Opcao key={posicao} rotulo={posicao} marcada={d.posicaoObra === posicao} />
          ))}
          {!posicaoNaLista && <Opcao rotulo={d.posicaoObra} marcada />}
        </div>

        <p className="doc-subtitulo">Decisões técnicas e orientações executivas:</p>
        <div className="doc-texto-livre">{d.decisoesOrientacoes}</div>

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
              rotulo: "Assinatura do Proprietário ou Construtor",
              nome: d.proprietario.nome,
            },
          ]}
        />
      </div>

      {d.fotos.length > 0 && (
        <section className="doc-fotos">
          <p className="doc-subtitulo">Anexo: fotos do relato de visita</p>
          {d.fotos.map((foto) => (
            <FotoAnexa key={foto.id} relatoId={documento.referenciaId} foto={foto} />
          ))}
        </section>
      )}
    </FolhaA4>
  );
}
