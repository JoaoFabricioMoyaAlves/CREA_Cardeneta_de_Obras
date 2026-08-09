import { createFileRoute } from "@tanstack/react-router";
import { FinalizarCadernetaPage } from "@/app/FinalizarCadernetaPage";

export const Route = createFileRoute("/obras/$id/finalizar")({
  head: () => ({
    meta: [
      { title: "Finalizar caderneta — Caderneta de Obras Digital" },
      {
        name: "description",
        content:
          "Emissão do termo de conclusão da obra com data, declaração técnica e dupla assinatura.",
      },
      { property: "og:title", content: "Finalizar caderneta" },
      {
        property: "og:description",
        content: "Termo de conclusão da obra com assinatura do engenheiro e do proprietário.",
      },
    ],
  }),
  component: RouteComponent,
});

function RouteComponent() {
  const { id } = Route.useParams();
  return <FinalizarCadernetaPage id={id} />;
}
