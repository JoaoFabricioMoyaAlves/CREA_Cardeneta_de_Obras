import { Link } from "@tanstack/react-router";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { areaTotal, nomeUsuario, registrosDaObra, type Obra } from "@/lib/mock-data";
import { usePerfil } from "@/lib/perfil-context";
import { StatusBadge } from "./ObraCard";
import { RegistroCard } from "@/features/registros/components/RegistroCard";
import { BlocoAssinaturas } from "@/features/assinatura/components/AssinaturaStatusCard";
import { CheckCircle2, FileText, MapPin, Plus } from "lucide-react";

export function ObraDetalhe({ obra }: { obra: Obra }) {
  const { perfil } = usePerfil();
  const registros = registrosDaObra(obra.id);
  const ehEngenheiro = perfil === "engenheiro";

  const itensArea = [
    ["Construir", obra.areas.construir],
    ["Ampliar", obra.areas.ampliar],
    ["Reformar", obra.areas.reformar],
    ["Regularizar", obra.areas.regularizar],
  ] as const;

  return (
    <div className="space-y-6">
      <Card className="border-border bg-secondary/40">
        <CardContent className="space-y-5 p-5 md:p-6">
          <div className="flex flex-wrap items-start justify-between gap-4">
            <div>
              <p className="font-mono text-xs tracking-wide text-link">{obra.numero}</p>
              <h1 className="mt-1 flex items-start gap-2 text-xl font-bold text-primary md:text-2xl">
                <MapPin className="mt-1 size-5 shrink-0" />
                {obra.local}
              </h1>
              <p className="mt-1 pl-7 text-sm text-muted-foreground">{obra.cidade}</p>
            </div>
            <div className="flex flex-col items-end gap-3">
              <StatusBadge status={obra.status} />
              <div className="flex flex-wrap gap-2">
                {ehEngenheiro && (
                  <Button asChild className="min-h-11">
                    <Link to="/obras/$id/registros/novo" params={{ id: obra.id }}>
                      <Plus className="mr-1.5 size-4" /> Novo registro
                    </Link>
                  </Button>
                )}
                {ehEngenheiro && (
                  <Button
                    asChild={obra.status === "Ativa"}
                    variant="destaque"
                    className="min-h-11"
                    disabled={obra.status !== "Ativa"}
                  >
                    {obra.status === "Ativa" ? (
                      <Link to="/obras/$id/finalizar" params={{ id: obra.id }}>
                        <CheckCircle2 className="mr-1.5 size-4" /> Finalizar caderneta
                      </Link>
                    ) : (
                      <span>
                        <CheckCircle2 className="mr-1.5 inline size-4" /> Finalizar caderneta
                      </span>
                    )}
                  </Button>
                )}
              </div>
            </div>
          </div>

          <div className="grid gap-4 border-t border-border pt-4 md:grid-cols-3">
            <Info titulo="Responsável técnico" valor={nomeUsuario(obra.engenheiroId)} />
            <Info titulo="Proprietário" valor={nomeUsuario(obra.proprietarioId)} />
            <Info titulo="Número RT" valor={obra.numeroRT} />
            <Info titulo="Tipo de edificação" valor={obra.tipoEdificacao} />
            <Info titulo="Atividade técnica" valor={obra.atividades.join(", ") || "—"} />
            <Info titulo="Empresa" valor={obra.empresa ?? "Não informada"} />
          </div>

          <div className="grid gap-3 border-t border-border pt-4 sm:grid-cols-2 lg:grid-cols-5">
            {itensArea.map(([label, valor]) => (
              <div key={label} className="rounded-md bg-card p-3">
                <p className="text-xs text-muted-foreground">{label}</p>
                <p className="text-base font-semibold text-foreground">
                  {valor.toLocaleString("pt-BR")} m²
                </p>
              </div>
            ))}
            <div className="rounded-md bg-primary p-3 text-primary-foreground">
              <p className="text-xs text-white/75">Área total</p>
              <p className="text-base font-bold">
                {areaTotal(obra.areas).toLocaleString("pt-BR")} m²
              </p>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card className="border-border">
        <CardContent className="p-5 md:p-6">
          <BlocoAssinaturas
            titulo="Assinaturas da caderneta"
            assinaturas={obra.assinaturas}
            podeAssinar={perfil !== "administrador"}
          />
        </CardContent>
      </Card>

      <section className="space-y-4">
        <div className="flex items-center justify-between">
          <h2 className="flex items-center gap-2 text-lg font-semibold text-primary">
            <FileText className="size-5" /> Registros de visita
            <Badge variant="secondary">{registros.length}</Badge>
          </h2>
        </div>
        {registros.length === 0 ? (
          <Card className="border-dashed">
            <CardContent className="p-8 text-center text-sm text-muted-foreground">
              Nenhum registro de visita lançado nesta caderneta.
            </CardContent>
          </Card>
        ) : (
          <div className="grid gap-4 lg:grid-cols-2">
            {registros.map((r) => (
              <RegistroCard key={r.id} registro={r} />
            ))}
          </div>
        )}
      </section>
    </div>
  );
}

function Info({ titulo, valor }: { titulo: string; valor: string }) {
  return (
    <div>
      <p className="text-xs tracking-wide text-muted-foreground uppercase">{titulo}</p>
      <p className="text-sm font-medium text-foreground">{valor}</p>
    </div>
  );
}
