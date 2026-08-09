import { Link } from "@tanstack/react-router";
import { PageContainer } from "@/components/layout/PageContainer";
import { ObraDetalhe } from "@/features/obras/components/ObraDetalhe";
import { obraPorId, obrasVisiveis } from "@/lib/mock-data";
import { usePerfil } from "@/lib/perfil-context";
import { AcessoRestrito } from "./AcessoRestrito";
import { Button } from "@/components/ui/button";
import { ArrowLeft } from "lucide-react";

export function ObraDetalhePage({ id }: { id: string }) {
  const { perfil } = usePerfil();
  const obra = obraPorId(id);

  if (!obra) {
    return <AcessoRestrito mensagem="Caderneta não encontrada." />;
  }
  if (!obrasVisiveis(perfil).some((o) => o.id === obra.id)) {
    return (
      <AcessoRestrito mensagem="Esta caderneta não está atribuída ao perfil selecionado." />
    );
  }

  return (
    <PageContainer>
      <Button asChild variant="ghost" className="mb-4 min-h-11 -ml-2 text-link">
        <Link to="/dashboard">
          <ArrowLeft className="mr-2 size-4" /> Cadernetas
        </Link>
      </Button>
      <ObraDetalhe obra={obra} />
    </PageContainer>
  );
}
