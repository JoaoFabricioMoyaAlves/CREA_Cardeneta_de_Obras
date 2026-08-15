import { useState } from "react";
import { Link } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent } from "@/components/ui/card";
import { ArrowLeft, HardHat, Loader2, MailCheck } from "lucide-react";

export function ForgotPasswordForm() {
  const [email, setEmail] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);
  const [enviado, setEnviado] = useState(false);

  function enviarCodigo(e: React.FormEvent) {
    e.preventDefault();
    setErro(null);
    if (!email.trim()) {
      setErro("Informe seu e-mail cadastrado.");
      return;
    }
    setCarregando(true);
    setTimeout(() => {
      setCarregando(false);
      setEnviado(true);
    }, 900);
  }

  return (
    <Card className="w-full max-w-md border-border shadow-lg">
      <CardContent className="p-6 md:p-8">
        <div className="mb-8 text-center">
          <span className="mx-auto mb-4 flex size-14 items-center justify-center rounded-xl bg-primary text-primary-foreground">
            <HardHat className="size-7" />
          </span>
          <h1 className="text-xl font-bold tracking-tight text-primary md:text-2xl">
            Recuperar senha
          </h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Informe seu e-mail cadastrado para receber um código de recuperação
          </p>
        </div>

        {enviado ? (
          <div className="space-y-5">
            <div className="flex items-start gap-3 rounded-md border border-border bg-secondary/60 p-4">
              <MailCheck className="mt-0.5 size-5 shrink-0 text-primary" />
              <p className="text-sm text-foreground">
                Se o e-mail informado estiver cadastrado, você receberá em instantes um código de
                recuperação para redefinir sua senha.
              </p>
            </div>
            <Link
              to="/"
              className="flex items-center justify-center gap-2 text-sm font-medium text-link hover:underline"
            >
              <ArrowLeft className="size-4" />
              Voltar para o login
            </Link>
          </div>
        ) : (
          <form onSubmit={enviarCodigo} className="space-y-5" noValidate>
            <div className="space-y-2">
              <Label htmlFor="email">E-mail</Label>
              <Input
                id="email"
                type="email"
                placeholder="seuemail@exemplo.com"
                className="min-h-11"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                aria-invalid={Boolean(erro)}
              />
            </div>

            {erro && <p className="text-sm text-destructive">{erro}</p>}

            <Button type="submit" className="min-h-12 w-full text-base" disabled={carregando}>
              {carregando && <Loader2 className="mr-2 size-4 animate-spin" />}
              {carregando ? "Enviando…" : "Enviar código de recuperação"}
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
