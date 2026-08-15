import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { formatarDataHora } from "@/lib/format";
import { BadgeCheck, Clock, Loader2, PenLine } from "lucide-react";
import { SignatureCanvas } from "./SignatureCanvas";

export type AssinaturaSlot = {
  papel: "Engenheiro" | "Proprietario";
  nome: string;
  assinadoEm: string | null;
  hash: string | null;
  // true só quando: é o usuário logado, é o papel dele nesta obra, e ele
  // ainda não assinou — nunca é possível assinar pelo outro lado.
  podeAssinar: boolean;
};

const LABEL_PAPEL: Record<AssinaturaSlot["papel"], string> = {
  Engenheiro: "Engenheiro",
  Proprietario: "Proprietário",
};

function AssinaturaStatusCard({
  papel,
  nome,
  assinadoEm,
  hash,
  podeAssinar,
  assinando,
  onAssinar,
}: AssinaturaSlot & { assinando: boolean; onAssinar: () => Promise<void> }) {
  const [open, setOpen] = useState(false);
  const assinado = Boolean(assinadoEm);
  const label = LABEL_PAPEL[papel];

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
          <p className="text-sm font-semibold text-primary">{label}</p>
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
        </div>
      ) : (
        <>
          <p className="text-xs font-medium text-warning-foreground/80">
            {podeAssinar ? "Aguardando sua assinatura" : "Aguardando assinatura"}
          </p>
          {podeAssinar && (
            <Button
              variant="assinar"
              className="min-h-11 w-full"
              disabled={assinando}
              onClick={() => setOpen(true)}
            >
              {assinando ? (
                <Loader2 className="mr-2 size-4 animate-spin" />
              ) : (
                <PenLine className="mr-2 size-4" />
              )}
              {assinando ? "Assinando…" : "Assinar"}
            </Button>
          )}
        </>
      )}

      <SignatureCanvas
        open={open}
        onOpenChange={setOpen}
        papel={label}
        nome={nome}
        onConfirmar={async () => {
          await onAssinar();
          setOpen(false);
        }}
      />
    </Card>
  );
}

export function BlocoAssinaturas({
  slots,
  titulo = "Assinaturas necessárias",
  assinandoPapel,
  onAssinar,
}: {
  slots: AssinaturaSlot[];
  titulo?: string;
  assinandoPapel: string | null;
  onAssinar: (papel: AssinaturaSlot["papel"]) => Promise<void>;
}) {
  return (
    <section className="space-y-3">
      <h3 className="text-base font-semibold text-primary">{titulo}</h3>
      <div className="grid gap-3 md:grid-cols-2">
        {slots.map((slot) => (
          <AssinaturaStatusCard
            key={slot.papel}
            {...slot}
            assinando={assinandoPapel === slot.papel}
            onAssinar={() => onAssinar(slot.papel)}
          />
        ))}
      </div>
    </section>
  );
}
