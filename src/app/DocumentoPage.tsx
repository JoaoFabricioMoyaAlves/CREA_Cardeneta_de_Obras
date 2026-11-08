import { useQuery } from "@tanstack/react-query";
import { useRouter } from "@tanstack/react-router";
import { ArrowLeft, Loader2, Printer } from "lucide-react";
import { Button } from "@/components/ui/button";
import { ehTipoDocumento, ModeloDocumento } from "@/features/documentos/modelos";
import { ApiError } from "@/lib/api/client";
import { obterDocumento, type TipoDocumentoApi } from "@/lib/api/documentos";
import { useRequireAuth } from "@/lib/auth-context";

export function DocumentoPage({ tipo, id }: { tipo: string; id: string }) {
  const { carregando, usuario } = useRequireAuth();
  const router = useRouter();
  const referenciaId = Number(id);
  const valido = ehTipoDocumento(tipo) && !Number.isNaN(referenciaId);

  const query = useQuery({
    queryKey: ["documento", tipo, referenciaId],
    queryFn: () => obterDocumento(tipo as TipoDocumentoApi, referenciaId),
    enabled: Boolean(usuario) && valido,
    retry: false,
  });

  if (carregando || !usuario) return null;

  return (
    <div className="doc-pagina min-h-screen bg-muted">
      <div className="doc-nao-imprimir flex items-center justify-between gap-3 border-b border-border bg-background p-3">
        <Button variant="outline" className="min-h-11" onClick={() => router.history.back()}>
          <ArrowLeft className="mr-2 size-4" /> Voltar
        </Button>
        <Button className="min-h-11" disabled={!query.data} onClick={() => window.print()}>
          <Printer className="mr-2 size-4" /> Imprimir / salvar PDF
        </Button>
      </div>

      {!valido && <p className="p-6 text-sm text-destructive">Documento não encontrado.</p>}
      {query.isLoading && (
        <div className="flex items-center justify-center p-10 text-muted-foreground">
          <Loader2 className="mr-2 size-5 animate-spin" /> Carregando documento…
        </div>
      )}
      {query.isError && (
        <p className="p-6 text-sm text-destructive">
          {query.error instanceof ApiError
            ? query.error.message
            : "Não foi possível abrir o documento."}
        </p>
      )}
      {query.data && <ModeloDocumento documento={query.data} />}
    </div>
  );
}
