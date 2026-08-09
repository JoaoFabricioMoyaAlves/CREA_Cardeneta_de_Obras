import type { Perfil, StatusObra } from "./constants";

export type Usuario = {
  id: string;
  nome: string;
  documento: string;
  perfil: Perfil;
  registroCrea?: string;
};

export const usuarios: Usuario[] = [
  {
    id: "u1",
    nome: "Marina Alves Peixoto",
    documento: "123.456.789-00",
    perfil: "administrador",
  },
  {
    id: "u2",
    nome: "Eng. Rafael Monteiro",
    documento: "987.654.321-00",
    perfil: "engenheiro",
    registroCrea: "CREA-SP 5063214578",
  },
  {
    id: "u3",
    nome: "Arq. Helena Duarte",
    documento: "456.123.789-00",
    perfil: "engenheiro",
    registroCrea: "CAU A118924-7",
  },
  {
    id: "u4",
    nome: "Carlos Eduardo Ramos",
    documento: "321.654.987-00",
    perfil: "proprietario",
  },
  {
    id: "u5",
    nome: "Beatriz Nogueira Lima",
    documento: "741.852.963-00",
    perfil: "proprietario",
  },
];

export const usuarioAtualPorPerfil: Record<Perfil, Usuario> = {
  administrador: usuarios[0]!,
  engenheiro: usuarios[1]!,
  proprietario: usuarios[3]!,
};

export type Assinatura = {
  papel: "Engenheiro" | "Proprietário";
  nome: string;
  assinadoEm: string | null;
  hash: string | null;
};

export type Registro = {
  id: string;
  obraId: string;
  data: string;
  posicao: string;
  decisoes: string;
  fases: string[];
  fotos: string[];
  status: "Assinado" | "Pendente assinatura";
  assinaturas: Assinatura[];
};

export type Obra = {
  id: string;
  numero: string;
  local: string;
  cidade: string;
  numeroRT: string;
  tipoEdificacao: string;
  status: StatusObra;
  engenheiroId: string;
  proprietarioId: string;
  empresa?: string;
  atividades: string[];
  dataRecibo: string;
  areas: {
    construir: number;
    ampliar: number;
    reformar: number;
    regularizar: number;
  };
  assinaturas: Assinatura[];
};

