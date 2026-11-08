import type { ComponentType } from "react";
import type { DocumentoResponse, TipoDocumentoApi } from "@/lib/api/documentos";
import { AberturaObraV1 } from "./AberturaObraV1";
import { RelatoVisitaV1 } from "./RelatoVisitaV1";
import { TermoConclusaoV1 } from "./TermoConclusaoV1";

type Modelo = ComponentType<{ documento: DocumentoResponse }>;

// Cada documento abre sempre no modelo com que foi congelado.
const MODELOS: Record<DocumentoResponse["tipo"], Record<number, Modelo>> = {
  AberturaObra: { 1: AberturaObraV1 },
  RelatoVisita: { 1: RelatoVisitaV1 },
  TermoConclusao: { 1: TermoConclusaoV1 },
};

export function ModeloDocumento({ documento }: { documento: DocumentoResponse }) {
  const Modelo = MODELOS[documento.tipo][documento.modeloVersao];

  if (!Modelo) {
    return (
      <p className="p-6 text-sm text-destructive">
        O modelo {documento.modeloVersao} deste documento não está disponível nesta versão do
        sistema.
      </p>
    );
  }

  return <Modelo documento={documento} />;
}

export function ehTipoDocumento(valor: string): valor is TipoDocumentoApi {
  return valor === "abertura-obra" || valor === "relato-visita" || valor === "termo-conclusao";
}
