import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { ImageOff, Loader2 } from "lucide-react";

export function ImagemPreview({ registroId, imagemId, nome }: { registroId: number; imagemId: number; nome: string }) {
  const [open, setOpen] = useState(false);
  const { data, isLoading, isError } = useQuery({
    queryKey: ["imagemUrl", registroId, imagemId],
    queryFn: () => api.get<{ url: string }>(`/api/registros/${registroId}/imagens/${imagemId}/url`),
    staleTime: 10 * 60 * 1000, // a URL presignada do MinIO vale 15min no backend
  });

  if (isLoading) {
    return (
      <div className="flex aspect-4/3 w-full items-center justify-center rounded-lg border border-border bg-secondary/40">
        <Loader2 className="size-5 animate-spin text-muted-foreground" />
      </div>
    );
  }

  if (isError || !data) {
    return (
      <div className="flex aspect-4/3 w-full flex-col items-center justify-center gap-1 rounded-lg border border-border bg-secondary/40 text-muted-foreground">
        <ImageOff className="size-5" />
        <span className="text-xs">Falha ao carregar</span>
      </div>
    );
  }

  return (
    <>
      <button
        type="button"
        onClick={() => setOpen(true)}
        className="block aspect-4/3 w-full cursor-zoom-in overflow-hidden rounded-lg border border-border focus:outline-none focus-visible:ring-2 focus-visible:ring-ring"
      >
        <img
          src={data.url}
          alt={nome}
          loading="lazy"
          className="size-full object-cover transition-transform hover:scale-105"
        />
      </button>

      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-w-3xl border-none bg-transparent p-0 shadow-none [&>button]:text-white [&>button]:opacity-80 [&>button]:hover:opacity-100">
          <DialogTitle className="sr-only">{nome}</DialogTitle>
          <img src={data.url} alt={nome} className="max-h-[85vh] w-full rounded-lg object-contain" />
        </DialogContent>
      </Dialog>
    </>
  );
}
