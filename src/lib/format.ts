export function formatarMoeda(valor: number) {
  return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

// Aplica a máscara 000.000.000-00 conforme o usuário digita — usar direto
// no onChange do input (recebe o valor bruto, devolve já mascarado). Só
// mantém dígitos e limita a 11, então cola/apaga também funcionam bem.
export function formatarCpfInput(valor: string): string {
  const digitos = valor.replace(/\D/g, "").slice(0, 11);
  let resultado = digitos.slice(0, 3);
  if (digitos.length > 3) resultado += "." + digitos.slice(3, 6);
  if (digitos.length > 6) resultado += "." + digitos.slice(6, 9);
  if (digitos.length > 9) resultado += "-" + digitos.slice(9, 11);
  return resultado;
}

// Data de hoje no formato "YYYY-MM-DD" (fuso local, não UTC) — usada como
// valor fixo e não editável nos campos de data que registram o momento do
// preenchimento (visita, recibo, conclusão).
export function dataHojeIso(): string {
  const hoje = new Date();
  const ano = hoje.getFullYear();
  const mes = String(hoje.getMonth() + 1).padStart(2, "0");
  const dia = String(hoje.getDate()).padStart(2, "0");
  return `${ano}-${mes}-${dia}`;
}

// API manda datas "YYYY-MM-DD" (DateOnly do .NET) — evita o bug clássico de
// `new Date("YYYY-MM-DD")` interpretar como UTC e exibir o dia anterior.
export function formatarData(dataIso: string) {
  return new Date(`${dataIso.slice(0, 10)}T12:00:00`).toLocaleDateString("pt-BR");
}

export function formatarDataHora(dataIso: string) {
  return new Date(dataIso).toLocaleString("pt-BR", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}
