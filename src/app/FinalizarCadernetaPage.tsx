import { Link } from "@tanstack/react-router";
import { PageContainer } from "@/components/layout/PageContainer";
import { TermoConclusaoForm } from "@/features/termo-conclusao/components/TermoConclusaoForm";
import { obraPorId } from "@/lib/mock-data";
import { usePerfil } from "@/lib/perfil-context";
import { AcessoRestrito } from "./AcessoRestrito";
import { Button } from "@/components/ui/button";
import { ArrowLeft } from "lucide-react";

export function FinalizarCadernetaPage({ id }: { id: string }) {
  const { perfil } = usePerfil();
  const obra = obraPorId(id);

  if (!obra) return <AcessoRestrito mensagem="Caderneta não encontrada." />;
  if (perfil !== "engenheiro") {
    return (
      <AcessoRestrito mensagem="Somente o Engenheiro/Arquiteto responsável pode emitir o termo de conclusão." />
    );
  }

  return (
    <PageContainer
      titulo="Finalizar caderneta"
      descricao={`Termo de conclusão — ${obra.numero}`}
    >
      <Button asChild variant="ghost" className="mb-4 min-h-11 -ml-2 text-link">
        <Link to="/obras/$id" params={{ id: obra.id }}>
          <ArrowLeft className="mr-2 size-4" /> Voltar à caderneta
        </Link>
      </Button>
      <TermoConclusaoForm obra={obra} />
    </PageContainer>
  );
}
