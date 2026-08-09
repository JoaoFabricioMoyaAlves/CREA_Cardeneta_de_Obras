import { Link } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { PageContainer } from "@/components/layout/PageContainer";
import { ObraCard } from "@/features/obras/components/ObraCard";
import { obrasVisiveis } from "@/lib/mock-data";
import { usePerfil } from "@/lib/perfil-context";
import { Plus } from "lucide-react";
import { useState } from "react";
import type { StatusObra } from "@/lib/constants";

const filtros: ("Todas" | StatusObra)[] = [
  "Todas",
  "Pendente assinatura",
  "Ativa",
  "Finalizada",
];

export function DashboardPage() {
  const { perfil } = usePerfil();
  const [filtro, setFiltro] = useState<string>("Todas");
  const lista = obrasVisiveis(perfil);
  const filtradas = filtro === "Todas" ? lista : lista.filter((o) => o.status === filtro);

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
            <TabsTrigger key={f} value={f} className="min-h-10 px-4">
              {f}
            </TabsTrigger>
          ))}
        </TabsList>
      </Tabs>

      {filtradas.length === 0 ? (
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
