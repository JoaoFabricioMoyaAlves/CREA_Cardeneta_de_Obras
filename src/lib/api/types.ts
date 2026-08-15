import type { Perfil } from "@/lib/constants";

// Espelha os DTOs do backend (.NET). Nomes de campo em camelCase porque o
// System.Text.Json do ASP.NET Core serializa assim por padrão.

export type PerfilApi = "Administrador" | "Engenheiro" | "Proprietario";

export function perfilDeApi(perfil: PerfilApi): Perfil {
  return perfil.toLowerCase() as Perfil;
}

export type UsuarioLogado = {
  id: string;
  nome: string;
  cpf: string;
  email: string;
  perfil: PerfilApi;
  tituloProfissional: string | null;
  numeroRegistro: string | null;
};

export type LoginResponse = {
  token: string;
  usuario: UsuarioLogado;
};

export type UsuarioResponse = {
  id: string;
  nome: string;
  cpf: string;
  email: string;
  telefone: string;
  perfil: PerfilApi;
  tituloProfissional: string | null;
  numeroRegistro: string | null;
};

export type UsuarioCriadoResponse = {
  usuario: UsuarioResponse;
  senhaProvisoria: string;
};

export type AssinaturaResumo = {
  papel: "Engenheiro" | "Proprietario";
  usuarioId: string;
  nomeUsuario: string;
  data: string | null;
  hash: string | null;
};

export type StatusObraApi = "PendenteAssinatura" | "Ativa" | "Finalizada";
export type StatusRegistroApi = "PendenteAssinatura" | "Assinado";

export type ObraResponse = {
  id: number;
  numeroCaderneta: string;
  localObra: string;
  cidade: string;
  numeroRt: string;
  tipoEdificacao: string;
  areaConstruirM2: number;
  areaAmpliarM2: number;
  areaReformarM2: number;
  areaRegularizarM2: number;
  areaTotalEdificadaM2: number;
  valorObra: number;
  status: StatusObraApi;
  profissionalId: string;
  nomeProfissional: string;
  proprietarioId: string;
  nomeProprietario: string;
  nomeEmpresa: string | null;
  cnpjEmpresa: string | null;
  dataReciboAbertura: string;
  atividadesTecnicas: string[];
  assinaturas: AssinaturaResumo[];
};

export type ImagemResponse = {
  id: number;
  name: string;
  storageKey: string;
  data: string;
};

export type RegistroResponse = {
  id: number;
  obraId: number;
  dataVisita: string;
  posicaoObra: string;
  decisoesOrientacoes: string;
  fases: string[];
  status: StatusRegistroApi;
  imagens: ImagemResponse[];
  assinaturas: AssinaturaResumo[];
};

export type TermoResponse = {
  id: number;
  obraId: number;
  dataConclusao: string;
  declaracao: string;
  status: StatusRegistroApi;
  assinaturas: AssinaturaResumo[];
};
