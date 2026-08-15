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

export const PERFIS: { value: Perfil; label: string }[] = [
  { value: "administrador", label: "Administrador (CREA)" },
  { value: "engenheiro", label: "Engenheiro/Arquiteto" },
  { value: "proprietario", label: "Proprietário" },
];

// Tipo de usuário exibido no CADASTRO (mais granular que o Perfil de acesso/RBAC
// acima). Engenheiro e Arquiteto têm as mesmas permissões no sistema — ambos
// mapeiam para o perfil "engenheiro" — mas são escolhidos como opções
// separadas aqui porque exigem título profissional e registro diferentes.
export type TipoUsuario = "administrador" | "engenheiro" | "arquiteto" | "proprietario";

export const TIPOS_USUARIO: { value: TipoUsuario; label: string }[] = [
  { value: "administrador", label: "Administrador (CREA)" },
  { value: "engenheiro", label: "Engenheiro" },
  { value: "arquiteto", label: "Arquiteto" },
  { value: "proprietario", label: "Proprietário" },
];

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
  "Início de obra",
  "Em andamento — até 25%",
  "Em andamento — até 50%",
  "Em andamento — até 75%",
  "Acabamento final",
  "Obra concluída",
] as const;

export const TIPOS_EDIFICACAO = [
  "Residencial unifamiliar",
  "Residencial multifamiliar",
  "Comercial",
  "Industrial",
  "Institucional",
] as const;

export type StatusObra = "Pendente assinatura" | "Ativa" | "Finalizada";
