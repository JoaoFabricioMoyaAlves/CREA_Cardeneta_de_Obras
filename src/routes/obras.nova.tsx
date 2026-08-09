import { createFileRoute } from "@tanstack/react-router";
import { NovaObraPage } from "@/app/NovaObraPage";

export const Route = createFileRoute("/obras/nova")({
  head: () => ({
    meta: [
      { title: "Criar nova obra — Caderneta de Obras Digital" },
      {
        name: "description",
        content:
          "Abertura de caderneta de obra: responsáveis, número RT, áreas, atividade técnica e assinaturas.",
      },
      { property: "og:title", content: "Criar nova obra" },
      {
        property: "og:description",
        content: "Formulário de abertura de caderneta com dupla assinatura.",
      },
    ],
  }),
  component: NovaObraPage,
});
