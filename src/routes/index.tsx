import { createFileRoute } from "@tanstack/react-router";
import { LoginPage } from "@/app/LoginPage";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "Entrar — Caderneta de Obras Digital" },
      {
        name: "description",
        content:
          "Acesso ao sistema de acompanhamento técnico de obras do CREA: registros de visita, assinaturas e termo de conclusão.",
      },
      { property: "og:title", content: "Entrar — Caderneta de Obras Digital" },
      {
        property: "og:description",
        content: "Acesso restrito a profissionais e proprietários cadastrados.",
      },
    ],
  }),
  component: LoginPage,
});
