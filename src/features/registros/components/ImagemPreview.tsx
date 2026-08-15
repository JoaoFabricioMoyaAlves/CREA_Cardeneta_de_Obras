import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api/client";
import { ImageOff, Loader2 } from "lucide-react";

export function ImagemPreview({ registroId, imagemId, nome }: { registroId: number; imagemId: number; nome: string }) {
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
    <img
      src={data.url}
      alt={nome}
      loading="lazy"
      className="aspect-4/3 w-full rounded-lg border border-border object-cover"
    />
  );
}
