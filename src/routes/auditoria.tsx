import { createFileRoute } from "@tanstack/react-router";
import { AuditoriaPage } from "@/app/AuditoriaPage";

export const Route = createFileRoute("/auditoria")({
  head: () => ({
    meta: [
      { title: "Log de auditoria — Caderneta de Obras Digital" },
      {
        name: "description",
        content: "Registro append-only de ações do sistema — data, hora, usuário, IP e ação executada.",
      },
      { property: "og:title", content: "Log de auditoria — Caderneta de Obras Digital" },
      {
        property: "og:description",
        content: "Acesso restrito ao Administrador do CREA.",
      },
    ],
  }),
  component: AuditoriaPage,
});
