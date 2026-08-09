import { PageContainer } from "@/components/layout/PageContainer";
import { ObraForm } from "@/features/obras/components/ObraForm";
import { usePerfil } from "@/lib/perfil-context";
import { AcessoRestrito } from "./AcessoRestrito";

export function NovaObraPage() {
  const { perfil } = usePerfil();

  if (perfil !== "administrador") {
    return (
      <AcessoRestrito mensagem="Somente o Administrador do CREA pode abrir novas cadernetas de obra." />
    );
  }

  return (
    <PageContainer
      titulo="Criar nova obra"
      descricao="Abertura de caderneta e atribuição de responsáveis técnicos."
    >
      <ObraForm />
    </PageContainer>
  );
}
