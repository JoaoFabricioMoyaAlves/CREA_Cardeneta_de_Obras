import { createFileRoute } from "@tanstack/react-router";
import { NovoRegistroPage } from "@/app/NovoRegistroPage";

export const Route = createFileRoute("/obras/$id/registros/novo")({
  head: () => ({
    meta: [
      { title: "Novo registro de visita — Caderneta de Obras Digital" },
      {
        name: "description",
        content:
          "Lance o relato da visita técnica: posição da obra, decisões, fases executadas e fotos.",
      },
      { property: "og:title", content: "Novo registro de visita" },
      {
        property: "og:description",
        content: "Relato de visita técnica com fases de serviço, fotos e dupla assinatura.",
      },
    ],
  }),
  component: RouteComponent,
});

function RouteComponent() {
  const { id } = Route.useParams();
  return <NovoRegistroPage id={id} />;
}
