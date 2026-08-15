import { Link } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { PageContainer } from "@/components/layout/PageContainer";
import { UsuarioCard } from "@/features/usuarios/components/UsuarioCard";
import { listarUsuarios } from "@/lib/api/usuarios";
import { useRequireAuth } from "@/lib/auth-context";
import { Loader2, UserPlus } from "lucide-react";
import { AcessoRestrito } from "./AcessoRestrito";

export function UsuariosPage() {
  const { perfil, carregando } = useRequireAuth();

  const { data: usuarios, isLoading, isError } = useQuery({
    queryKey: ["usuarios"],
    queryFn: listarUsuarios,
    enabled: perfil === "administrador",
  });

  if (carregando || !perfil) return null;

  if (perfil !== "administrador") {
    return (
      <AcessoRestrito mensagem="Somente o Administrador do CREA pode gerenciar o cadastro de usuários." />
    );
  }

  return (
    <PageContainer
      titulo="Usuários"
      descricao="Administradores, engenheiros/arquitetos e proprietários com acesso ao sistema."
      acoes={
        <Button asChild variant="destaque" className="min-h-12 px-6 text-base">
          <Link to="/usuarios/novo">
            <UserPlus className="mr-2 size-5" /> Novo usuário
          </Link>
        </Button>
      }
    >
      {isLoading ? (
        <div className="flex items-center justify-center p-10 text-muted-foreground">
          <Loader2 className="mr-2 size-5 animate-spin" /> Carregando usuários…
        </div>
      ) : isError ? (
        <Card className="border-destructive/30 bg-destructive/5">
          <CardContent className="p-10 text-center text-sm text-destructive">
            Não foi possível carregar os usuários.
          </CardContent>
        </Card>
      ) : !usuarios || usuarios.length === 0 ? (
        <Card className="border-dashed">
          <CardContent className="p-10 text-center text-sm text-muted-foreground">
            Nenhum usuário cadastrado.
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {usuarios.map((usuario) => (
            <UsuarioCard key={usuario.id} usuario={usuario} />
          ))}
        </div>
      )}
    </PageContainer>
  );
}
