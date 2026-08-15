import { api } from "./client";
import type { TermoResponse } from "./types";

export function obterTermoPorObra(obraId: number) {
  return api.get<TermoResponse | null>(`/api/obras/${obraId}/termo-conclusao`);
}

export function criarTermo(obraId: number, dataConclusao: string, declaracao: string) {
  return api.post<TermoResponse>("/api/termos-conclusao", { obraId, dataConclusao, declaracao });
}

export function assinarTermo(id: number) {
  return api.post<TermoResponse>(`/api/termos-conclusao/${id}/assinar`);
}
