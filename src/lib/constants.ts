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
