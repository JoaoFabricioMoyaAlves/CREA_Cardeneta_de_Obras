import { api } from "./client";
import type { ImagemResponse, RegistroResponse } from "./types";

export function listarRegistrosPorObra(obraId: number) {
  return api.get<RegistroResponse[]>(`/api/obras/${obraId}/registros`);
}

export function obterRegistro(id: number) {
  return api.get<RegistroResponse>(`/api/registros/${id}`);
}

// Mesma grafia usada em src/lib/constants.ts (FASES_SERVICO) e no
// RegistroMapper.cs do backend — os rótulos batem nos dois lados.
const CAMPO_POR_FASE: Record<string, string> = {
  "Serviços preliminares": "faseServicosPreliminares",
  "Fundação": "faseFundacao",
  "Alvenaria": "faseAlvenarias",
  "Superestrutura": "faseSuperestrutura",
  "Cobertura": "faseCobertura",
  "Esquadrias e instalações": "faseEsquadriasInst",
  "Revestimento": "faseRevestimento",
  "Pintura": "fasePintura",
  "Serviços complementares": "faseServicosComp",
};

export type CriarRegistroPayload = {
  obraId: number;
  dataVisita: string;
  posicaoObra: string;
  decisoesOrientacoes: string;
  fasesSelecionadas: string[];
};

export function criarRegistro(payload: CriarRegistroPayload) {
  const fases: Record<string, boolean> = {};
  for (const campo of Object.values(CAMPO_POR_FASE)) fases[campo] = false;
  for (const fase of payload.fasesSelecionadas) {
    const campo = CAMPO_POR_FASE[fase];
    if (campo) fases[campo] = true;
  }

  return api.post<RegistroResponse>("/api/registros", {
    obraId: payload.obraId,
    dataVisita: payload.dataVisita,
    posicaoObra: payload.posicaoObra,
    decisoesOrientacoes: payload.decisoesOrientacoes,
    ...fases,
  });
}

export function assinarRegistro(id: number) {
  return api.post<RegistroResponse>(`/api/registros/${id}/assinar`);
}

export function adicionarImagem(registroId: number, arquivo: File) {
  const formData = new FormData();
  formData.append("arquivo", arquivo);
  return api.postForm<ImagemResponse>(`/api/registros/${registroId}/imagens`, formData);
}
