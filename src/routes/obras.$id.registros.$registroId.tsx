import { createFileRoute } from "@tanstack/react-router";
import { RegistroDetalhePage } from "@/app/RegistroDetalhePage";

export const Route = createFileRoute("/obras/$id/registros/$registroId")({
  head: () => ({
    meta: [
      { title: "Registro de visita — Caderneta de Obras Digital" },
      {
        name: "description",
        content:
          "Detalhe do relato de visita: decisões técnicas, fases concluídas, galeria de fotos e assinaturas verificadas.",
      },
      { property: "og:title", content: "Registro de visita" },
      {
        property: "og:description",
        content: "Relato de visita técnica assinado, com fotos e fases concluídas.",
      },
    ],
  }),
  component: RouteComponent,
});

function RouteComponent() {
  const { id, registroId } = Route.useParams();
  return <RegistroDetalhePage id={id} registroId={registroId} />;
}
