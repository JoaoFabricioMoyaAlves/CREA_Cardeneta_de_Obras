import { Link } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";
import { PageContainer } from "@/components/layout/PageContainer";
import { RegistroDetalhe } from "@/features/registros/components/RegistroDetalhe";
import { obterObra } from "@/lib/api/obras";
import { obterRegistro } from "@/lib/api/registros";
import { useRequireAuth } from "@/lib/auth-context";
import { ApiError } from "@/lib/api/client";
import { AcessoRestrito } from "./AcessoRestrito";
import { Button } from "@/components/ui/button";
import { ArrowLeft, Loader2 } from "lucide-react";

export function RegistroDetalhePage({ id, registroId }: { id: string; registroId: string }) {
  const { carregando, usuario } = useRequireAuth();
  const obraId = Number(id);
  const regId = Number(registroId);

  const obraQuery = useQuery({
    queryKey: ["obra", obraId],
    queryFn: () => obterObra(obraId),
    enabled: Boolean(usuario) && !Number.isNaN(obraId),
    retry: false,
  });

  const registroQuery = useQuery({
    queryKey: ["registro", regId],
    queryFn: () => obterRegistro(regId),
    enabled: Boolean(usuario) && !Number.isNaN(regId),
    retry: false,
  });

  if (carregando || !usuario) return null;

  if (obraQuery.isLoading || registroQuery.isLoading) {
    return (
      <PageContainer>
        <div className="flex items-center justify-center p-10 text-muted-foreground">
          <Loader2 className="mr-2 size-5 animate-spin" /> Carregando registro…
        </div>
      </PageContainer>
    );
  }

  if (obraQuery.isError || registroQuery.isError || !obraQuery.data || !registroQuery.data) {
    const erro = obraQuery.error ?? registroQuery.error;
    const mensagem = erro instanceof ApiError ? erro.message : "Registro de visita não encontrado.";
    return <AcessoRestrito mensagem={mensagem} />;
  }

  const obra = obraQuery.data;
  const registro = registroQuery.data;

  if (registro.obraId !== obra.id) {
    return <AcessoRestrito mensagem="Registro de visita não encontrado." />;
  }

  return (
    <PageContainer titulo="Registro de visita" descricao={`${obra.numeroCaderneta} — ${obra.localObra}`}>
      <Button asChild variant="ghost" className="mb-4 min-h-11 -ml-2 text-link">
        <Link to="/obras/$id" params={{ id: String(obra.id) }}>
          <ArrowLeft className="mr-2 size-4" /> Voltar à caderneta
        </Link>
      </Button>
      <RegistroDetalhe registro={registro} obra={obra} />
    </PageContainer>
  );
}
