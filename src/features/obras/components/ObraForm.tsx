import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { useMutation, useQuery } from "@tanstack/react-query";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { ATIVIDADES_TECNICAS, TIPOS_EDIFICACAO } from "@/lib/constants";
import { listarUsuarios } from "@/lib/api/usuarios";
import { criarObra } from "@/lib/api/obras";
import { ApiError } from "@/lib/api/client";
import { AlertCircle, Loader2, Ruler } from "lucide-react";
import { toast } from "sonner";

export function ObraForm() {
  const navigate = useNavigate();
  const [proprietario, setProprietario] = useState("");
  const [engenheiro, setEngenheiro] = useState("");
  const [tipo, setTipo] = useState("");
  const [local, setLocal] = useState("");
  const [cidade, setCidade] = useState("");
  const [rt, setRt] = useState("");
  const [valorObra, setValorObra] = useState("");
  const [empresa, setEmpresa] = useState("");
  const [dataRecibo, setDataRecibo] = useState("");
  const [atividades, setAtividades] = useState<string[]>([]);
  const [areas, setAreas] = useState({
    construir: "",
    ampliar: "",
    reformar: "",
    regularizar: "",
  });
  const [erro, setErro] = useState<string | null>(null);

  const { data: usuarios } = useQuery({ queryKey: ["usuarios"], queryFn: listarUsuarios });
  const engenheiros = usuarios?.filter((u) => u.perfil === "Engenheiro") ?? [];
  const proprietarios = usuarios?.filter((u) => u.perfil === "Proprietario") ?? [];

  const total = Object.values(areas).reduce((s, v) => s + (parseFloat(v) || 0), 0);

  function alternarAtividade(a: string) {
    setAtividades((prev) => (prev.includes(a) ? prev.filter((x) => x !== a) : [...prev, a]));
  }

  const mutation = useMutation({
    mutationFn: criarObra,
    onSuccess: (obra) => {
      toast.success(`Caderneta ${obra.numeroCaderneta} criada. Aguardando assinatura do Engenheiro e do Proprietário.`);
      navigate({ to: "/obras/$id", params: { id: String(obra.id) } });
    },
    onError: (err) => {
      setErro(err instanceof ApiError ? err.message : "Não foi possível criar a caderneta.");
    },
  });

  function salvar(e: React.FormEvent) {
    e.preventDefault();
    if (!proprietario || !engenheiro || !local.trim() || !cidade.trim() || !rt.trim() || !tipo || !valorObra.trim() || !dataRecibo) {
      setErro("Preencha proprietário, engenheiro, local, cidade, número RT, tipo de edificação, valor da obra e data do recibo.");
      return;
    }
    setErro(null);
    mutation.mutate({
      proprietarioId: proprietario,
      profissionalId: engenheiro,
      localObra: local.trim(),
      cidade: cidade.trim(),
      numeroRt: rt.trim(),
      areaConstruirM2: parseFloat(areas.construir) || 0,
      areaAmpliarM2: parseFloat(areas.ampliar) || 0,
      areaReformarM2: parseFloat(areas.reformar) || 0,
      areaRegularizarM2: parseFloat(areas.regularizar) || 0,
      tipoEdificacao: tipo,
      tipoEdificacaoOutros: null,
      ativTecnicaDirecao: atividades.includes("Direção"),
      ativTecnicaExecucao: atividades.includes("Execução"),
      ativTecnicaFiscalizacao: atividades.includes("Fiscalização"),
      ativTecnicaProjeto: atividades.includes("Projeto"),
      valorObra: parseFloat(valorObra) || 0,
      dataReciboAbertura: dataRecibo,
      nomeEmpresa: empresa.trim() || null,
      cnpjEmpresa: null,
    });
  }

  return (
    <form onSubmit={salvar} className="space-y-6" noValidate>
      <Card className="border-border">
        <CardContent className="space-y-6 p-5 md:p-6">
          <h2 className="text-lg font-semibold text-primary">Responsáveis</h2>
          <div className="grid gap-5 md:grid-cols-2">
            <div className="space-y-2">
              <Label>Proprietário do imóvel *</Label>
              <Select value={proprietario} onValueChange={setProprietario}>
                <SelectTrigger className="min-h-11 w-full">
                  <SelectValue placeholder="Selecione o proprietário" />
                </SelectTrigger>
                <SelectContent>
                  {proprietarios.map((u) => (
                    <SelectItem key={u.id} value={u.id}>
                      {u.nome} — {u.cpf}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Engenheiro/Arquiteto responsável *</Label>
              <Select value={engenheiro} onValueChange={setEngenheiro}>
                <SelectTrigger className="min-h-11 w-full">
                  <SelectValue placeholder="Selecione o profissional" />
                </SelectTrigger>
                <SelectContent>
                  {engenheiros.map((u) => (
                    <SelectItem key={u.id} value={u.id}>
                      {u.nome} — {u.numeroRegistro}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <Separator />

          <h2 className="text-lg font-semibold text-primary">Dados da obra</h2>
          <div className="grid gap-5 md:grid-cols-2">
            <div className="space-y-2">
              <Label htmlFor="local">Local da obra *</Label>
              <Input
                id="local"
                className="min-h-11"
                placeholder="Rua, número, bairro"
                value={local}
                onChange={(e) => setLocal(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="cidade">Cidade *</Label>
              <Input
                id="cidade"
                className="min-h-11"
                placeholder="Cidade / UF"
                value={cidade}
                onChange={(e) => setCidade(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="rt">Número RT *</Label>
              <Input
                id="rt"
                className="min-h-11"
                placeholder="RT 00.000/2026"
                value={rt}
                onChange={(e) => setRt(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label>Tipo de edificação *</Label>
              <Select value={tipo} onValueChange={setTipo}>
                <SelectTrigger className="min-h-11 w-full">
                  <SelectValue placeholder="Selecione o tipo" />
                </SelectTrigger>
                <SelectContent>
                  {TIPOS_EDIFICACAO.map((t) => (
                    <SelectItem key={t} value={t}>
                      {t}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="valor-obra">Valor da obra (R$) *</Label>
              <Input
                id="valor-obra"
                type="number"
                inputMode="decimal"
                min="0"
                step="0.01"
                className="min-h-11"
                placeholder="0,00"
                value={valorObra}
                onChange={(e) => setValorObra(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="empresa">Empresa (opcional)</Label>
              <Input
                id="empresa"
                className="min-h-11"
                placeholder="Razão social"
                value={empresa}
                onChange={(e) => setEmpresa(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="recibo">Data do recibo de abertura *</Label>
              <Input
                id="recibo"
                type="date"
                className="min-h-11"
                value={dataRecibo}
                onChange={(e) => setDataRecibo(e.target.value)}
              />
            </div>
          </div>

          <Separator />

          <h2 className="text-lg font-semibold text-primary">Áreas (m²)</h2>
          <div className="grid gap-5 md:grid-cols-4">
            {(
              [
                ["construir", "Construir"],
                ["ampliar", "Ampliar"],
                ["reformar", "Reformar"],
                ["regularizar", "Regularizar"],
              ] as const
            ).map(([campo, label]) => (
              <div key={campo} className="space-y-2">
                <Label htmlFor={campo}>{label}</Label>
                <Input
                  id={campo}
                  type="number"
                  min="0"
                  step="0.01"
                  className="min-h-11"
                  placeholder="0,00"
                  value={areas[campo]}
                  onChange={(e) => setAreas({ ...areas, [campo]: e.target.value })}
                />
              </div>
            ))}
          </div>
          <div className="flex items-center justify-between rounded-lg bg-secondary p-4">
            <span className="flex items-center gap-2 text-sm font-medium text-primary">
              <Ruler className="size-4" /> Área total da obra
            </span>
            <span className="text-2xl font-bold text-primary">
              {total.toLocaleString("pt-BR", { maximumFractionDigits: 2 })} m²
            </span>
          </div>

          <Separator />

          <h2 className="text-lg font-semibold text-primary">Atividade técnica</h2>
          <div className="grid gap-3 md:grid-cols-2">
            {ATIVIDADES_TECNICAS.map((a) => (
              <label
                key={a}
                className="flex min-h-11 cursor-pointer items-center gap-3 rounded-md border border-border px-4 py-2 hover:bg-secondary/40"
              >
                <Checkbox
                  checked={atividades.includes(a)}
                  onCheckedChange={() => alternarAtividade(a)}
                />
                <span className="text-sm text-foreground">{a}</span>
              </label>
            ))}
          </div>
        </CardContent>
      </Card>

      <div className="flex items-start gap-2 rounded-md border border-border bg-secondary/50 p-3">
        <AlertCircle className="mt-0.5 size-4 shrink-0 text-primary" />
        <p className="text-xs text-muted-foreground">
          A caderneta nasce como <strong>Pendente assinatura</strong>. O Engenheiro e o
          Proprietário assinam depois, cada um logado com sua própria conta, na tela de
          detalhe da obra.
        </p>
      </div>

      {erro && (
        <div className="flex items-start gap-2 rounded-md border border-destructive/30 bg-destructive/5 p-3">
          <AlertCircle className="mt-0.5 size-4 shrink-0 text-destructive" />
          <p className="text-sm text-destructive">{erro}</p>
        </div>
      )}

      <div className="flex flex-wrap justify-end gap-3">
        <Button
          type="button"
          variant="outline"
          className="min-h-11"
          onClick={() => navigate({ to: "/dashboard" })}
        >
          Cancelar
        </Button>
        <Button type="submit" className="min-h-11 px-6" disabled={mutation.isPending}>
          {mutation.isPending && <Loader2 className="mr-2 size-4 animate-spin" />}
          {mutation.isPending ? "Criando…" : "Criar caderneta"}
        </Button>
      </div>
    </form>
  );
}
