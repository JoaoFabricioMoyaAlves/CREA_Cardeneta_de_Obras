import { PageContainer } from "@/components/layout/PageContainer";
import { UsuarioForm } from "@/features/usuarios/components/UsuarioForm";
import { useRequireAuth } from "@/lib/auth-context";
import { AcessoRestrito } from "./AcessoRestrito";

export function NovoUsuarioPage() {
  const { perfil, carregando } = useRequireAuth();

  if (carregando || !perfil) return null;

  if (perfil === "engenheiro") {
    return (
      <PageContainer
        titulo="Cadastrar proprietário"
        descricao="Cadastre o proprietário da obra. Engenheiros e arquitetos só podem cadastrar usuários do tipo Proprietário."
      >
        <UsuarioForm somenteProprietario />
      </PageContainer>
    );
  }

  if (perfil !== "administrador") {
    return (
      <AcessoRestrito mensagem="Somente o Administrador do CREA ou um Engenheiro/Arquiteto podem cadastrar novos usuários." />
    );
  }

  return (
    <PageContainer
      titulo="Cadastrar usuário"
      descricao="Escolha o tipo de usuário: Administrador, Engenheiro, Arquiteto ou Proprietário. Cada tipo pede dados adicionais próprios."
    >
      <UsuarioForm />
    </PageContainer>
  );
}
