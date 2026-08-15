import { api } from "./client";
import type { LoginResponse } from "./types";

export function login(cpf: string, senha: string) {
  return api.post<LoginResponse>("/api/auth/login", { cpf, senha });
}
