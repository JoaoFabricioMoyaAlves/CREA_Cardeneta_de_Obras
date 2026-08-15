import { Link } from "@tanstack/react-router";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { formatarData } from "@/lib/format";
import type { RegistroResponse } from "@/lib/api/types";
import { CalendarDays, ChevronRight, Images } from "lucide-react";

export function RegistroCard({ registro, obraId }: { registro: RegistroResponse; obraId: number }) {
  return (
    <Link
      to="/obras/$id/registros/$registroId"
      params={{ id: String(obraId), registroId: String(registro.id) }}
      className="block"
    >
      <Card className="h-full border-border transition-shadow hover:shadow-md">
        <CardContent className="p-5">
          <div className="flex items-start justify-between gap-3">
            <div>
              <p className="flex items-center gap-2 text-sm font-semibold text-primary">
                <CalendarDays className="size-4" />
                {formatarData(registro.dataVisita)}
              </p>
              <p className="mt-1 text-xs text-muted-foreground">{registro.posicaoObra}</p>
            </div>
            <div className="flex items-center gap-2">
              <Badge variant={registro.status === "Assinado" ? "assinado" : "pendente"}>
                {registro.status === "Assinado" ? "Assinado" : "Pendente assinatura"}
              </Badge>
              <ChevronRight className="size-4 text-muted-foreground" />
            </div>
          </div>

          <p className="mt-3 line-clamp-2 text-sm text-foreground">{registro.decisoesOrientacoes}</p>

          <div className="mt-4 flex flex-wrap gap-1.5">
            {registro.fases.slice(0, 4).map((f) => (
              <span
                key={f}
                className="rounded bg-secondary px-2 py-0.5 text-xs text-secondary-foreground"
              >
                {f}
              </span>
            ))}
            {registro.fases.length > 4 && (
              <span className="rounded bg-secondary px-2 py-0.5 text-xs text-secondary-foreground">
                +{registro.fases.length - 4}
              </span>
            )}
          </div>

          {registro.imagens.length > 0 && (
            <p className="mt-4 flex items-center gap-1 text-xs text-muted-foreground">
              <Images className="size-3.5" /> {registro.imagens.length} foto(s)
            </p>
          )}
        </CardContent>
      </Card>
    </Link>
  );
}
