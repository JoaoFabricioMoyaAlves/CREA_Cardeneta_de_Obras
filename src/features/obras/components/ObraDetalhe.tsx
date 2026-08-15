import { Link } from "@tanstack/react-router";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { formatarData, formatarMoeda } from "@/lib/format";
import { assinarObra } from "@/lib/api/obras";
import { assinarTermo } from "@/lib/api/termos";
import { ApiError } from "@/lib/api/client";
import { useAuth } from "@/lib/auth-context";
import type { AssinaturaSlot } from "@/features/assinatura/components/AssinaturaStatusCard";
import type { ObraResponse, RegistroResponse, TermoResponse } from "@/lib/api/types";
import { StatusBadge } from "./ObraCard";
import { RegistroCard } from "@/features/registros/components/RegistroCard";
import { BlocoAssinaturas } from "@/features/assinatura/components/AssinaturaStatusCard";
import { CheckCircle2, FileText, MapPin, Plus, ScrollText } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

export function ObraDetalhe({
  obra,
  registros,
  termo,
}: {
  obra: ObraResponse;
  registros: RegistroResponse[];
  termo: TermoResponse | null;
}) {
  const { usuario, perfil } = useAuth();
  const queryClient = useQueryClient();
  const [assinandoPapelObra, setAssinandoPapelObra] = useState<string | null>(null);
  const [assinandoPapelTermo, setAssinandoPapelTermo] = useState<string | null>(null);

  const ehEngenheiroResponsavel = perfil === "engenheiro" && usuario?.id === obra.profissionalId;

  function invalidar() {
    queryClient.invalidateQueries({ queryKey: ["obra", obra.id] });
    queryClient.invalidateQueries({ queryKey: ["termo", obra.id] });
    queryClient.invalidateQueries({ queryKey: ["obras"] });
  }

  const assinarObraMutation = useMutation({
    mutationFn: () => assinarObra(obra.id),
    onSuccess: () => {
      toast.success("Assinatura registrada.");
      invalidar();
    },
    onError: (err) => {
      toast.error(err instanceof ApiError ? err.message : "Não foi possível registrar a assinatura.");
    },
  });

  const assinarTermoMutation = useMutation({
    mutationFn: () => assinarTermo(termo!.id),
    onSuccess: () => {
      toast.success("Assinatura do termo registrada.");
      invalidar();
    },
    onError: (err) => {
      toast.error(err instanceof ApiError ? err.message : "Não foi possível registrar a assinatura.");
    },
  });

  async function assinarAberturaObra(papel: AssinaturaSlot["papel"]) {
    setAssinandoPapelObra(papel);
    try {
      await assinarObraMutation.mutateAsync();
    } finally {
      setAssinandoPapelObra(null);
    }
  }

  async function assinarTermoConclusao(papel: AssinaturaSlot["papel"]) {
    setAssinandoPapelTermo(papel);
    try {
      await assinarTermoMutation.mutateAsync();
    } finally {
      setAssinandoPapelTermo(null);
    }
  }

  const assinaturaEngenheiro = obra.assinaturas.find((a) => a.papel === "Engenheiro");
  const assinaturaProprietario = obra.assinaturas.find((a) => a.papel === "Proprietario");

  const slotsObra: AssinaturaSlot[] = [
    {
      papel: "Engenheiro",
      nome: obra.nomeProfissional,
      assinadoEm: assinaturaEngenheiro?.data ?? null,
      hash: assinaturaEngenheiro?.hash ?? null,
      podeAssinar:
        obra.status === "PendenteAssinatura" &&
        !assinaturaEngenheiro &&
        perfil === "engenheiro" &&
        usuario?.id === obra.profissionalId,
    },
    {
      papel: "Proprietario",
      nome: obra.nomeProprietario,
      assinadoEm: assinaturaProprietario?.data ?? null,
      hash: assinaturaProprietario?.hash ?? null,
      podeAssinar:
        obra.status === "PendenteAssinatura" &&
        !assinaturaProprietario &&
        perfil === "proprietario" &&
        usuario?.id === obra.proprietarioId,
    },
  ];

  const termoAssEngenheiro = termo?.assinaturas.find((a) => a.papel === "Engenheiro");
  const termoAssProprietario = termo?.assinaturas.find((a) => a.papel === "Proprietario");

  const slotsTermo: AssinaturaSlot[] = termo
    ? [
        {
          papel: "Engenheiro",
          nome: obra.nomeProfissional,
          assinadoEm: termoAssEngenheiro?.data ?? null,
          hash: termoAssEngenheiro?.hash ?? null,
          podeAssinar:
            termo.status === "PendenteAssinatura" &&
            !termoAssEngenheiro &&
            perfil === "engenheiro" &&
            usuario?.id === obra.profissionalId,
        },
        {
          papel: "Proprietario",
          nome: obra.nomeProprietario,
          assinadoEm: termoAssProprietario?.data ?? null,
          hash: termoAssProprietario?.hash ?? null,
          podeAssinar:
            termo.status === "PendenteAssinatura" &&
            !termoAssProprietario &&
            perfil === "proprietario" &&
            usuario?.id === obra.proprietarioId,
        },
      ]
    : [];

  const itensArea = [
    ["Construir", obra.areaConstruirM2],
    ["Ampliar", obra.areaAmpliarM2],
    ["Reformar", obra.areaReformarM2],
    ["Regularizar", obra.areaRegularizarM2],
  ] as const;

  return (
    <div className="space-y-6">
      <Card className="border-border bg-secondary/40">
        <CardContent className="space-y-5 p-5 md:p-6">
          <div className="flex flex-wrap items-start justify-between gap-4">
            <div>
              <p className="font-mono text-xs tracking-wide text-link">{obra.numeroCaderneta}</p>
              <h1 className="mt-1 flex items-start gap-2 text-xl font-bold text-primary md:text-2xl">
                <MapPin className="mt-1 size-5 shrink-0" />
                {obra.localObra}
              </h1>
              <p className="mt-1 pl-7 text-sm text-muted-foreground">{obra.cidade}</p>
            </div>
            <div className="flex flex-col items-end gap-3">
              <StatusBadge status={obra.status} />
              <div className="flex flex-wrap gap-2">
                {ehEngenheiroResponsavel && obra.status === "Ativa" && (
                  <Button asChild className="min-h-11">
                    <Link to="/obras/$id/registros/novo" params={{ id: String(obra.id) }}>
                      <Plus className="mr-1.5 size-4" /> Novo registro
                    </Link>
                  </Button>
                )}
                {ehEngenheiroResponsavel && !termo && (
                  <Button
                    asChild={obra.status === "Ativa"}
                    variant="destaque"
                    className="min-h-11"
                    disabled={obra.status !== "Ativa"}
                  >
                    {obra.status === "Ativa" ? (
                      <Link to="/obras/$id/finalizar" params={{ id: String(obra.id) }}>
                        <CheckCircle2 className="mr-1.5 size-4" /> Finalizar caderneta
                      </Link>
                    ) : (
                      <span>
                        <CheckCircle2 className="mr-1.5 inline size-4" /> Finalizar caderneta
                      </span>
                    )}
                  </Button>
                )}
              </div>
            </div>
          </div>

          <div className="grid gap-4 border-t border-border pt-4 md:grid-cols-3">
            <Info titulo="Responsável técnico" valor={obra.nomeProfissional} />
            <Info titulo="Proprietário" valor={obra.nomeProprietario} />
            <Info titulo="Número RT" valor={obra.numeroRt} />
            <Info titulo="Tipo de edificação" valor={obra.tipoEdificacao} />
            <Info titulo="Valor da obra" valor={formatarMoeda(obra.valorObra)} />
            <Info titulo="Data do recibo" valor={formatarData(obra.dataReciboAbertura)} />
            <Info titulo="Atividade técnica" valor={obra.atividadesTecnicas.join(", ") || "—"} />
            <Info titulo="Empresa" valor={obra.nomeEmpresa ?? "Não informada"} />
          </div>

          <div className="grid gap-3 border-t border-border pt-4 sm:grid-cols-2 lg:grid-cols-5">
            {itensArea.map(([label, valor]) => (
              <div key={label} className="rounded-md bg-card p-3">
                <p className="text-xs text-muted-foreground">{label}</p>
                <p className="text-base font-semibold text-foreground">
                  {valor.toLocaleString("pt-BR")} m²
                </p>
              </div>
            ))}
            <div className="rounded-md bg-primary p-3 text-primary-foreground">
              <p className="text-xs text-white/75">Área total</p>
              <p className="text-base font-bold">
                {obra.areaTotalEdificadaM2.toLocaleString("pt-BR")} m²
              </p>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card className="border-border">
        <CardContent className="p-5 md:p-6">
          <BlocoAssinaturas
            titulo="Assinaturas da caderneta"
            slots={slotsObra}
            assinandoPapel={assinandoPapelObra}
            onAssinar={assinarAberturaObra}
          />
        </CardContent>
      </Card>

      {termo && (
        <Card className="border-border">
          <CardContent className="space-y-4 p-5 md:p-6">
            <h2 className="flex items-center gap-2 text-lg font-semibold text-primary">
              <ScrollText className="size-5" /> Termo de conclusão
              <Badge variant={termo.status === "Assinado" ? "assinado" : "pendente"}>
                {termo.status === "Assinado" ? "Assinado" : "Pendente assinatura"}
              </Badge>
            </h2>
            <p className="text-sm text-muted-foreground">
              Conclusão em {formatarData(termo.dataConclusao)}
            </p>
            <p className="text-sm whitespace-pre-line text-foreground">{termo.declaracao}</p>
            <BlocoAssinaturas
              titulo="Assinaturas do termo"
              slots={slotsTermo}
              assinandoPapel={assinandoPapelTermo}
              onAssinar={assinarTermoConclusao}
            />
          </CardContent>
        </Card>
      )}

      <section className="space-y-4">
        <div className="flex items-center justify-between">
          <h2 className="flex items-center gap-2 text-lg font-semibold text-primary">
            <FileText className="size-5" /> Registros de visita
            <Badge variant="secondary">{registros.length}</Badge>
          </h2>
        </div>
        {registros.length === 0 ? (
          <Card className="border-dashed">
            <CardContent className="p-8 text-center text-sm text-muted-foreground">
              Nenhum registro de visita lançado nesta caderneta.
            </CardContent>
          </Card>
        ) : (
          <div className="grid gap-4 lg:grid-cols-2">
            {registros.map((r) => (
              <RegistroCard key={r.id} registro={r} obraId={obra.id} />
            ))}
          </div>
        )}
      </section>
    </div>
  );
}

function Info({ titulo, valor }: { titulo: string; valor: string }) {
  return (
    <div>
      <p className="text-xs tracking-wide text-muted-foreground uppercase">{titulo}</p>
      <p className="text-sm font-medium text-foreground">{valor}</p>
    </div>
  );
}