export const obras: Obra[] = [
  {
    id: "1",
    numero: "CAD-2026-0148",
    local: "Rua das Acácias, 412 — Jardim Paulista",
    cidade: "São Paulo / SP",
    numeroRT: "RT 88.412/2026",
    tipoEdificacao: "Residencial unifamiliar",
    status: "Ativa",
    engenheiroId: "u2",
    proprietarioId: "u4",
    empresa: "Monteiro Engenharia LTDA — 22.145.876/0001-09",
    atividades: ["Direção", "Execução"],
    dataRecibo: "2026-02-10",
    areas: { construir: 184.5, ampliar: 32, reformar: 0, regularizar: 0 },
    assinaturas: [
      {
        papel: "Engenheiro",
        nome: "Eng. Rafael Monteiro",
        assinadoEm: "2026-02-10 09:12",
        hash: "a1b2c3d4e5f6f9e8",
      },
      {
        papel: "Proprietário",
        nome: "Carlos Eduardo Ramos",
        assinadoEm: "2026-02-10 14:40",
        hash: "9f8e7d6c5b4a3210",
      },
    ],
  },
  {
    id: "2",
    numero: "CAD-2026-0163",
    local: "Av. Bandeirantes, 2.075 — Centro",
    cidade: "Campinas / SP",
    numeroRT: "RT 91.007/2026",
    tipoEdificacao: "Comercial",
    status: "Pendente assinatura",
    engenheiroId: "u2",
    proprietarioId: "u5",
    atividades: ["Fiscalização", "Projeto"],
    dataRecibo: "2026-07-28",
    areas: { construir: 640, ampliar: 0, reformar: 120, regularizar: 45 },
    assinaturas: [
      {
        papel: "Engenheiro",
        nome: "Eng. Rafael Monteiro",
        assinadoEm: "2026-07-28 11:03",
        hash: "77aa55bb33cc11dd",
      },
      { papel: "Proprietário", nome: "Beatriz Nogueira Lima", assinadoEm: null, hash: null },
    ],
  },
  {
    id: "3",
    numero: "CAD-2025-0912",
    local: "Estrada do Mirante, km 4 — Zona Rural",
    cidade: "Atibaia / SP",
    numeroRT: "RT 74.559/2025",
    tipoEdificacao: "Residencial multifamiliar",
    status: "Finalizada",
    engenheiroId: "u3",
    proprietarioId: "u4",
    empresa: "Duarte Projetos — 41.998.220/0001-55",
    atividades: ["Projeto"],
    dataRecibo: "2025-05-04",
    areas: { construir: 1240, ampliar: 0, reformar: 0, regularizar: 0 },
    assinaturas: [
      {
        papel: "Engenheiro",
        nome: "Arq. Helena Duarte",
        assinadoEm: "2025-05-04 08:30",
        hash: "c0ffee1234ab56cd",
      },
      {
        papel: "Proprietário",
        nome: "Carlos Eduardo Ramos",
        assinadoEm: "2025-05-05 17:22",
        hash: "beef4242deadc0de",
      },
    ],
  },
  {
    id: "4",
    numero: "CAD-2026-0201",
    local: "Rua Coronel Bento, 89 — Vila Industrial",
    cidade: "Sorocaba / SP",
    numeroRT: "RT 93.310/2026",
    tipoEdificacao: "Industrial",
    status: "Ativa",
    engenheiroId: "u3",
    proprietarioId: "u5",
    atividades: ["Direção", "Fiscalização"],
    dataRecibo: "2026-06-15",
    areas: { construir: 2100, ampliar: 350, reformar: 0, regularizar: 0 },
    assinaturas: [
      {
        papel: "Engenheiro",
        nome: "Arq. Helena Duarte",
        assinadoEm: "2026-06-15 10:00",
        hash: "1a2b3c4d5e6f7a8b",
      },
      {
        papel: "Proprietário",
        nome: "Beatriz Nogueira Lima",
        assinadoEm: "2026-06-15 16:45",
        hash: "8b7a6f5e4d3c2b1a",
      },
    ],
  },
];

const fotoPlaceholders = [
  "https://images.unsplash.com/photo-1503387762-592deb58ef4e?w=600&q=70",
  "https://images.unsplash.com/photo-1541888946425-d81bb19240f5?w=600&q=70",
  "https://images.unsplash.com/photo-1504307651254-35680f356dfd?w=600&q=70",
  "https://images.unsplash.com/photo-1523413651479-597eb2da0ad6?w=600&q=70",
];

