import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { nomeUsuario, type Obra } from "@/lib/mock-data";
import { BlocoAssinaturas } from "@/features/assinatura/components/AssinaturaStatusCard";
import { AlertCircle, AlertTriangle } from "lucide-react";
import { toast } from "sonner";

export function TermoConclusaoForm({ obra }: { obra: Obra }) {
  const navigate = useNavigate();
  const [data, setData] = useState("");
  const [declaracao, setDeclaracao] = useState(
    "Declaro, para os devidos fins e sob responsabilidade técnica, que a obra foi concluída conforme o projeto aprovado e as normas técnicas aplicáveis.",
  );
  const [erro, setErro] = useState<string | null>(null);

  function salvar(e: React.FormEvent) {
    e.preventDefault();
    if (!data || !declaracao.trim()) {
      setErro("Informe a data de conclusão e a declaração técnica.");
      return;
    }
    setErro(null);
    toast.success("Termo de conclusão emitido (demonstração).");
    navigate({ to: "/obras/$id", params: { id: obra.id } });
  }

  return (
    <form onSubmit={salvar} className="space-y-6" noValidate>
      <div className="flex items-start gap-3 rounded-lg border border-warning/40 bg-warning-soft p-4">
        <AlertTriangle className="mt-0.5 size-5 shrink-0 text-warning" />
        <p className="text-sm text-foreground">
          Após a dupla assinatura, a caderneta <strong>{obra.numero}</strong> será encerrada e
          passará a ficar disponível somente para consulta.
        </p>
      </div>

      <Card className="border-border">
        <CardContent className="space-y-5 p-5 md:p-6">
          <div className="space-y-2 md:max-w-xs">
            <Label htmlFor="dataConclusao">Data de conclusão *</Label>
            <Input
              id="dataConclusao"
              type="date"
              className="min-h-11"
              value={data}
              onChange={(e) => setData(e.target.value)}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="declaracao">Declaração de conclusão *</Label>
            <Textarea
              id="declaracao"
              rows={6}
              value={declaracao}
              onChange={(e) => setDeclaracao(e.target.value)}
            />
          </div>
        </CardContent>
      </Card>

      <Card className="border-border">
        <CardContent className="p-5 md:p-6">
          <BlocoAssinaturas
            titulo="Assinaturas do termo de conclusão"
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
        <Button type="submit" variant="destaque" className="min-h-11 px-6">
          Emitir termo de conclusão
        </Button>
      </div>
    </form>
  );
}
