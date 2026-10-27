import { createFileRoute } from "@tanstack/react-router";
import { ResetPasswordPage } from "@/app/ResetPasswordPage";

type RedefinirSenhaSearch = {
  token: string;
  primeiroAcesso: boolean;
};

export const Route = createFileRoute("/redefinir-senha")({
  validateSearch: (search: Record<string, unknown>): RedefinirSenhaSearch => ({
    token: typeof search["token"] === "string" ? search["token"] : "",
    primeiroAcesso: [true, 1, "true", "1"].includes(search["primeiroAcesso"] as never),
  }),
  head: () => ({
    meta: [
      { title: "Definir senha — Caderneta de Obras Digital" },
      { name: "description", content: "Crie ou redefina a senha de acesso à sua conta." },
      { name: "robots", content: "noindex" },
    ],
  }),
  component: RedefinirSenhaRoute,
});

function RedefinirSenhaRoute() {
  const { token, primeiroAcesso } = Route.useSearch();
  return <ResetPasswordPage token={token} primeiroAcesso={primeiroAcesso} />;
}
