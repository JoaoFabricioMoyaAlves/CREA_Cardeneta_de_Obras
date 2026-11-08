export const CORES = {
  azulPrimario: "#1E3A8A",
  azulSecundario: "#2563EB",
  azulClaro: "#DBEAFE",
  ambar: "#F59E0B",
  ambarClaro: "#FEF3C7",
  branco: "#FFFFFF",
  texto: "#1F2937",
} as const;

export type Perfil = "administrador" | "engenheiro" | "proprietario";

// Tipo de usuário exibido no CADASTRO (mais granular que o Perfil de acesso/RBAC
// acima). Engenheiro, Arquiteto e Técnico têm as mesmas permissões no sistema —
// os três mapeiam para o perfil "engenheiro" — mas são escolhidos como opções
// separadas aqui porque exigem título profissional e registro diferentes
// (CREA, CAU e CFT).
export type TipoUsuario = "administrador" | "engenheiro" | "arquiteto" | "tecnico" | "proprietario";

export const TIPOS_USUARIO: { value: TipoUsuario; label: string }[] = [
  { value: "administrador", label: "Administrador (CREA)" },
  { value: "engenheiro", label: "Engenheiro" },
  { value: "arquiteto", label: "Arquiteto" },
  { value: "tecnico", label: "Técnico" },
  { value: "proprietario", label: "Proprietário" },
];

// Títulos fixos gravados no cadastro de quem não escolhe especialidade.
export const TITULO_ARQUITETO = "Arquiteto e Urbanista";
export const TITULO_TECNICO = "Técnico";

// Valores iniciais dos campos de localização; o usuário pode alterar.
export const CIDADE_PADRAO = "Adamantina";
export const ESTADO_PADRAO = "SP";

export const ESPECIALIDADES_ENGENHARIA = [
  "Engenheiro Civil",
  "Engenheiro Eletricista",
  "Engenheiro Mecânico",
  "Engenheiro Ambiental",
  "Outro",
] as const;

export const ESPECIALIDADES_ARQUITETURA = [
  "Arquitetura e Urbanismo",
  "Paisagismo",
  "Design de Interiores",
  "Restauro e Patrimônio Histórico",
  "Outro",
] as const;

// Perfil de acesso (RBAC) não distingue Engenheiro, Arquiteto e Técnico — os três
// são PerfilUsuario.Engenheiro no backend. A abreviação exibida ao usuário usa o
// tituloProfissional só para escolher entre "Eng.", "Arq." e "Téc.".
type PerfilComoTexto = "Administrador" | "Engenheiro" | "Proprietario";

export function abreviarPerfil(perfil: PerfilComoTexto, tituloProfissional?: string | null): string {
  if (perfil === "Administrador") return "Admin.";
  if (perfil === "Proprietario") return "Prop.";
  if (tituloProfissional === TITULO_TECNICO) return "Téc.";
  const ehArquiteto =
    tituloProfissional === TITULO_ARQUITETO ||
    (ESPECIALIDADES_ARQUITETURA as readonly string[]).includes(tituloProfissional ?? "");
  return ehArquiteto ? "Arq." : "Eng.";
}

export function nomeComAbreviacao(
  nome: string,
  perfil: PerfilComoTexto,
  tituloProfissional?: string | null,
): string {
  return `${abreviarPerfil(perfil, tituloProfissional)} ${nome}`;
}

export const FASES_SERVICO = [
  "Serviços preliminares",
  "Fundação",
  "Alvenaria",
  "Superestrutura",
  "Cobertura",
  "Esquadrias e instalações",
  "Revestimento",
  "Pintura",
  "Serviços complementares",
] as const;

export const ATIVIDADES_TECNICAS = [
  "Direção",
  "Execução",
  "Fiscalização",
  "Projeto",
] as const;

export const POSICOES_OBRA = [
  "De acordo com o projeto",
  "Em desacordo com o projeto",
  "Em andamento",
  "Paralisada",
] as const;

export const TIPOS_EDIFICACAO = [
  "Residencial unifamiliar",
  "Residencial multifamiliar",
  "Comercial",
  "Industrial",
  "Institucional",
] as const;
