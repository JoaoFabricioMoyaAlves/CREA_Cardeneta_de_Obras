import { useState } from "react";
import { Link, useNavigate } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent } from "@/components/ui/card";
import { useAuth } from "@/lib/auth-context";
import { ApiError } from "@/lib/api/client";
import { formatarCpfInput } from "@/lib/format";
import { AlertCircle, HardHat, Loader2 } from "lucide-react";

export function LoginForm() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [cpf, setCpf] = useState("");
  const [senha, setSenha] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  async function entrar(e: React.FormEvent) {
    e.preventDefault();
    setErro(null);
    if (!cpf.trim() || !senha.trim()) {
      setErro("Informe CPF e senha para continuar.");
      return;
    }
    setCarregando(true);
    try {
      await login(cpf, senha);
      navigate({ to: "/dashboard" });
    } catch (err) {
      setErro(
        err instanceof ApiError ? err.message : "Não foi possível entrar. Verifique sua conexão.",
      );
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
            Caderneta de Obras Digital
          </h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Acesso restrito a profissionais e proprietários cadastrados
          </p>
        </div>

        <form onSubmit={entrar} className="space-y-5" noValidate>
          <div className="space-y-2">
            <Label htmlFor="cpf">CPF</Label>
            <Input
              id="cpf"
              inputMode="numeric"
              placeholder="000.000.000-00"
              maxLength={14}
              className="min-h-11"
              value={cpf}
              onChange={(e) => setCpf(formatarCpfInput(e.target.value))}
              aria-invalid={Boolean(erro)}
            />
          </div>

          <div className="space-y-2">
            <div className="flex items-center justify-between">
              <Label htmlFor="senha">Senha</Label>
              <Link
                to="/recuperar-senha"
                className="text-sm font-medium text-link hover:underline"
              >
                Esqueceu a senha?
              </Link>
            </div>
            <Input
              id="senha"
              type="password"
              placeholder="••••••••"
              className="min-h-11"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
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
            {carregando ? "Entrando…" : "Entrar"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
