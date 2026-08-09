import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { FASES_SERVICO } from "@/lib/constants";
import type { Registro } from "@/lib/mock-data";
import { BadgeCheck, CalendarDays, Check, Circle } from "lucide-react";

export function RegistroDetalhe({ registro }: { registro: Registro }) {
  return (
    <div className="space-y-6">
      <Card className="border-border bg-secondary/40">
        <CardContent className="flex flex-wrap items-start justify-between gap-4 p-5 md:p-6">
          <div>
            <p className="flex items-center gap-2 text-lg font-bold text-primary">
              <CalendarDays className="size-5" />
              Visita de {new Date(registro.data + "T12:00:00").toLocaleDateString("pt-BR")}
            </p>
            <p className="mt-1 text-sm text-muted-foreground">
              Posição da obra: {registro.posicao}
            </p>
          </div>
          <Badge variant={registro.status === "Assinado" ? "assinado" : "pendente"}>
            {registro.status}
          </Badge>
        </CardContent>
      </Card>

      <Card className="border-border">
        <CardContent className="space-y-2 p-5 md:p-6">
          <h2 className="text-base font-semibold text-primary">Decisões e orientações</h2>
          <p className="text-sm leading-relaxed whitespace-pre-line text-foreground">
            {registro.decisoes}
          </p>
        </CardContent>
      </Card>

      <Card className="border-border">
        <CardContent className="space-y-4 p-5 md:p-6">
          <h2 className="text-base font-semibold text-primary">Fases de serviço</h2>
          <div className="grid gap-2 md:grid-cols-2 xl:grid-cols-3">
            {FASES_SERVICO.map((f) => {
              const concluida = registro.fases.includes(f);
              return (
                <div
                  key={f}
                  className={
                    concluida
                      ? "flex min-h-11 items-center gap-3 rounded-md bg-secondary px-4 py-2"
                      : "flex min-h-11 items-center gap-3 rounded-md border border-border px-4 py-2"
                  }
                >
                  {concluida ? (
                    <span className="flex size-5 items-center justify-center rounded-full bg-primary text-primary-foreground">
                      <Check className="size-3" />
                    </span>
                  ) : (
                    <Circle className="size-5 text-muted-foreground/50" />
                  )}
                  <span
                    className={
                      concluida
                        ? "text-sm font-medium text-primary"
                        : "text-sm text-muted-foreground"
                    }
                  >
                    {f}
                  </span>
                </div>
              );
            })}
          </div>
        </CardContent>
      </Card>

      {registro.fotos.length > 0 && (
        <Card className="border-border">
          <CardContent className="space-y-4 p-5 md:p-6">
            <h2 className="text-base font-semibold text-primary">
              Galeria de fotos ({registro.fotos.length})
            </h2>
            <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
              {registro.fotos.map((src, i) => (
                <img
                  key={i}
                  src={src}
                  alt={`Registro fotográfico ${i + 1} da visita técnica`}
                  loading="lazy"
                  className="aspect-4/3 w-full rounded-lg border border-border object-cover"
                />
              ))}
            </div>
          </CardContent>
        </Card>
      )}

      <Card className="border-border">
        <CardContent className="space-y-4 p-5 md:p-6">
          <h2 className="text-base font-semibold text-primary">Assinaturas</h2>
          <div className="grid gap-3 md:grid-cols-2">
            {registro.assinaturas.map((a) => (
              <div
                key={a.papel}
                className={
                  a.assinadoEm
                    ? "rounded-lg border border-secondary bg-secondary/60 p-4"
                    : "rounded-lg border border-warning/40 bg-warning-soft p-4"
                }
              >
                <div className="flex items-start justify-between gap-3">
                  <div>
                    <p className="text-sm font-semibold text-primary">{a.papel}</p>
                    <p className="text-sm text-foreground">{a.nome}</p>
                  </div>
                  {a.assinadoEm && <BadgeCheck className="size-5 shrink-0 text-primary" />}
                </div>
                {a.assinadoEm ? (
                  <div className="mt-2 space-y-1">
                    <p className="text-xs font-medium text-primary">
                      Assinado em {a.assinadoEm}
                    </p>
                    <p className="font-mono text-xs text-muted-foreground">
                      {a.hash?.slice(0, 6)}…{a.hash?.slice(-4)}
                    </p>
                  </div>
                ) : (
                  <p className="mt-2 text-xs font-medium text-warning-foreground/80">
                    Aguardando assinatura
                  </p>
                )}
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
