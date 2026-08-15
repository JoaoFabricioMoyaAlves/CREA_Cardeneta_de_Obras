import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { ATIVIDADES_TECNICAS, TIPOS_EDIFICACAO } from "@/lib/constants";
import { usuarios } from "@/lib/mock-data";
import { BlocoAssinaturas } from "@/features/assinatura/components/AssinaturaStatusCard";
import { AlertCircle, Ruler } from "lucide-react";
import { toast } from "sonner";

const engenheiros = usuarios.filter((u) => u.perfil === "engenheiro");
const proprietarios = usuarios.filter((u) => u.perfil === "proprietario");

export function ObraForm() {
  const navigate = useNavigate();
  const [proprietario, setProprietario] = useState("");
  const [engenheiro, setEngenheiro] = useState("");
  const [tipo, setTipo] = useState("");
  const [local, setLocal] = useState("");
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

  const total = Object.values(areas).reduce((s, v) => s + (parseFloat(v) || 0), 0);

  function alternarAtividade(a: string) {
    setAtividades((prev) => (prev.includes(a) ? prev.filter((x) => x !== a) : [...prev, a]));
  }

  function salvar(e: React.FormEvent) {
    e.preventDefault();
    if (!proprietario || !engenheiro || !local.trim() || !rt.trim() || !tipo || !valorObra.trim()) {
      setErro(
        "Preencha proprietário, engenheiro, local da obra, número RT, tipo de edificação e valor da obra.",
      );
      return;
    }
    setErro(null);
    toast.success("Caderneta criada (demonstração). Aguardando assinaturas.");
    navigate({ to: "/dashboard" });
  }

  const nomeEng = engenheiros.find((u) => u.id === engenheiro)?.nome ?? "A definir";
  const nomeProp = proprietarios.find((u) => u.id === proprietario)?.nome ?? "A definir";

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
                      {u.nome} — {u.documento}
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
                      {u.nome} — {u.registroCrea}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <Separator />

          <h2 className="text-lg font-semibold text-primary">Dados da obra</h2>
          <div className="grid gap-5 md:grid-cols-2">
            <div className="space-y-2 md:col-span-2">
              <Label htmlFor="local">Local da obra *</Label>
              <Input
                id="local"
                className="min-h-11"
                placeholder="Rua, número, bairro, cidade"
                value={local}
                onChange={(e) => setLocal(e.target.value)}
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
              <Label htmlFor="empresa">Empresa / CNPJ (opcional)</Label>
              <Input
                id="empresa"
                className="min-h-11"
                placeholder="Razão social — 00.000.000/0000-00"
                value={empresa}
                onChange={(e) => setEmpresa(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="recibo">Data do recibo de abertura</Label>
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

      <Card className="border-border">
        <CardContent className="p-5 md:p-6">
          <BlocoAssinaturas
            assinaturas={[
              { papel: "Engenheiro", nome: nomeEng, assinadoEm: null, hash: null },
              { papel: "Proprietário", nome: nomeProp, assinadoEm: null, hash: null },
            ]}
          />
        </CardContent>
      </Card>

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
        <Button type="submit" className="min-h-11 px-6">
          Criar caderneta
        </Button>
      </div>
    </form>
  );
}
