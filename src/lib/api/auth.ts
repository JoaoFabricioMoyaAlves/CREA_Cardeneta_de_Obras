import { api } from "./client";
import type { LoginResponse } from "./types";

export function login(cpf: string, senha: string) {
  return api.post<LoginResponse>("/api/auth/login", { cpf, senha });
}

export function solicitarRecuperacaoSenha(email: string) {
  return api.post<null>("/api/auth/recuperar-senha", { email });
}

export function redefinirSenha(token: string, novaSenha: string) {
  return api.post<null>("/api/auth/redefinir-senha", { token, novaSenha });
}
