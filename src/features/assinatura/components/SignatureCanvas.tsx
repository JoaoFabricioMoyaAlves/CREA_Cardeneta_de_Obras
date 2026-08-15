import { useRef, useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Eraser, Loader2 } from "lucide-react";

type Props = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  papel: string;
  nome: string;
  onConfirmar: () => Promise<void>;
};

export function SignatureCanvas({ open, onOpenChange, papel, nome, onConfirmar }: Props) {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const desenhando = useRef(false);
  const [temTraco, setTemTraco] = useState(false);
  const [enviando, setEnviando] = useState(false);

  function pos(e: React.PointerEvent<HTMLCanvasElement>) {
    const canvas = canvasRef.current!;
    const rect = canvas.getBoundingClientRect();
    return {
      x: ((e.clientX - rect.left) / rect.width) * canvas.width,
      y: ((e.clientY - rect.top) / rect.height) * canvas.height,
    };
  }

  function start(e: React.PointerEvent<HTMLCanvasElement>) {
    const ctx = canvasRef.current?.getContext("2d");
    if (!ctx) return;
    desenhando.current = true;
    setTemTraco(true);
    const p = pos(e);
    ctx.lineWidth = 2.5;
    ctx.lineCap = "round";
    ctx.strokeStyle = "#1E3A8A";
    ctx.beginPath();
    ctx.moveTo(p.x, p.y);
  }

  function move(e: React.PointerEvent<HTMLCanvasElement>) {
    if (!desenhando.current) return;
    const ctx = canvasRef.current?.getContext("2d");
    if (!ctx) return;
    const p = pos(e);
    ctx.lineTo(p.x, p.y);
    ctx.stroke();
  }

  function end() {
    desenhando.current = false;
  }

  function limpar() {
    const canvas = canvasRef.current;
    const ctx = canvas?.getContext("2d");
    if (!canvas || !ctx) return;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    setTemTraco(false);
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-xl">
        <DialogHeader>
          <DialogTitle>Assinatura do {papel}</DialogTitle>
          <DialogDescription>
            {nome} — desenhe sua assinatura no quadro abaixo. O traço é só a representação
            visual; o que dá validade é o hash gerado pelo servidor ao confirmar.
          </DialogDescription>
        </DialogHeader>

        <div className="rounded-lg border-2 border-dashed border-border bg-background p-2">
          <canvas
            ref={canvasRef}
            width={640}
            height={240}
            onPointerDown={start}
            onPointerMove={move}
            onPointerUp={end}
            onPointerLeave={end}
            className="h-48 w-full touch-none rounded-md bg-card"
          />
          <div className="mt-2 flex items-center justify-between px-1">
            <span className="text-xs text-muted-foreground">Assine acima da linha</span>
            <Button variant="ghost" size="sm" onClick={limpar} disabled={enviando}>
              <Eraser className="mr-1.5 size-4" /> Limpar
            </Button>
          </div>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            className="min-h-11"
            disabled={enviando}
            onClick={() => onOpenChange(false)}
          >
            Cancelar
          </Button>
          <Button
            variant="assinar"
            className="min-h-11"
            disabled={!temTraco || enviando}
            onClick={async () => {
              setEnviando(true);
              try {
                await onConfirmar();
                limpar();
              } finally {
                setEnviando(false);
              }
            }}
          >
            {enviando && <Loader2 className="mr-2 size-4 animate-spin" />}
            {enviando ? "Confirmando…" : "Confirmar assinatura"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
