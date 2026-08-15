import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import type { PerfilApi, UsuarioResponse } from "@/lib/api/types";
import { Mail, Phone, ShieldCheck } from "lucide-react";

const perfilInfo: Record<PerfilApi, { label: string; variant: "default" | "ativa" | "secondary" }> = {
  Administrador: { label: "Administrador (CREA)", variant: "default" },
  Engenheiro: { label: "Engenheiro/Arquiteto", variant: "ativa" },
  Proprietario: { label: "Proprietário", variant: "secondary" },
};

export function PerfilBadge({ perfil }: { perfil: PerfilApi }) {
  const info = perfilInfo[perfil];
  return <Badge variant={info.variant}>{info.label}</Badge>;
}

export function UsuarioCard({ usuario }: { usuario: UsuarioResponse }) {
  return (
    <Card className="border-border">
      <CardContent className="p-5">
        <div className="flex items-start justify-between gap-3">
          <div>
            <h3 className="text-base font-semibold text-primary">{usuario.nome}</h3>
            <p className="text-xs text-muted-foreground">{usuario.cpf}</p>
          </div>
          <PerfilBadge perfil={usuario.perfil} />
        </div>

        <div className="mt-4 space-y-2 rounded-md bg-secondary/50 p-3 text-sm">
          <p className="flex items-center gap-2 text-foreground">
            <Mail className="size-4 text-primary" />
            {usuario.email}
          </p>
          <p className="flex items-center gap-2 text-foreground">
            <Phone className="size-4 text-primary" />
            {usuario.telefone}
          </p>
          {usuario.numeroRegistro && (
            <p className="flex items-center gap-2 text-foreground">
              <ShieldCheck className="size-4 text-primary" />
              {usuario.tituloProfissional} — {usuario.numeroRegistro}
            </p>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
