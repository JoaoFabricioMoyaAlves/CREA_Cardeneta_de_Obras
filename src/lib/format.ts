export function formatarMoeda(valor: number) {
  return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
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
