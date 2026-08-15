import { Link } from "@tanstack/react-router";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { formatarMoeda } from "@/lib/format";
import type { ObraResponse, StatusObraApi } from "@/lib/api/types";
import { ChevronRight, CircleDollarSign, HardHat, MapPin, Ruler, User } from "lucide-react";

const STATUS_INFO: Record<StatusObraApi, { label: string; variant: "pendente" | "ativa" | "finalizada" }> = {
  PendenteAssinatura: { label: "Pendente assinatura", variant: "pendente" },
  Ativa: { label: "Ativa", variant: "ativa" },
  Finalizada: { label: "Finalizada", variant: "finalizada" },
};

export function StatusBadge({ status }: { status: StatusObraApi }) {
  const info = STATUS_INFO[status];
  return <Badge variant={info.variant}>{info.label}</Badge>;
}

export function ObraCard({ obra }: { obra: ObraResponse }) {
  return (
    <Link to="/obras/$id" params={{ id: String(obra.id) }} className="block">
      <Card className="h-full gap-0 border-border transition-shadow hover:shadow-md">
        <CardContent className="p-5">
          <div className="flex items-start justify-between gap-3">
            <div>
              <p className="font-mono text-xs tracking-wide text-link">{obra.numeroCaderneta}</p>
              <h3 className="mt-1 flex items-start gap-1.5 text-base font-semibold text-primary">
                <MapPin className="mt-0.5 size-4 shrink-0" />
                {obra.localObra}
              </h3>
              <p className="mt-0.5 pl-5.5 text-xs text-muted-foreground">{obra.cidade}</p>
            </div>
            <ChevronRight className="mt-1 size-5 shrink-0 text-muted-foreground" />
          </div>

          <div className="mt-4">
            <StatusBadge status={obra.status} />
          </div>

          <div className="mt-4 space-y-2 rounded-md bg-secondary/50 p-3 text-sm">
            <p className="flex items-center gap-2 text-foreground">
              <HardHat className="size-4 text-primary" />
              <span className="text-muted-foreground">Responsável técnico:</span>
              {obra.nomeProfissional}
            </p>
            <p className="flex items-center gap-2 text-foreground">
              <User className="size-4 text-primary" />
              <span className="text-muted-foreground">Proprietário:</span>
              {obra.nomeProprietario}
            </p>
            <p className="flex items-center gap-2 text-foreground">
              <Ruler className="size-4 text-primary" />
              <span className="text-muted-foreground">Área total:</span>
              {obra.areaTotalEdificadaM2.toLocaleString("pt-BR")} m²
            </p>
            <p className="flex items-center gap-2 text-foreground">
              <CircleDollarSign className="size-4 text-primary" />
              <span className="text-muted-foreground">Valor da obra:</span>
              {formatarMoeda(obra.valorObra)}
            </p>
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}
