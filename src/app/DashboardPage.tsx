import { Link } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { PageContainer } from "@/components/layout/PageContainer";
import { ObraCard } from "@/features/obras/components/ObraCard";
import { listarObras } from "@/lib/api/obras";
import type { StatusObraApi } from "@/lib/api/types";
import { useRequireAuth } from "@/lib/auth-context";
import { Loader2, Plus } from "lucide-react";
import { useState } from "react";

const filtros: { valor: "Todas" | StatusObraApi; label: string }[] = [
  { valor: "Todas", label: "Todas" },
  { valor: "PendenteAssinatura", label: "Pendente assinatura" },
  { valor: "Ativa", label: "Ativa" },
  { valor: "Finalizada", label: "Finalizada" },
];

export function DashboardPage() {
  const { perfil, carregando: carregandoAuth } = useRequireAuth();
  const [filtro, setFiltro] = useState<string>("Todas");

  const { data: obras, isLoading, isError } = useQuery({
    queryKey: ["obras"],
    queryFn: listarObras,
    enabled: Boolean(perfil),
  });

  if (carregandoAuth || !perfil) return null;

  const filtradas = obras?.filter((o) => filtro === "Todas" || o.status === filtro) ?? [];

  const descricao =
    perfil === "administrador"
      ? "Todas as cadernetas registradas no conselho."
      : perfil === "engenheiro"
        ? "Cadernetas sob sua responsabilidade técnica."
        : "Cadernetas do seu imóvel — acesso para consulta e assinatura.";

  return (
    <PageContainer
      titulo="Cadernetas de obra"
      descricao={descricao}
      acoes={
        perfil === "administrador" ? (
          <Button asChild variant="destaque" className="min-h-12 px-6 text-base">
            <Link to="/obras/nova">
              <Plus className="mr-2 size-5" /> Criar nova obra
            </Link>
          </Button>
        ) : null
      }
    >
      <Tabs value={filtro} onValueChange={setFiltro} className="mb-6">
        <TabsList className="h-auto flex-wrap">
          {filtros.map((f) => (
            <TabsTrigger key={f.valor} value={f.valor} className="min-h-10 px-4">
              {f.label}
            </TabsTrigger>
          ))}
        </TabsList>
      </Tabs>

      {isLoading ? (
        <div className="flex items-center justify-center p-10 text-muted-foreground">
          <Loader2 className="mr-2 size-5 animate-spin" /> Carregando cadernetas…
        </div>
      ) : isError ? (
        <Card className="border-destructive/30 bg-destructive/5">
          <CardContent className="p-10 text-center text-sm text-destructive">
            Não foi possível carregar as cadernetas. Verifique se a API está no ar.
          </CardContent>
        </Card>
      ) : filtradas.length === 0 ? (
        <Card className="border-dashed">
          <CardContent className="p-10 text-center text-sm text-muted-foreground">
            Nenhuma caderneta encontrada para este filtro.
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {filtradas.map((obra) => (
            <ObraCard key={obra.id} obra={obra} />
          ))}
        </div>
      )}
    </PageContainer>
  );
}
