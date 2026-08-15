import { api } from "./client";
import type { LogAuditoriaResponse } from "./types";

export function listarAuditoria(limite = 200) {
  return api.get<LogAuditoriaResponse[]>(`/api/auditoria?limite=${limite}`);
}
