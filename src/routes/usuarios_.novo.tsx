import { createFileRoute } from "@tanstack/react-router";
import { NovoUsuarioPage } from "@/app/NovoUsuarioPage";

export const Route = createFileRoute("/usuarios_/novo")({
  head: () => ({
    meta: [
      { title: "Cadastrar usuário — Caderneta de Obras Digital" },
      {
        name: "description",
        content: "Cadastro de novo usuário: administrador, engenheiro/arquiteto ou proprietário.",
      },
      { property: "og:title", content: "Cadastrar usuário — Caderneta de Obras Digital" },
      {
        property: "og:description",
        content: "Acesso restrito ao Administrador do CREA.",
      },
    ],
  }),
  component: NovoUsuarioPage,
});
