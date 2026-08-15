import { createFileRoute } from "@tanstack/react-router";
import { ForgotPasswordPage } from "@/app/ForgotPasswordPage";

export const Route = createFileRoute("/recuperar-senha")({
  head: () => ({
    meta: [
      { title: "Recuperar senha — Caderneta de Obras Digital" },
      {
        name: "description",
        content: "Recupere o acesso à sua conta informando o e-mail cadastrado.",
      },
      { property: "og:title", content: "Recuperar senha — Caderneta de Obras Digital" },
      {
        property: "og:description",
        content: "Recupere o acesso à sua conta informando o e-mail cadastrado.",
      },
    ],
  }),
  component: ForgotPasswordPage,
});
