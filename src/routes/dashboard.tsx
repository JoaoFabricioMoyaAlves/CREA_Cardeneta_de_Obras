import { createFileRoute } from "@tanstack/react-router";
import { DashboardPage } from "@/app/DashboardPage";

export const Route = createFileRoute("/dashboard")({
  head: () => ({
    meta: [
      { title: "Cadernetas de obra — Caderneta de Obras Digital" },
      {
        name: "description",
        content:
          "Painel com todas as cadernetas de obra, status de assinatura, responsável técnico e proprietário.",
      },
      { property: "og:title", content: "Cadernetas de obra" },
      {
        property: "og:description",
        content: "Acompanhe status, responsáveis e áreas de cada caderneta de obra.",
      },
    ],
  }),
  component: DashboardPage,
});
