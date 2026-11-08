import { useState } from "react";
import { Link } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { AssinarDocumentoDialog } from "@/features/documentos/AssinarDocumentoDialog";
import type { TipoDocumentoApi } from "@/lib/api/documentos";
import { formatarDataHora } from "@/lib/format";
import { BadgeCheck, Clock, FileText, PenLine, ShieldCheck } from "lucide-react";

export type AssinaturaSlot = {
  papel: "Engenheiro" | "Proprietario" | "Administrador";
  nome: string;
  assinadoEm: string | null;
  hash: string | null;
  // true só quando é o usuário logado, no papel dele neste documento, e ele
  // ainda não assinou. O servidor confere a mesma regra ao assinar.
  podeAssinar: boolean;
  // Carimbo de tempo RFC 3161 de uma autoridade externa — reforço opcional
  // de não-repúdio além do timestamp do próprio servidor.
  carimboTempoVerificado?: boolean | undefined;
  carimboTempoAutoridade?: string | null | undefined;
};

const LABEL_PAPEL: Record<AssinaturaSlot["papel"], string> = {
  Engenheiro: "Responsável técnico",
  Proprietario: "Proprietário",
  Administrador: "Administrador (CREA)",
};

function AssinaturaStatusCard({
  papel,
  nome,
  assinadoEm,
  hash,
  podeAssinar,
  carimboTempoVerificado,
  carimboTempoAutoridade,
  onAssinar,
}: AssinaturaSlot & { onAssinar: () => void }) {
  const assinado = Boolean(assinadoEm);

  return (
    <Card
      className={
        assinado
          ? "gap-3 border-secondary bg-secondary/60 p-4 shadow-none"
          : "gap-3 border-warning/50 bg-warning-soft p-4 shadow-none"
      }
    >
      <div className="flex items-start justify-between gap-3">
        <div>
          <p className="text-sm font-semibold text-primary">{LABEL_PAPEL[papel]}</p>
          <p className="text-sm text-foreground">{nome}</p>
        </div>
        {assinado ? (
          <BadgeCheck className="size-5 shrink-0 text-primary" />
        ) : (
          <Clock className="size-5 shrink-0 text-warning" />
        )}
      </div>

      {assinado ? (
        <div className="space-y-1">
          <p className="text-xs font-medium text-primary">
            Assinado em {formatarDataHora(assinadoEm!)}
          </p>
          {hash && (
            <p className="font-mono text-xs text-muted-foreground">
              hash {hash.slice(0, 6)}…{hash.slice(-4)}
            </p>
          )}
          {carimboTempoVerificado && (
            <p
              className="flex items-center gap-1 text-xs text-primary"
              title={`Carimbo de tempo RFC 3161 — ${carimboTempoAutoridade}`}
            >
              <ShieldCheck className="size-3.5" />
              Carimbo de tempo verificado ({carimboTempoAutoridade})
            </p>
          )}
        </div>
      ) : (
        <>
          <p className="text-xs font-medium text-warning-foreground/80">
            {podeAssinar ? "Aguardando sua assinatura" : "Aguardando assinatura"}
          </p>
          {podeAssinar && (
            <Button variant="assinar" className="min-h-11 w-full" onClick={onAssinar}>
              <PenLine className="mr-2 size-4" />
              Ler e assinar
            </Button>
          )}
        </>
      )}
    </Card>
  );
}

export function BlocoAssinaturas({
  slots,
  titulo = "Assinaturas necessárias",
  tipo,
  referenciaId,
  onAssinado,
}: {
  slots: AssinaturaSlot[];
  titulo?: string;
  tipo: TipoDocumentoApi;
  referenciaId: number;
  onAssinado: () => void;
}) {
  const [assinando, setAssinando] = useState(false);

  return (
    <section className="space-y-3">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <h3 className="text-base font-semibold text-primary">{titulo}</h3>
        <Button asChild variant="outline" size="sm">
          <Link to="/documentos/$tipo/$id" params={{ tipo, id: String(referenciaId) }}>
            <FileText className="mr-1.5 size-4" /> Ver / imprimir documento
          </Link>
        </Button>
      </div>
      <div className="grid gap-3 md:grid-cols-2">
        {slots.map((slot) => (
          <AssinaturaStatusCard key={slot.papel} {...slot} onAssinar={() => setAssinando(true)} />
        ))}
      </div>
      <AssinarDocumentoDialog
        open={assinando}
        onOpenChange={setAssinando}
        tipo={tipo}
        referenciaId={referenciaId}
        onAssinado={onAssinado}
      />
    </section>
  );
}
