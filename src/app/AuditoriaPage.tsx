import { useQuery } from "@tanstack/react-query";
import { PageContainer } from "@/components/layout/PageContainer";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { listarAuditoria } from "@/lib/api/auditoria";
import { formatarDataHora } from "@/lib/format";
import { useRequireAuth } from "@/lib/auth-context";
import { Loader2, ShieldCheck } from "lucide-react";
import { AcessoRestrito } from "./AcessoRestrito";

const VARIANTE_ACAO: Record<string, "default" | "ativa" | "assinado" | "pendente" | "secondary"> = {
  LoginRealizado: "ativa",
  LoginFalhou: "pendente",
  ObraAtivada: "assinado",
  ObraFinalizada: "assinado",
};

export function AuditoriaPage() {
  const { perfil, carregando } = useRequireAuth();

  const { data: logs, isLoading, isError } = useQuery({
    queryKey: ["auditoria"],
    queryFn: () => listarAuditoria(200),
    enabled: perfil === "administrador",
  });

  if (carregando || !perfil) return null;

  if (perfil !== "administrador") {
    return <AcessoRestrito mensagem="Somente o Administrador do CREA pode consultar o log de auditoria." />;
  }

  return (
    <PageContainer
      titulo="Log de auditoria"
      descricao="Registro append-only de ações do sistema — data, hora, usuário, IP e ação executada (RF09). Não pode ser editado nem excluído, nem pelo Administrador."
    >
      {isLoading ? (
        <div className="flex items-center justify-center p-10 text-muted-foreground">
          <Loader2 className="mr-2 size-5 animate-spin" /> Carregando log de auditoria…
        </div>
      ) : isError ? (
        <Card className="border-destructive/30 bg-destructive/5">
          <CardContent className="p-10 text-center text-sm text-destructive">
            Não foi possível carregar o log de auditoria.
          </CardContent>
        </Card>
      ) : !logs || logs.length === 0 ? (
        <Card className="border-dashed">
          <CardContent className="p-10 text-center text-sm text-muted-foreground">
            Nenhum evento registrado ainda.
          </CardContent>
        </Card>
      ) : (
        <Card className="border-border">
          <CardContent className="p-0">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Data/hora</TableHead>
                  <TableHead>Usuário</TableHead>
                  <TableHead>Ação</TableHead>
                  <TableHead>Entidade</TableHead>
                  <TableHead>Detalhes</TableHead>
                  <TableHead>IP</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {logs.map((log) => (
                  <TableRow key={log.id}>
                    <TableCell className="whitespace-nowrap text-xs text-muted-foreground">
                      {formatarDataHora(log.dataHoraUtc)}
                    </TableCell>
                    <TableCell className="text-sm">
                      {log.usuarioNome ?? <span className="text-muted-foreground">—</span>}
                    </TableCell>
                    <TableCell>
                      <Badge variant={VARIANTE_ACAO[log.acao] ?? "secondary"}>{log.acao}</Badge>
                    </TableCell>
                    <TableCell className="text-xs text-muted-foreground">
                      {log.entidadeTipo ? `${log.entidadeTipo} #${log.entidadeId}` : "—"}
                    </TableCell>
                    <TableCell className="max-w-xs truncate text-xs text-muted-foreground" title={log.detalhes ?? undefined}>
                      {log.detalhes ?? "—"}
                    </TableCell>
                    <TableCell className="font-mono text-xs text-muted-foreground">{log.ip}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </CardContent>
        </Card>
      )}

      <p className="mt-4 flex items-center gap-2 text-xs text-muted-foreground">
        <ShieldCheck className="size-3.5" />
        Reforçado por trigger no Postgres — nenhum registro aqui pode ser alterado ou apagado, mesmo por acesso direto ao banco.
      </p>
    </PageContainer>
  );
}
