import { Link } from "@tanstack/react-router";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import type { Registro } from "@/lib/mock-data";
import { CalendarDays, ChevronRight, Images } from "lucide-react";

export function RegistroCard({ registro }: { registro: Registro }) {
  return (
    <Link
      to="/obras/$id/registros/$registroId"
      params={{ id: registro.obraId, registroId: registro.id }}
      className="block"
    >
      <Card className="h-full border-border transition-shadow hover:shadow-md">
        <CardContent className="p-5">
          <div className="flex items-start justify-between gap-3">
            <div>
              <p className="flex items-center gap-2 text-sm font-semibold text-primary">
                <CalendarDays className="size-4" />
                {new Date(registro.data + "T12:00:00").toLocaleDateString("pt-BR")}
              </p>
              <p className="mt-1 text-xs text-muted-foreground">{registro.posicao}</p>
            </div>
            <div className="flex items-center gap-2">
              <Badge variant={registro.status === "Assinado" ? "assinado" : "pendente"}>
                {registro.status}
              </Badge>
              <ChevronRight className="size-4 text-muted-foreground" />
            </div>
          </div>

          <p className="mt-3 line-clamp-2 text-sm text-foreground">{registro.decisoes}</p>

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

          {registro.fotos.length > 0 && (
            <div className="mt-4 flex items-center gap-2">
              <div className="flex gap-2">
                {registro.fotos.slice(0, 3).map((src, i) => (
                  <img
                    key={i}
                    src={src}
                    alt={`Foto ${i + 1} da visita técnica`}
                    loading="lazy"
                    className="size-14 rounded-md border border-border object-cover"
                  />
                ))}
              </div>
              <span className="flex items-center gap-1 text-xs text-muted-foreground">
                <Images className="size-3.5" /> {registro.fotos.length} foto(s)
              </span>
            </div>
          )}
        </CardContent>
      </Card>
    </Link>
  );
}
