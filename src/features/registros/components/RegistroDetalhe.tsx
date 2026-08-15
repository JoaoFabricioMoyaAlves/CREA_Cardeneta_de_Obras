import { useRef, useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { FASES_SERVICO } from "@/lib/constants";
import { formatarData } from "@/lib/format";
import { assinarRegistro, adicionarImagem } from "@/lib/api/registros";
import { ApiError } from "@/lib/api/client";
import { useAuth } from "@/lib/auth-context";
import type { ObraResponse, RegistroResponse } from "@/lib/api/types";
import type { AssinaturaSlot } from "@/features/assinatura/components/AssinaturaStatusCard";
import { BlocoAssinaturas } from "@/features/assinatura/components/AssinaturaStatusCard";
import { ImagemPreview } from "./ImagemPreview";
import { CalendarDays, Check, Circle, ImagePlus, Loader2 } from "lucide-react";
import { toast } from "sonner";

export function RegistroDetalhe({ registro, obra }: { registro: RegistroResponse; obra: ObraResponse }) {
  const { usuario, perfil } = useAuth();
  const queryClient = useQueryClient();
  const [assinandoPapel, setAssinandoPapel] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const ehEngenheiroResponsavel = perfil === "engenheiro" && usuario?.id === obra.profissionalId;
  const podeAdicionarFoto = ehEngenheiroResponsavel && registro.status === "PendenteAssinatura";

  function invalidar() {
    queryClient.invalidateQueries({ queryKey: ["registro", registro.id] });
    queryClient.invalidateQueries({ queryKey: ["registros", obra.id] });
  }

  const assinarMutation = useMutation({
    mutationFn: () => assinarRegistro(registro.id),
    onSuccess: () => {
      toast.success("Assinatura registrada.");
      invalidar();
    },
    onError: (err) => {
      toast.error(err instanceof ApiError ? err.message : "Não foi possível registrar a assinatura.");
    },
  });

  const uploadMutation = useMutation({
    mutationFn: (arquivo: File) => adicionarImagem(registro.id, arquivo),
    onSuccess: () => {
      toast.success("Foto adicionada.");
      invalidar();
    },
    onError: (err) => {
      toast.error(err instanceof ApiError ? err.message : "Não foi possível enviar a foto.");
    },
  });

  async function assinar(papel: AssinaturaSlot["papel"]) {
    setAssinandoPapel(papel);
    try {
      await assinarMutation.mutateAsync();
    } finally {
      setAssinandoPapel(null);
    }
  }

  function selecionarArquivo(e: React.ChangeEvent<HTMLInputElement>) {
    const arquivo = e.target.files?.[0];
    if (arquivo) uploadMutation.mutate(arquivo);
    e.target.value = "";
  }

  const assinaturaEngenheiro = registro.assinaturas.find((a) => a.papel === "Engenheiro");
  const assinaturaProprietario = registro.assinaturas.find((a) => a.papel === "Proprietario");

  const slots: AssinaturaSlot[] = [
    {
      papel: "Engenheiro",
      nome: obra.nomeProfissional,
      assinadoEm: assinaturaEngenheiro?.data ?? null,
      hash: assinaturaEngenheiro?.hash ?? null,
      podeAssinar:
        registro.status === "PendenteAssinatura" &&
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
        registro.status === "PendenteAssinatura" &&
        !assinaturaProprietario &&
        perfil === "proprietario" &&
        usuario?.id === obra.proprietarioId,
    },
  ];

  return (
    <div className="space-y-6">
      <Card className="border-border bg-secondary/40">
        <CardContent className="flex flex-wrap items-start justify-between gap-4 p-5 md:p-6">
          <div>
            <p className="flex items-center gap-2 text-lg font-bold text-primary">
              <CalendarDays className="size-5" />
              Visita de {formatarData(registro.dataVisita)}
            </p>
            <p className="mt-1 text-sm text-muted-foreground">
              Posição da obra: {registro.posicaoObra}
            </p>
          </div>
          <Badge variant={registro.status === "Assinado" ? "assinado" : "pendente"}>
            {registro.status === "Assinado" ? "Assinado" : "Pendente assinatura"}
          </Badge>
        </CardContent>
      </Card>

      <Card className="border-border">
        <CardContent className="space-y-2 p-5 md:p-6">
          <h2 className="text-base font-semibold text-primary">Decisões e orientações</h2>
          <p className="text-sm leading-relaxed whitespace-pre-line text-foreground">
            {registro.decisoesOrientacoes}
          </p>
        </CardContent>
      </Card>

      <Card className="border-border">
        <CardContent className="space-y-4 p-5 md:p-6">
          <h2 className="text-base font-semibold text-primary">Fases de serviço</h2>
          <div className="grid gap-2 md:grid-cols-2 xl:grid-cols-3">
            {FASES_SERVICO.map((f) => {
              const concluida = registro.fases.includes(f);
              return (
                <div
                  key={f}
                  className={
                    concluida
                      ? "flex min-h-11 items-center gap-3 rounded-md bg-secondary px-4 py-2"
                      : "flex min-h-11 items-center gap-3 rounded-md border border-border px-4 py-2"
                  }
                >
                  {concluida ? (
                    <span className="flex size-5 items-center justify-center rounded-full bg-primary text-primary-foreground">
                      <Check className="size-3" />
                    </span>
                  ) : (
                    <Circle className="size-5 text-muted-foreground/50" />
                  )}
                  <span
                    className={
                      concluida
                        ? "text-sm font-medium text-primary"
                        : "text-sm text-muted-foreground"
                    }
                  >
                    {f}
                  </span>
                </div>
              );
            })}
          </div>
        </CardContent>
      </Card>

      <Card className="border-border">
        <CardContent className="space-y-4 p-5 md:p-6">
          <div className="flex items-center justify-between">
            <h2 className="text-base font-semibold text-primary">
              Galeria de fotos ({registro.imagens.length})
            </h2>
            {podeAdicionarFoto && (
              <>
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/*"
                  className="hidden"
                  onChange={selecionarArquivo}
                />
                <Button
                  type="button"
                  size="sm"
                  variant="outline"
                  disabled={uploadMutation.isPending}
                  onClick={() => fileInputRef.current?.click()}
                >
                  {uploadMutation.isPending ? (
                    <Loader2 className="mr-1.5 size-4 animate-spin" />
                  ) : (
                    <ImagePlus className="mr-1.5 size-4" />
                  )}
                  Adicionar foto
                </Button>
              </>
            )}
          </div>
          {registro.imagens.length > 0 ? (
            <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
              {registro.imagens.map((img) => (
                <ImagemPreview key={img.id} registroId={registro.id} imagemId={img.id} nome={img.name} />
              ))}
            </div>
          ) : (
            <p className="text-sm text-muted-foreground">Nenhuma foto anexada ainda.</p>
          )}
        </CardContent>
      </Card>

      <Card className="border-border">
        <CardContent className="p-5 md:p-6">
          <BlocoAssinaturas
            titulo="Assinaturas"
            slots={slots}
            assinandoPapel={assinandoPapel}
            onAssinar={assinar}
          />
        </CardContent>
      </Card>
    </div>
  );
}
