import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { Bot, Send } from "lucide-react";

export function AssistentePanel({
  open,
  onOpenChange,
}: {
  open: boolean;
  onOpenChange: (v: boolean) => void;
}) {
  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent className="flex w-full flex-col sm:max-w-md">
        <SheetHeader>
          <SheetTitle className="flex items-center gap-2 text-primary">
            <Bot className="size-5" /> Assistente IA
            <Badge variant="pendente">em breve</Badge>
          </SheetTitle>
          <SheetDescription>
            Consulta inteligente às suas cadernetas de obra.
          </SheetDescription>
        </SheetHeader>

        <div className="flex-1 space-y-4 overflow-y-auto px-4">
          <div className="flex gap-3">
            <div className="flex size-9 shrink-0 items-center justify-center rounded-full bg-secondary text-primary">
              <Bot className="size-5" />
            </div>
            <div className="rounded-lg rounded-tl-none bg-secondary/60 p-3 text-sm text-foreground">
              Olá! Posso te ajudar a consultar informações das suas obras.
            </div>
          </div>
          <div className="rounded-lg border border-dashed border-border p-3 text-xs text-muted-foreground">
            Exemplos de perguntas: “Quais registros estão pendentes de assinatura?”,
            “Qual a área total da caderneta CAD-2026-0148?”
          </div>
        </div>

        <div className="border-t border-border p-4">
          <div className="flex items-center gap-2">
            <Input disabled placeholder="Digite sua pergunta…" className="min-h-11" />
            <Button disabled className="min-h-11 px-4">
              <Send className="size-4" />
            </Button>
          </div>
          <p className="mt-2 text-xs text-muted-foreground">
            Interface de demonstração — o agente será conectado futuramente.
          </p>
        </div>
      </SheetContent>
    </Sheet>
  );
}
