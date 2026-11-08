import { useState } from "react";
import { useMutation, useQuery } from "@tanstack/react-query";
import { AlertCircle, Loader2, PenLine } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { ApiError } from "@/lib/api/client";
import { assinarDocumento, obterDocumento, type TipoDocumentoApi } from "@/lib/api/documentos";
import { formatarCpfInput } from "@/lib/format";
import { ModeloDocumento } from "./modelos";

const HTTP_CONFLITO = 409;

export function AssinarDocumentoDialog({
  open,
  onOpenChange,
  tipo,
  referenciaId,
  onAssinado,
}: {
  open: boolean;
  onOpenChange: (aberto: boolean) => void;
  tipo: TipoDocumentoApi;
  referenciaId: number;
  onAssinado: () => void;
}) {
  const [cpf, setCpf] = useState("");
  const [senha, setSenha] = useState("");
  const [erro, setErro] = useState<string | null>(null);

  const documento = useQuery({
    queryKey: ["documento", tipo, referenciaId],
    queryFn: () => obterDocumento(tipo, referenciaId),
    enabled: open,
    retry: false,
    // Quem vai assinar precisa ver a versão atual, nunca uma cópia antiga.
    staleTime: 0,
    refetchOnMount: "always",
  });

  function fechar(aberto: boolean) {
    if (!aberto) {
      setCpf("");
      setSenha("");
      setErro(null);
    }
    onOpenChange(aberto);
  }

  const assinatura = useMutation({
    mutationFn: () =>
      assinarDocumento(tipo, referenciaId, {
        cpf,
        senha,
        hashDocumentoVisualizado: documento.data!.hashDocumento,
      }),
    onSuccess: () => {
      toast.success("Documento assinado.");
      fechar(false);
      onAssinado();
    },
    onError: (err) => {
      setSenha("");
      setErro(err instanceof ApiError ? err.message : "Não foi possível registrar a assinatura.");
      // O documento mudou ou outra pessoa assinou: mostra a versão atual.
      if (err instanceof ApiError && err.status === HTTP_CONFLITO) void documento.refetch();
    },
  });

  function enviar(e: React.FormEvent) {
    e.preventDefault();
    if (!cpf.trim() || !senha) {
      setErro("Informe seu CPF e sua senha para assinar.");
      return;
    }
    setErro(null);
    assinatura.mutate();
  }

  return (
    <Dialog open={open} onOpenChange={fechar}>
      <DialogContent className="flex max-h-[95vh] max-w-[calc(100vw-1rem)] flex-col gap-3 overflow-hidden p-4 lg:max-w-[920px]">
        <DialogHeader>
          <DialogTitle>Leia o documento antes de assinar</DialogTitle>
          <DialogDescription>
            A assinatura fica vinculada a este documento, ao seu usuário, à data e hora e ao seu IP.
          </DialogDescription>
        </DialogHeader>

        <div className="min-h-0 flex-1 overflow-y-auto rounded-md border border-border">
          {documento.isLoading && (
            <div className="flex items-center justify-center p-10 text-muted-foreground">
              <Loader2 className="mr-2 size-5 animate-spin" /> Carregando documento…
            </div>
          )}
          {documento.isError && (
            <p className="p-6 text-sm text-destructive">
              {documento.error instanceof ApiError
                ? documento.error.message
                : "Não foi possível abrir o documento."}
            </p>
          )}
          {documento.data && <ModeloDocumento documento={documento.data} />}
        </div>

        <form onSubmit={enviar} className="space-y-3" noValidate>
          <div className="grid gap-3 md:grid-cols-[1fr_1fr_auto] md:items-end">
            <div className="space-y-1.5">
              <Label htmlFor="assinar-cpf">CPF</Label>
              <Input
                id="assinar-cpf"
                inputMode="numeric"
                autoComplete="off"
                className="min-h-11"
                placeholder="000.000.000-00"
                maxLength={14}
                value={cpf}
                onChange={(e) => setCpf(formatarCpfInput(e.target.value))}
              />
            </div>
            <div className="space-y-1.5">
              <Label htmlFor="assinar-senha">Senha</Label>
              <Input
                id="assinar-senha"
                type="password"
                autoComplete="off"
                className="min-h-11"
                placeholder="••••••••"
                value={senha}
                onChange={(e) => setSenha(e.target.value)}
              />
            </div>
            <Button
              type="submit"
              variant="assinar"
              className="min-h-11"
              disabled={!documento.data || assinatura.isPending}
            >
              {assinatura.isPending ? (
                <Loader2 className="mr-2 size-4 animate-spin" />
              ) : (
                <PenLine className="mr-2 size-4" />
              )}
              {assinatura.isPending ? "Assinando…" : "Assinar documento"}
            </Button>
          </div>
          {erro && (
            <p className="flex items-start gap-2 text-sm text-destructive" role="alert">
              <AlertCircle className="mt-0.5 size-4 shrink-0" />
              {erro}
            </p>
          )}
        </form>
      </DialogContent>
    </Dialog>
  );
}
