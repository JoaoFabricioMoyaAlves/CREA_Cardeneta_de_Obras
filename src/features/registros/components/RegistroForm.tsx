import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { useMutation } from "@tanstack/react-query";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { FASES_SERVICO, POSICOES_OBRA } from "@/lib/constants";
import { criarRegistro } from "@/lib/api/registros";
import { ApiError } from "@/lib/api/client";
import type { ObraResponse } from "@/lib/api/types";
import { AlertCircle, ImagePlus, Loader2 } from "lucide-react";
import { toast } from "sonner";

export function RegistroForm({ obra }: { obra: ObraResponse }) {
  const navigate = useNavigate();
  const [data, setData] = useState("");
  const [posicao, setPosicao] = useState("");
  const [decisoes, setDecisoes] = useState("");
  const [fases, setFases] = useState<string[]>([]);
  const [erro, setErro] = useState<string | null>(null);

  function alternarFase(f: string) {
    setFases((prev) => (prev.includes(f) ? prev.filter((x) => x !== f) : [...prev, f]));
  }

  const mutation = useMutation({
    mutationFn: criarRegistro,
    onSuccess: (registro) => {
      toast.success("Registro de visita criado. Adicione as fotos e depois assine.");
      navigate({
        to: "/obras/$id/registros/$registroId",
        params: { id: String(obra.id), registroId: String(registro.id) },
      });
    },
    onError: (err) => {
      setErro(err instanceof ApiError ? err.message : "Não foi possível salvar o registro.");
    },
  });

  function salvar(e: React.FormEvent) {
    e.preventDefault();
    if (!data || !posicao || !decisoes.trim()) {
      setErro("Informe a data da visita, a posição da obra e as decisões/orientações.");
      return;
    }
    setErro(null);
    mutation.mutate({
      obraId: obra.id,
      dataVisita: data,
      posicaoObra: posicao,
      decisoesOrientacoes: decisoes.trim(),
      fasesSelecionadas: fases,
    });
  }

  return (
    <form onSubmit={salvar} className="space-y-6" noValidate>
      <Card className="border-border">
        <CardContent className="space-y-6 p-5 md:p-6">
          <div className="grid gap-5 md:grid-cols-2">
            <div className="space-y-2">
              <Label htmlFor="data">Data da visita *</Label>
              <Input
                id="data"
                type="date"
                className="min-h-11"
                value={data}
                onChange={(e) => setData(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label>Posição da obra *</Label>
              <Select value={posicao} onValueChange={setPosicao}>
                <SelectTrigger className="min-h-11 w-full">
                  <SelectValue placeholder="Selecione a posição" />
                </SelectTrigger>
                <SelectContent>
                  {POSICOES_OBRA.map((p) => (
                    <SelectItem key={p} value={p}>
                      {p}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="decisoes">Decisões e orientações *</Label>
            <Textarea
              id="decisoes"
              rows={6}
              placeholder="Descreva as verificações realizadas, orientações à equipe e decisões técnicas tomadas nesta visita…"
              value={decisoes}
              onChange={(e) => setDecisoes(e.target.value)}
            />
          </div>

          <Separator />

          <div className="space-y-3">
            <h2 className="text-lg font-semibold text-primary">Fases de serviço executadas</h2>
            <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-3">
              {FASES_SERVICO.map((f) => (
                <label
                  key={f}
                  className="flex min-h-11 cursor-pointer items-center gap-3 rounded-md border border-border px-4 py-2 hover:bg-secondary/40"
                >
                  <Checkbox checked={fases.includes(f)} onCheckedChange={() => alternarFase(f)} />
                  <span className="text-sm text-foreground">{f}</span>
                </label>
              ))}
            </div>
          </div>
        </CardContent>
      </Card>

      <div className="flex items-start gap-2 rounded-md border border-border bg-secondary/50 p-3">
        <ImagePlus className="mt-0.5 size-4 shrink-0 text-primary" />
        <p className="text-xs text-muted-foreground">
          As fotos da visita e a dupla assinatura (Engenheiro + Proprietário) são feitas na
          tela do registro, depois de salvar.
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
          onClick={() => navigate({ to: "/obras/$id", params: { id: String(obra.id) } })}
        >
          Cancelar
        </Button>
        <Button type="submit" className="min-h-11 px-6" disabled={mutation.isPending}>
          {mutation.isPending && <Loader2 className="mr-2 size-4 animate-spin" />}
          {mutation.isPending ? "Salvando…" : "Salvar registro"}
        </Button>
      </div>
    </form>
  );
}
