import { Link } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";
import { PageContainer } from "@/components/layout/PageContainer";
import { ObraDetalhe } from "@/features/obras/components/ObraDetalhe";
import { obterObra } from "@/lib/api/obras";
import { listarRegistrosPorObra } from "@/lib/api/registros";
import { obterTermoPorObra } from "@/lib/api/termos";
import { useRequireAuth } from "@/lib/auth-context";
import { ApiError } from "@/lib/api/client";
import { AcessoRestrito } from "./AcessoRestrito";
import { Button } from "@/components/ui/button";
import { ArrowLeft, Loader2 } from "lucide-react";

export function ObraDetalhePage({ id }: { id: string }) {
  const { carregando: carregandoAuth, usuario } = useRequireAuth();
  const obraId = Number(id);

  const obraQuery = useQuery({
    queryKey: ["obra", obraId],
    queryFn: () => obterObra(obraId),
    enabled: Boolean(usuario) && !Number.isNaN(obraId),
    retry: false,
  });

  const registrosQuery = useQuery({
    queryKey: ["registros", obraId],
    queryFn: () => listarRegistrosPorObra(obraId),
    enabled: obraQuery.isSuccess,
  });

  const termoQuery = useQuery({
    queryKey: ["termo", obraId],
    queryFn: () => obterTermoPorObra(obraId),
    enabled: obraQuery.isSuccess,
  });

  if (carregandoAuth || !usuario) return null;

  if (obraQuery.isLoading) {
    return (
      <PageContainer>
        <div className="flex items-center justify-center p-10 text-muted-foreground">
          <Loader2 className="mr-2 size-5 animate-spin" /> Carregando caderneta…
        </div>
      </PageContainer>
    );
  }

  if (obraQuery.isError) {
    const mensagem =
      obraQuery.error instanceof ApiError
        ? obraQuery.error.message
        : "Não foi possível carregar esta caderneta.";
    return <AcessoRestrito mensagem={mensagem} />;
  }

  if (!obraQuery.data) return null;

  return (
    <PageContainer>
      <Button asChild variant="ghost" className="mb-4 min-h-11 -ml-2 text-link">
        <Link to="/dashboard">
          <ArrowLeft className="mr-2 size-4" /> Cadernetas
        </Link>
      </Button>
      <ObraDetalhe
        obra={obraQuery.data}
        registros={registrosQuery.data ?? []}
        termo={termoQuery.data ?? null}
      />
    </PageContainer>
  );
}
