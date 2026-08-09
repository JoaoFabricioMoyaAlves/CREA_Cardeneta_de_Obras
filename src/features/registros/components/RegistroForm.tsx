import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { FASES_SERVICO, POSICOES_OBRA } from "@/lib/constants";
import { nomeUsuario, type Obra } from "@/lib/mock-data";
import { BlocoAssinaturas } from "@/features/assinatura/components/AssinaturaStatusCard";
import { AlertCircle, ImagePlus, X } from "lucide-react";
import { toast } from "sonner";

const fotosMock = [
  "https://images.unsplash.com/photo-1503387762-592deb58ef4e?w=600&q=70",
  "https://images.unsplash.com/photo-1541888946425-d81bb19240f5?w=600&q=70",
  "https://images.unsplash.com/photo-1504307651254-35680f356dfd?w=600&q=70",
];

export function RegistroForm({ obra }: { obra: Obra }) {
  const navigate = useNavigate();
  const [data, setData] = useState("");
  const [posicao, setPosicao] = useState("");
  const [decisoes, setDecisoes] = useState("");
  const [fases, setFases] = useState<string[]>([]);
  const [fotos, setFotos] = useState<string[]>(fotosMock.slice(0, 2));
  const [erro, setErro] = useState<string | null>(null);

  function alternarFase(f: string) {
    setFases((prev) => (prev.includes(f) ? prev.filter((x) => x !== f) : [...prev, f]));
  }

  function salvar(e: React.FormEvent) {
    e.preventDefault();
    if (!data || !posicao || !decisoes.trim()) {
      setErro("Informe a data da visita, a posição da obra e as decisões/orientações.");
      return;
    }
    setErro(null);
    toast.success("Registro de visita salvo (demonstração).");
    navigate({ to: "/obras/$id", params: { id: obra.id } });
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

          <Separator />

          <div className="space-y-3">
            <h2 className="text-lg font-semibold text-primary">Fotos da visita</h2>
            <div className="grid gap-3 sm:grid-cols-3 lg:grid-cols-4">
              {fotos.map((src, i) => (
                <div key={i} className="relative overflow-hidden rounded-lg border border-border">
                  <img
                    src={src}
                    alt={`Pré-visualização da foto ${i + 1}`}
                    className="aspect-4/3 w-full object-cover"
                  />
                  <button
                    type="button"
                    aria-label="Remover foto"
                    onClick={() => setFotos(fotos.filter((_, idx) => idx !== i))}
                    className="absolute top-2 right-2 flex size-8 items-center justify-center rounded-full bg-primary/90 text-primary-foreground"
                  >
                    <X className="size-4" />
                  </button>
                </div>
              ))}
              <button
                type="button"
                onClick={() => setFotos([...fotos, fotosMock[fotos.length % fotosMock.length]!])}
                className="flex aspect-4/3 min-h-28 flex-col items-center justify-center gap-2 rounded-lg border-2 border-dashed border-border text-muted-foreground hover:border-link hover:text-link"
              >
                <ImagePlus className="size-6" />
                <span className="text-xs font-medium">Adicionar foto</span>
              </button>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card className="border-border">
        <CardContent className="p-5 md:p-6">
          <BlocoAssinaturas
            assinaturas={[
              {
                papel: "Engenheiro",
                nome: nomeUsuario(obra.engenheiroId),
                assinadoEm: null,
                hash: null,
              },
              {
                papel: "Proprietário",
                nome: nomeUsuario(obra.proprietarioId),
                assinadoEm: null,
                hash: null,
              },
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
          onClick={() => navigate({ to: "/obras/$id", params: { id: obra.id } })}
        >
          Cancelar
        </Button>
        <Button type="submit" className="min-h-11 px-6">
          Salvar registro
        </Button>
      </div>
    </form>
  );
}
