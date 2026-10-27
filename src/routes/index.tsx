import { createFileRoute } from "@tanstack/react-router";
import { LoginPage } from "@/app/LoginPage";

type LoginSearch = {
  // Página para onde voltar após o login (ex.: link de um e-mail de assinatura).
  redirect?: string;
};

export const Route = createFileRoute("/")({
  validateSearch: (search: Record<string, unknown>): LoginSearch => {
    const redirect = search["redirect"];
    // Só caminhos internos: evita usar o login para redirecionar a outro site.
    return typeof redirect === "string" && redirect.startsWith("/") && !redirect.startsWith("//")
      ? { redirect }
      : {};
  },
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
