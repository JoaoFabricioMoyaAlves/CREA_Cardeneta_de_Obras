import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { BadgeCheck, Clock, PenLine } from "lucide-react";
import { SignatureCanvas } from "./SignatureCanvas";
import { toast } from "sonner";

export type AssinaturaStatus = {
  papel: string;
  nome: string;
  assinadoEm: string | null;
  hash: string | null;
};

export function AssinaturaStatusCard({
  papel,
  nome,
  assinadoEm,
  hash,
  podeAssinar = true,
}: AssinaturaStatus & { podeAssinar?: boolean }) {
  const [open, setOpen] = useState(false);
  const [assinado, setAssinado] = useState(Boolean(assinadoEm));
  const [quando, setQuando] = useState(assinadoEm);

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
          <p className="text-sm font-semibold text-primary">{papel}</p>
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
          <p className="text-xs font-medium text-primary">Assinado em {quando}</p>
          {hash && (
            <p className="font-mono text-xs text-muted-foreground">
              hash {hash.slice(0, 6)}…{hash.slice(-4)}
            </p>
          )}
        </div>
      ) : (
        <>
          <p className="text-xs font-medium text-warning-foreground/80">
            Aguardando assinatura
          </p>
          {podeAssinar && (
            <Button
              variant="assinar"
              className="min-h-11 w-full"
              onClick={() => setOpen(true)}
            >
              <PenLine className="mr-2 size-4" /> Assinar
            </Button>
          )}
        </>
      )}

      <SignatureCanvas
        open={open}
        onOpenChange={setOpen}
        papel={papel}
        nome={nome}
        onConfirmar={() => {
          setAssinado(true);
          setQuando(new Date().toLocaleString("pt-BR").slice(0, 16));
          toast.success(`Assinatura do ${papel.toLowerCase()} registrada (demonstração).`);
        }}
      />
    </Card>
  );
}

export function BlocoAssinaturas({
  assinaturas,
  titulo = "Assinaturas necessárias",
  podeAssinar = true,
}: {
  assinaturas: AssinaturaStatus[];
  titulo?: string;
  podeAssinar?: boolean;
}) {
  return (
    <section className="space-y-3">
      <h3 className="text-base font-semibold text-primary">{titulo}</h3>
      <div className="grid gap-3 md:grid-cols-2">
        {assinaturas.map((a) => (
          <AssinaturaStatusCard key={a.papel} {...a} podeAssinar={podeAssinar} />
        ))}
      </div>
    </section>
  );
}
