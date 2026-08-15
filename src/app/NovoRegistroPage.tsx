import { Link } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";
import { PageContainer } from "@/components/layout/PageContainer";
import { RegistroForm } from "@/features/registros/components/RegistroForm";
import { obterObra } from "@/lib/api/obras";
import { useRequireAuth } from "@/lib/auth-context";
import { AcessoRestrito } from "./AcessoRestrito";
import { Button } from "@/components/ui/button";
import { ArrowLeft, Loader2 } from "lucide-react";

export function NovoRegistroPage({ id }: { id: string }) {
  const { perfil, usuario, carregando } = useRequireAuth();
  const obraId = Number(id);

  const obraQuery = useQuery({
    queryKey: ["obra", obraId],
    queryFn: () => obterObra(obraId),
    enabled: Boolean(usuario) && !Number.isNaN(obraId),
    retry: false,
  });

  if (carregando || !usuario) return null;

  if (obraQuery.isLoading) {
    return (
      <PageContainer>
        <div className="flex items-center justify-center p-10 text-muted-foreground">
          <Loader2 className="mr-2 size-5 animate-spin" /> Carregando caderneta…
        </div>
      </PageContainer>
    );
  }

  if (obraQuery.isError || !obraQuery.data) {
    return <AcessoRestrito mensagem="Caderneta não encontrada." />;
  }

  const obra = obraQuery.data;

  if (perfil !== "engenheiro" || usuario.id !== obra.profissionalId) {
    return (
      <AcessoRestrito mensagem="Somente o Engenheiro/Arquiteto responsável pode lançar registros de visita." />
    );
  }

  if (obra.status !== "Ativa") {
    return (
      <AcessoRestrito mensagem="A caderneta precisa estar ativa (dupla assinatura de abertura concluída) para receber registros de visita." />
    );
  }

  return (
    <PageContainer titulo="Novo registro de visita" descricao={`${obra.numeroCaderneta} — ${obra.localObra}`}>
      <Button asChild variant="ghost" className="mb-4 min-h-11 -ml-2 text-link">
        <Link to="/obras/$id" params={{ id: String(obra.id) }}>
          <ArrowLeft className="mr-2 size-4" /> Voltar à caderneta
        </Link>
      </Button>
      <RegistroForm obra={obra} />
    </PageContainer>
  );
}
