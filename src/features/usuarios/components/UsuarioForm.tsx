import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { useMutation } from "@tanstack/react-query";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import {
  ESPECIALIDADES_ARQUITETURA,
  ESPECIALIDADES_ENGENHARIA,
  TIPOS_USUARIO,
  type TipoUsuario,
} from "@/lib/constants";
import { criarUsuario } from "@/lib/api/usuarios";
import { ApiError } from "@/lib/api/client";
import { formatarCpfInput } from "@/lib/format";
import { AlertCircle, KeyRound, Loader2, MailCheck } from "lucide-react";
import { toast } from "sonner";

const badgeVarianteTipo: Record<TipoUsuario, "default" | "ativa" | "secondary"> = {
  administrador: "default",
  engenheiro: "ativa",
  arquiteto: "ativa",
  proprietario: "secondary",
};

export function UsuarioForm() {
  const navigate = useNavigate();
  const [nome, setNome] = useState("");
  const [cpf, setCpf] = useState("");
  const [email, setEmail] = useState("");
  const [telefone, setTelefone] = useState("");
  const [tipoUsuario, setTipoUsuario] = useState<TipoUsuario | "">("");
  const [especialidade, setEspecialidade] = useState("");
  const [registro, setRegistro] = useState("");
  const [senhaSecreta, setSenhaSecreta] = useState("");
  const [erro, setErro] = useState<string | null>(null);

  const ehEngenheiro = tipoUsuario === "engenheiro";
  const ehArquiteto = tipoUsuario === "arquiteto";
  const ehProfissionalTecnico = ehEngenheiro || ehArquiteto;
  const ehAdministrador = tipoUsuario === "administrador";

  const especialidadesDisponiveis = ehEngenheiro
    ? ESPECIALIDADES_ENGENHARIA
    : ehArquiteto
      ? ESPECIALIDADES_ARQUITETURA
      : [];
  const labelRegistro = ehEngenheiro ? "Número de registro (CREA) *" : "Número de registro (CAU) *";
  const placeholderRegistro = ehEngenheiro ? "CREA-SP 0000000000" : "CAU A000000-0";

  // O título profissional exibido ao Engenheiro/Arquiteto vira o texto
  // livre gravado no backend (ver seção 3.5 do ROADMAP — Arquiteto é um
  // Engenheiro cujo tituloProfissional descreve a especialidade).
  const tituloProfissional = ehArquiteto ? "Arquiteto e Urbanista" : especialidade;

  const mutation = useMutation({
    mutationFn: criarUsuario,
    onSuccess: (resultado) => {
      toast.success(
        `Usuário ${resultado.usuario.nome} cadastrado. Senha provisória: ${resultado.senhaProvisoria} (em produção isso vai por e-mail).`,
        { duration: 10000 },
      );
      navigate({ to: "/usuarios" });
    },
    onError: (err) => {
      // A senha secreta de Administrador é validada no servidor — o
      // front-end nunca deveria ter esse segredo embutido no código.
      setErro(err instanceof ApiError ? err.message : "Não foi possível cadastrar o usuário.");
    },
  });

  function salvar(e: React.FormEvent) {
    e.preventDefault();
    if (!nome.trim() || !cpf.trim() || !email.trim() || !telefone.trim() || !tipoUsuario) {
      setErro("Preencha nome, CPF, e-mail, telefone e o tipo de usuário.");
      return;
    }
    if (ehEngenheiro && (!especialidade || !registro.trim())) {
      setErro("Para Engenheiro, informe também a especialidade e o número de registro (CREA).");
      return;
    }
    if (ehArquiteto && !registro.trim()) {
      setErro("Para Arquiteto, informe também o número de registro (CAU).");
      return;
    }
    if (ehAdministrador && !senhaSecreta.trim()) {
      setErro("Informe a senha secreta de desenvolvedor para cadastrar outro Administrador.");
      return;
    }
    setErro(null);
    mutation.mutate({
      nome: nome.trim(),
      cpf: cpf.trim(),
      email: email.trim(),
      telefone: telefone.trim(),
      tipoUsuario,
      tituloProfissional: ehProfissionalTecnico ? tituloProfissional : null,
      numeroRegistro: ehProfissionalTecnico ? registro.trim() : null,
      senhaSecretaDev: ehAdministrador ? senhaSecreta : null,
    });
  }

  return (
    <form onSubmit={salvar} className="space-y-6" noValidate>
      <Card className="border-border">
        <CardContent className="space-y-6 p-5 md:p-6">
          <h2 className="text-lg font-semibold text-primary">Dados de acesso</h2>
          <div className="grid gap-5 md:grid-cols-2">
            <div className="space-y-2 md:col-span-2">
              <Label htmlFor="nome">Nome completo *</Label>
              <Input
                id="nome"
                className="min-h-11"
                placeholder="Nome completo"
                value={nome}
                onChange={(e) => setNome(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="cpf">CPF *</Label>
              <Input
                id="cpf"
                inputMode="numeric"
                className="min-h-11"
                placeholder="000.000.000-00"
                maxLength={14}
                value={cpf}
                onChange={(e) => setCpf(formatarCpfInput(e.target.value))}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="telefone">Telefone *</Label>
              <Input
                id="telefone"
                inputMode="tel"
                className="min-h-11"
                placeholder="(00) 00000-0000"
                value={telefone}
                onChange={(e) => setTelefone(e.target.value)}
              />
            </div>
            <div className="space-y-2 md:col-span-2">
              <Label htmlFor="email">E-mail *</Label>
              <Input
                id="email"
                type="email"
                className="min-h-11"
                placeholder="usuario@exemplo.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </div>
            <div className="space-y-2 md:col-span-2">
              <Label>Tipo de usuário *</Label>
              <Select
                value={tipoUsuario}
                onValueChange={(v) => {
                  setTipoUsuario(v as TipoUsuario);
                  setEspecialidade("");
                  setRegistro("");
                }}
              >
                <SelectTrigger className="min-h-11 w-full">
                  <SelectValue placeholder="Selecione o tipo de usuário" />
                </SelectTrigger>
                <SelectContent>
                  {TIPOS_USUARIO.map((t) => (
                    <SelectItem key={t.value} value={t.value}>
                      {t.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {tipoUsuario && (
                <div className="flex items-center gap-2 pt-1 text-sm text-muted-foreground">
                  Você está cadastrando:{" "}
                  <Badge variant={badgeVarianteTipo[tipoUsuario]}>
                    {TIPOS_USUARIO.find((t) => t.value === tipoUsuario)?.label}
                  </Badge>
                </div>
              )}
            </div>
          </div>

          {ehProfissionalTecnico && (
            <>
              <Separator />
              <h2 className="text-lg font-semibold text-primary">
                Dados profissionais (obrigatórios para {ehEngenheiro ? "Engenheiro" : "Arquiteto"})
              </h2>
              <div className="grid gap-5 md:grid-cols-2">
                {ehEngenheiro && (
                  <div className="space-y-2">
                    <Label>Especialidade *</Label>
                    <Select value={especialidade} onValueChange={setEspecialidade}>
                      <SelectTrigger className="min-h-11 w-full">
                        <SelectValue placeholder="Selecione a especialidade" />
                      </SelectTrigger>
                      <SelectContent>
                        {especialidadesDisponiveis.map((t) => (
                          <SelectItem key={t} value={t}>
                            {t}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                )}
                <div className="space-y-2">
                  <Label htmlFor="registro">{labelRegistro}</Label>
                  <Input
                    id="registro"
                    className="min-h-11"
                    placeholder={placeholderRegistro}
                    value={registro}
                    onChange={(e) => setRegistro(e.target.value)}
                  />
                </div>
              </div>
            </>
          )}

          {ehAdministrador && (
            <>
              <Separator />
              <h2 className="text-lg font-semibold text-primary">
                Confirmação de segurança (obrigatória para Administrador)
              </h2>
              <div className="grid gap-5 md:grid-cols-2">
                <div className="space-y-2 md:col-span-2">
                  <Label htmlFor="senha-secreta">Senha secreta de desenvolvedor *</Label>
                  <Input
                    id="senha-secreta"
                    type="password"
                    className="min-h-11"
                    placeholder="••••••••••"
                    value={senhaSecreta}
                    onChange={(e) => setSenhaSecreta(e.target.value)}
                  />
                  <p className="flex items-start gap-2 text-xs text-muted-foreground">
                    <KeyRound className="mt-0.5 size-3.5 shrink-0" />
                    Criar outro Administrador do CREA exige essa senha adicional, validada pelo
                    servidor — proteção extra contra criação indevida de contas com acesso total
                    ao sistema.
                  </p>
                </div>
              </div>
            </>
          )}

          <div className="flex items-start gap-2 rounded-md border border-border bg-secondary/50 p-3">
            <MailCheck className="mt-0.5 size-4 shrink-0 text-primary" />
            <p className="text-xs text-muted-foreground">
              O usuário recebe uma senha provisória para o primeiro acesso (hoje exibida na
              tela ao cadastrar — em produção deve ser enviada por e-mail).
            </p>
          </div>
        </CardContent>
      </Card>

      {erro && (
        <div className="flex items-start gap-2 rounded-md border border-destructive/30 bg-destructive/5 p-3">
          <AlertCircle className="mt-0.5 size-4 shrink-0 text-destructive" />
          <p className="text-sm text-destructive">{erro}</p>
        </div>
      )}

      <div className="flex flex-wrap justify-end gap-3">
        <Button
          type="button"
          variant="outline"
          className="min-h-11"
          onClick={() => navigate({ to: "/usuarios" })}
        >
          Cancelar
        </Button>
        <Button type="submit" className="min-h-11 px-6" disabled={mutation.isPending}>
          {mutation.isPending && <Loader2 className="mr-2 size-4 animate-spin" />}
          {mutation.isPending ? "Cadastrando…" : "Cadastrar usuário"}
        </Button>
      </div>
    </form>
  );
}
