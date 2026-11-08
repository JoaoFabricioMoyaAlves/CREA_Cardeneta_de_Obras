import { api } from "./client";

export type TipoDocumentoApi = "abertura-obra" | "relato-visita" | "termo-conclusao";

export type AssinaturaDocumento = {
  papel: "Engenheiro" | "Proprietario" | "Administrador";
  nome: string;
  data: string;
  hash: string;
  ip: string;
  carimboVerificado: boolean;
  carimboAutoridade: string | null;
};

export type ProfissionalDocumento = {
  nome: string;
  titulo: string | null;
  registro: string | null;
};
export type EmpresaDocumento = { nome: string | null; cnpj: string | null };
export type ProprietarioDocumento = { nome: string; cpf: string; telefone: string };

export type DadosAbertura = {
  numeroCaderneta: string;
  profissional: ProfissionalDocumento;
  empresa: EmpresaDocumento;
  proprietario: ProprietarioDocumento;
  localObra: string;
  cidade: string;
  estado: string;
  areaConstruirM2: number;
  areaAmpliarM2: number;
  areaReformarM2: number;
  areaRegularizarM2: number;
  areaTotalEdificadaM2: number;
  tipoEdificacao: string;
  tipoEdificacaoOutros: string | null;
  atividadesTecnicas: string[];
  numeroRt: string;
  valorObra: number;
  dataRecibo: string;
};

export type FotoDocumento = { id: number; nome: string; sha256: string | null };

export type DadosRelato = {
  numeroCaderneta: string;
  sequencia: number;
  localObra: string;
  cidade: string;
  estado: string;
  profissional: ProfissionalDocumento;
  proprietario: ProprietarioDocumento;
  dataVisita: string;
  fases: string[];
  posicaoObra: string;
  decisoesOrientacoes: string;
  fotos: FotoDocumento[];
};

export type DadosTermo = {
  numeroCaderneta: string;
  profissional: ProfissionalDocumento;
  empresa: EmpresaDocumento;
  proprietario: ProprietarioDocumento;
  localObra: string;
  cidade: string;
  estado: string;
  dataConclusao: string;
  declaracao: string;
};

export type DocumentoResponse = {
  tipo: "AberturaObra" | "RelatoVisita" | "TermoConclusao";
  referenciaId: number;
  modeloVersao: number;
  congelado: boolean;
  anteriorAoModelo: boolean;
  pendente: boolean;
  hashDocumento: string;
  dados: unknown;
  assinaturas: AssinaturaDocumento[];
  papelQuePodeAssinar: AssinaturaDocumento["papel"] | null;
};

export type AssinarDocumentoPayload = {
  cpf: string;
  senha: string;
  hashDocumentoVisualizado: string;
};

export function obterDocumento(tipo: TipoDocumentoApi, referenciaId: number) {
  return api.get<DocumentoResponse>(`/api/documentos/${tipo}/${referenciaId}`);
}

export function assinarDocumento(
  tipo: TipoDocumentoApi,
  referenciaId: number,
  payload: AssinarDocumentoPayload,
) {
  return api.post<DocumentoResponse>(`/api/documentos/${tipo}/${referenciaId}/assinar`, payload);
}
