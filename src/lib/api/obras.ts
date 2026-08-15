import { api } from "./client";
import type { ObraResponse } from "./types";

export function listarObras() {
  return api.get<ObraResponse[]>("/api/obras");
}

export function obterObra(id: number) {
  return api.get<ObraResponse>(`/api/obras/${id}`);
}

export type CriarObraPayload = {
  proprietarioId: string;
  profissionalId: string;
  localObra: string;
  cidade: string;
  numeroRt: string;
  areaConstruirM2: number;
  areaAmpliarM2: number;
  areaReformarM2: number;
  areaRegularizarM2: number;
  tipoEdificacao: string;
  tipoEdificacaoOutros: string | null;
  ativTecnicaDirecao: boolean;
  ativTecnicaExecucao: boolean;
  ativTecnicaFiscalizacao: boolean;
  ativTecnicaProjeto: boolean;
  valorObra: number;
  dataReciboAbertura: string;
  nomeEmpresa: string | null;
  cnpjEmpresa: string | null;
};

export function criarObra(payload: CriarObraPayload) {
  return api.post<ObraResponse>("/api/obras", payload);
}

export function assinarObra(id: number) {
  return api.post<ObraResponse>(`/api/obras/${id}/assinar`);
}
