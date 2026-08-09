import { Link } from "@tanstack/react-router";
import { PageContainer } from "@/components/layout/PageContainer";
import { RegistroDetalhe } from "@/features/registros/components/RegistroDetalhe";
import { obraPorId, obrasVisiveis, registroPorId } from "@/lib/mock-data";
import { usePerfil } from "@/lib/perfil-context";
import { AcessoRestrito } from "./AcessoRestrito";
import { Button } from "@/components/ui/button";
import { ArrowLeft } from "lucide-react";

export function RegistroDetalhePage({
  id,
  registroId,
}: {
  id: string;
  registroId: string;
}) {
  const { perfil } = usePerfil();
  const obra = obraPorId(id);
  const registro = registroPorId(registroId);

  if (!obra || !registro || registro.obraId !== obra.id) {
    return <AcessoRestrito mensagem="Registro de visita não encontrado." />;
  }
  if (!obrasVisiveis(perfil).some((o) => o.id === obra.id)) {
    return <AcessoRestrito mensagem="Este registro não está disponível para o perfil selecionado." />;
  }

  return (
    <PageContainer titulo="Registro de visita" descricao={`${obra.numero} — ${obra.local}`}>
      <Button asChild variant="ghost" className="mb-4 min-h-11 -ml-2 text-link">
        <Link to="/obras/$id" params={{ id: obra.id }}>
          <ArrowLeft className="mr-2 size-4" /> Voltar à caderneta
        </Link>
      </Button>
      <RegistroDetalhe registro={registro} />
    </PageContainer>
  );
}
