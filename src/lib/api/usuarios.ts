import { api } from "./client";
import type { TipoUsuario } from "@/lib/constants";
import type { UsuarioCriadoResponse, UsuarioResponse } from "./types";

// PerfilUsuario enum no backend: Administrador=1, Engenheiro=2, Proprietario=3.
// Arquiteto não existe como perfil próprio no backend — é um Engenheiro com
// título profissional diferente (ver seção 3.5 do ROADMAP).
const PERFIL_ENUM_POR_TIPO: Record<TipoUsuario, number> = {
  administrador: 1,
  engenheiro: 2,
  arquiteto: 2,
  proprietario: 3,
};

export function listarUsuarios() {
  return api.get<UsuarioResponse[]>("/api/usuarios");
}

export type CriarUsuarioPayload = {
  nome: string;
  cpf: string;
  email: string;
  telefone: string;
  tipoUsuario: TipoUsuario;
  tituloProfissional?: string | null;
  numeroRegistro?: string | null;
  senhaSecretaDev?: string | null;
};

export function criarUsuario(payload: CriarUsuarioPayload) {
  return api.post<UsuarioCriadoResponse>("/api/usuarios", {
    nome: payload.nome,
    cpf: payload.cpf,
    email: payload.email,
    telefone: payload.telefone,
    perfil: PERFIL_ENUM_POR_TIPO[payload.tipoUsuario],
    tituloProfissional: payload.tituloProfissional ?? null,
    numeroRegistro: payload.numeroRegistro ?? null,
    senhaSecretaDev: payload.senhaSecretaDev ?? null,
  });
}
