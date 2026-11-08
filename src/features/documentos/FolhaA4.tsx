import type { ReactNode } from "react";
import type { AssinaturaDocumento, DocumentoResponse } from "@/lib/api/documentos";
import "./documento.css";

const NOME_PAPEL: Record<AssinaturaDocumento["papel"], string> = {
  Engenheiro: "Profissional",
  Proprietario: "Proprietário",
  Administrador: "Administrador (CREA)",
};

export function formatarDataHoraBrasilia(iso: string): string {
  return new Date(iso).toLocaleString("pt-BR", {
    timeZone: "America/Sao_Paulo",
    dateStyle: "short",
    timeStyle: "medium",
  });
}

const MESES = [
  "janeiro",
  "fevereiro",
  "março",
  "abril",
  "maio",
  "junho",
  "julho",
  "agosto",
  "setembro",
  "outubro",
  "novembro",
  "dezembro",
];

// Datas de documento são só dia (AAAA-MM-DD): sem converter fuso, para não
// correr o risco de voltar um dia.
export function dataPorExtenso(iso: string): string {
  const [ano, mes, dia] = iso.split("-").map(Number);
  return `${dia} de ${MESES[(mes ?? 1) - 1]} de ${ano}`;
}

export function dataCurta(iso: string): string {
  const [ano, mes, dia] = iso.split("-");
  return `${dia}/${mes}/${ano}`;
}

export function FolhaA4({
  documento,
  children,
}: {
  documento: DocumentoResponse;
  children: ReactNode;
}) {
  return (
    <div className="doc-area">
      <article className="doc-folha">
        <aside className="doc-lateral" aria-label="Identificação das assinaturas">
          {documento.assinaturas.map((a) => (
            <div key={`${a.papel}-${a.hash}`} className="doc-lateral-item">
              <strong>
                Assinado digitalmente por {a.nome} ({NOME_PAPEL[a.papel]})
              </strong>
              <br />
              {formatarDataHoraBrasilia(a.data)} (Brasília) · IP {a.ip}
              {a.carimboVerificado && a.carimboAutoridade
                ? ` · Carimbo de tempo: ${a.carimboAutoridade}`
                : ""}
              <br />
              Hash: {a.hash}
            </div>
          ))}
        </aside>
        {children}
        <footer className="doc-rodape">
          Hash do documento: {documento.hashDocumento}
          {documento.anteriorAoModelo &&
            " · Documento anterior ao novo modelo: exibido com os dados atuais do cadastro."}
        </footer>
      </article>
    </div>
  );
}

// Larguras definidas à mão para as logos terem peso visual parecido na folha;
// a altura acompanha a proporção de cada arquivo.
const LOGOS = [
  { arquivo: "crea-sp.png", nome: "CREA-SP", largura: "38mm" },
  { arquivo: "crt-sp.png", nome: "CRT-SP", largura: "36mm" },
  { arquivo: "cau-sp.png", nome: "CAU/SP", largura: "33mm" },
  {
    arquivo: "prefeitura-adamantina.png",
    nome: "Prefeitura Municipal de Adamantina",
    largura: "32mm",
  },
];

export function Cabecalho({ numero }: { numero: string }) {
  return (
    <header className="doc-cabecalho">
      <div className="doc-logos">
        {LOGOS.map((logo) => (
          <img
            key={logo.arquivo}
            src={`/logos/${logo.arquivo}`}
            alt={logo.nome}
            className="doc-logo"
            style={{ width: logo.largura }}
          />
        ))}
      </div>
      <p className="doc-numero">{numero}</p>
    </header>
  );
}

export function Campo({ rotulo, valor }: { rotulo: string; valor?: string | number | null }) {
  return (
    <span className="doc-campo">
      <span className="doc-campo-rotulo">{rotulo}</span>
      <span className="doc-campo-valor">{valor ?? ""}</span>
    </span>
  );
}

export function Opcao({ rotulo, marcada }: { rotulo: string; marcada: boolean }) {
  return (
    <span className="doc-opcao">
      <span className="doc-caixa">{marcada ? "X" : ""}</span>
      {rotulo}
    </span>
  );
}

export type EspacoAssinatura = {
  papel: AssinaturaDocumento["papel"];
  rotulo: string;
  nome: string;
};

// Assinatura antiga de um papel que o modelo não prevê (proprietário em
// abertura antiga) também aparece: nada do que foi assinado pode sumir.
export function BlocoAssinaturasDocumento({
  documento,
  espacos,
}: {
  documento: DocumentoResponse;
  espacos: EspacoAssinatura[];
}) {
  const extras: EspacoAssinatura[] = documento.assinaturas
    .filter((a) => !espacos.some((e) => e.papel === a.papel))
    .map((a) => ({ papel: a.papel, rotulo: `Assinatura — ${NOME_PAPEL[a.papel]}`, nome: a.nome }));

  return (
    <div className="doc-assinaturas">
      {[...espacos, ...extras].map((espaco) => {
        const assinatura = documento.assinaturas.find((a) => a.papel === espaco.papel);
        return (
          <div key={espaco.papel} className="doc-assinatura">
            <div className="doc-assinatura-marca">
              {assinatura ? (
                <>
                  <strong>Assinado digitalmente</strong>
                  <span>{assinatura.nome}</span>
                </>
              ) : (
                <span className="doc-assinatura-pendente">Aguardando assinatura</span>
              )}
            </div>
            <div className="doc-assinatura-rotulo">{espaco.rotulo}</div>
          </div>
        );
      })}
    </div>
  );
}
