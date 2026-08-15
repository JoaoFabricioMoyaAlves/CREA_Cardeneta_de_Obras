import { createFileRoute } from "@tanstack/react-router";
import { UsuariosPage } from "@/app/UsuariosPage";

export const Route = createFileRoute("/usuarios")({
  head: () => ({
    meta: [
      { title: "Usuários — Caderneta de Obras Digital" },
      {
        name: "description",
        content: "Gestão de usuários do sistema: administradores, engenheiros/arquitetos e proprietários.",
      },
      { property: "og:title", content: "Usuários — Caderneta de Obras Digital" },
      {
        property: "og:description",
        content: "Acesso restrito ao Administrador do CREA.",
      },
    ],
  }),
  component: UsuariosPage,
});
