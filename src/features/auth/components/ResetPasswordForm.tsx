import { useState } from "react";
import { Link } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent } from "@/components/ui/card";
import { AlertCircle, ArrowLeft, CheckCircle2, HardHat, Loader2 } from "lucide-react";
import { redefinirSenha } from "@/lib/api/auth";
import { ApiError } from "@/lib/api/client";

const TAMANHO_MINIMO_SENHA = 8;

// Usado tanto no primeiro acesso (link do e-mail de boas-vindas) quanto na
// recuperação de senha — os dois chegam aqui com um token de uso único.
export function ResetPasswordForm({ token, primeiroAcesso }: { token: string; primeiroAcesso: boolean }) {
  const [senha, setSenha] = useState("");
  const [confirmacao, setConfirmacao] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);
  const [concluido, setConcluido] = useState(false);

  async function salvar(e: React.FormEvent) {
    e.preventDefault();
    setErro(null);
    if (senha.length < TAMANHO_MINIMO_SENHA) {
      setErro(`A senha precisa ter pelo menos ${TAMANHO_MINIMO_SENHA} caracteres.`);
      return;
    }
    if (senha !== confirmacao) {
      setErro("As senhas digitadas não são iguais.");
      return;
    }
    setCarregando(true);
    try {
      await redefinirSenha(token, senha);
      setConcluido(true);
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Não foi possível salvar a senha. Tente novamente.");
    } finally {
      setCarregando(false);
    }
  }

  return (
    <Card className="w-full max-w-md border-border shadow-lg">
      <CardContent className="p-6 md:p-8">
        <div className="mb-8 text-center">
          <span className="mx-auto mb-4 flex size-14 items-center justify-center rounded-xl bg-primary text-primary-foreground">
            <HardHat className="size-7" />
          </span>
          <h1 className="text-xl font-bold tracking-tight text-primary md:text-2xl">
            {primeiroAcesso ? "Criar sua senha" : "Definir nova senha"}
          </h1>
          <p className="mt-1 text-sm text-muted-foreground">
            {primeiroAcesso
              ? "Bem-vindo! Escolha a senha que você vai usar para entrar com o seu CPF."
              : "Escolha uma nova senha para a sua conta."}
          </p>
        </div>

        {!token ? (
          <div className="space-y-5">
            <div className="flex items-start gap-2 rounded-md border border-destructive/30 bg-destructive/5 p-3">
              <AlertCircle className="mt-0.5 size-4 shrink-0 text-destructive" />
              <p className="text-sm text-destructive">
                Link incompleto. Abra novamente o link recebido por e-mail ou solicite um novo.
              </p>
            </div>
            <Link
              to="/recuperar-senha"
              className="flex items-center justify-center gap-2 text-sm font-medium text-link hover:underline"
            >
              Solicitar novo link
            </Link>
          </div>
        ) : concluido ? (
          <div className="space-y-5">
            <div className="flex items-start gap-3 rounded-md border border-border bg-secondary/60 p-4">
              <CheckCircle2 className="mt-0.5 size-5 shrink-0 text-primary" />
              <p className="text-sm text-foreground">
                Senha salva com sucesso. Agora você já pode entrar com o seu CPF e a nova senha.
              </p>
            </div>
            <Button asChild className="min-h-12 w-full text-base">
              <Link to="/">Ir para o login</Link>
            </Button>
          </div>
        ) : (
          <form onSubmit={salvar} className="space-y-5" noValidate>
            <div className="space-y-2">
              <Label htmlFor="senha">Nova senha</Label>
              <Input
                id="senha"
                type="password"
                autoComplete="new-password"
                className="min-h-11"
                placeholder={`Mínimo de ${TAMANHO_MINIMO_SENHA} caracteres`}
                value={senha}
                onChange={(e) => setSenha(e.target.value)}
                aria-invalid={Boolean(erro)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="confirmacao">Confirme a nova senha</Label>
              <Input
                id="confirmacao"
                type="password"
                autoComplete="new-password"
                className="min-h-11"
                value={confirmacao}
                onChange={(e) => setConfirmacao(e.target.value)}
                aria-invalid={Boolean(erro)}
              />
            </div>

            {erro && (
              <div className="flex items-start gap-2 rounded-md border border-destructive/30 bg-destructive/5 p-3">
                <AlertCircle className="mt-0.5 size-4 shrink-0 text-destructive" />
                <p className="text-sm text-destructive">{erro}</p>
              </div>
            )}

            <Button type="submit" className="min-h-12 w-full text-base" disabled={carregando}>
              {carregando && <Loader2 className="mr-2 size-4 animate-spin" />}
              {carregando ? "Salvando…" : "Salvar senha"}
            </Button>

            <Link
              to="/"
              className="flex items-center justify-center gap-2 text-sm font-medium text-link hover:underline"
            >
              <ArrowLeft className="size-4" />
              Voltar para o login
            </Link>
          </form>
        )}
      </CardContent>
    </Card>
  );
}