export const registros: Registro[] = [
  {
    id: "r1",
    obraId: "1",
    data: "2026-08-04",
    posicao: "Em andamento — até 50%",
    decisoes:
      "Verificada a execução da alvenaria do pavimento superior. Orientado o encarregado quanto ao alinhamento das vergas e contravergas. Autorizada a concretagem da laje para a próxima semana, condicionada à conferência da armadura.",
    fases: ["Serviços preliminares", "Fundação", "Alvenaria", "Superestrutura"],
    fotos: fotoPlaceholders.slice(0, 3),
    status: "Assinado",
    assinaturas: [
      {
        papel: "Engenheiro",
        nome: "Eng. Rafael Monteiro",
        assinadoEm: "2026-08-04 15:20",
        hash: "a1b2c3d4e5f6f9e8",
      },
      {
        papel: "Proprietário",
        nome: "Carlos Eduardo Ramos",
        assinadoEm: "2026-08-04 18:02",
        hash: "5f4e3d2c1b0a9988",
      },
    ],
  },
  {
    id: "r2",
    obraId: "1",
    data: "2026-07-18",
    posicao: "Em andamento — até 25%",
    decisoes:
      "Conferência das sapatas e vigas baldrame. Solicitada correção do recobrimento em dois pontos da viga V3. Aprovado o prosseguimento após ajuste.",
    fases: ["Serviços preliminares", "Fundação"],
    fotos: fotoPlaceholders.slice(1, 4),
    status: "Pendente assinatura",
    assinaturas: [
      {
        papel: "Engenheiro",
        nome: "Eng. Rafael Monteiro",
        assinadoEm: "2026-07-18 11:40",
        hash: "0099aabbccddeeff",
      },
      { papel: "Proprietário", nome: "Carlos Eduardo Ramos", assinadoEm: null, hash: null },
    ],
  },
  {
    id: "r3",
    obraId: "2",
    data: "2026-08-01",
    posicao: "Início de obra",
    decisoes:
      "Instalação do canteiro de obras e conferência do gabarito. Orientações sobre sinalização e EPIs para a equipe.",
    fases: ["Serviços preliminares"],
    fotos: fotoPlaceholders.slice(0, 2),
    status: "Pendente assinatura",
    assinaturas: [
      {
        papel: "Engenheiro",
        nome: "Eng. Rafael Monteiro",
        assinadoEm: "2026-08-01 09:05",
        hash: "abcdef0123456789",
      },
      { papel: "Proprietário", nome: "Beatriz Nogueira Lima", assinadoEm: null, hash: null },
    ],
  },
  {
    id: "r4",
    obraId: "3",
    data: "2025-11-22",
    posicao: "Obra concluída",
    decisoes:
      "Vistoria final. Todos os serviços concluídos conforme projeto aprovado. Emitido termo de conclusão da caderneta.",
    fases: [
      "Serviços preliminares",
      "Fundação",
      "Alvenaria",
      "Superestrutura",
      "Cobertura",
      "Esquadrias e instalações",
      "Revestimento",
      "Pintura",
      "Serviços complementares",
    ],
    fotos: fotoPlaceholders,
    status: "Assinado",
    assinaturas: [
      {
        papel: "Engenheiro",
        nome: "Arq. Helena Duarte",
        assinadoEm: "2025-11-22 16:10",
        hash: "c0ffee1234ab56cd",
      },
      {
        papel: "Proprietário",
        nome: "Carlos Eduardo Ramos",
        assinadoEm: "2025-11-23 09:31",
        hash: "beef4242deadc0de",
      },
    ],
  },
  {
    id: "r5",
    obraId: "4",
    data: "2026-08-06",
    posicao: "Em andamento — até 75%",
    decisoes:
      "Cobertura metálica em execução. Orientada a revisão dos parafusos de fixação das terças. Liberado o início do contrapiso no galpão B.",
    fases: ["Serviços preliminares", "Fundação", "Superestrutura", "Cobertura"],
    fotos: fotoPlaceholders.slice(2, 4),
    status: "Assinado",
    assinaturas: [
      {
        papel: "Engenheiro",
        nome: "Arq. Helena Duarte",
        assinadoEm: "2026-08-06 13:15",
        hash: "1122334455667788",
      },
      {
        papel: "Proprietário",
        nome: "Beatriz Nogueira Lima",
        assinadoEm: "2026-08-06 19:48",
        hash: "8877665544332211",
      },
    ],
  },
];

export function nomeUsuario(id: string) {
  return usuarios.find((u) => u.id === id)?.nome ?? "—";
}

export function obraPorId(id: string) {
  return obras.find((o) => o.id === id);
}

export function registroPorId(id: string) {
  return registros.find((r) => r.id === id);
}

export function registrosDaObra(obraId: string) {
  return registros.filter((r) => r.obraId === obraId);
}

export function obrasVisiveis(perfil: Perfil) {
  const usuario = usuarioAtualPorPerfil[perfil];
  if (perfil === "administrador") return obras;
  if (perfil === "engenheiro") return obras.filter((o) => o.engenheiroId === usuario.id);
  return obras.filter((o) => o.proprietarioId === usuario.id);
}

export function areaTotal(areas: Obra["areas"]) {
  return areas.construir + areas.ampliar + areas.reformar + areas.regularizar;
}
