import { createFileRoute } from "@tanstack/react-router";
import { DocumentoPage } from "@/app/DocumentoPage";

export const Route = createFileRoute("/documentos/$tipo/$id")({
  head: () => ({
    meta: [{ title: "Documento — Caderneta de Obras Digital" }],
  }),
  component: RouteComponent,
});

function RouteComponent() {
  const { tipo, id } = Route.useParams();
  return <DocumentoPage tipo={tipo} id={id} />;
}
