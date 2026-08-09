import { createFileRoute } from "@tanstack/react-router";
import { ObraDetalhePage } from "@/app/ObraDetalhePage";

export const Route = createFileRoute("/obras/$id/")({
  head: () => ({
    meta: [
      { title: "Caderneta da obra — Caderneta de Obras Digital" },
      {
        name: "description",
        content:
          "Dados da obra, responsáveis, áreas, assinaturas e histórico de registros de visita.",
      },
      { property: "og:title", content: "Caderneta da obra" },
      {
        property: "og:description",
        content: "Consulte registros de visita, fases executadas e assinaturas da obra.",
      },
    ],
  }),
  component: RouteComponent,
});

function RouteComponent() {
  const { id } = Route.useParams();
  return <ObraDetalhePage id={id} />;
}
