import { useMemo, useState } from "react";
import { Check, ChevronsUpDown } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import type { UsuarioResponse } from "@/lib/api/types";
import { cn } from "@/lib/utils";

const LIMITE_RESULTADOS = 50;

// Sem acento e sem diferença de maiúsculas: "joao" encontra "João".
function normalizar(texto: string): string {
  return texto.normalize("NFD").replace(/\p{M}/gu, "").toLowerCase().trim();
}

function filtrarProprietarios(proprietarios: UsuarioResponse[], busca: string): UsuarioResponse[] {
  const termo = normalizar(busca);
  const digitos = busca.replace(/\D/g, "");

  return proprietarios
    .filter(
      (u) =>
        !termo ||
        normalizar(u.nome).includes(termo) ||
        (digitos.length > 0 && u.cpf.replace(/\D/g, "").includes(digitos)),
    )
    .sort((a, b) => a.nome.localeCompare(b.nome, "pt-BR"));
}

export function ProprietarioSelect({
  id,
  proprietarios,
  carregando,
  valor,
  onChange,
}: {
  id?: string;
  proprietarios: UsuarioResponse[];
  carregando: boolean;
  valor: string;
  onChange: (id: string) => void;
}) {
  const [aberto, setAberto] = useState(false);
  const [busca, setBusca] = useState("");

  const resultados = useMemo(
    () => filtrarProprietarios(proprietarios, busca),
    [proprietarios, busca],
  );
  const visiveis = resultados.slice(0, LIMITE_RESULTADOS);
  const selecionado = proprietarios.find((u) => u.id === valor);

  function alternar(proximo: boolean) {
    setAberto(proximo);
    if (!proximo) setBusca("");
  }

  return (
    <Popover open={aberto} onOpenChange={alternar}>
      <PopoverTrigger asChild>
        <Button
          id={id}
          type="button"
          variant="outline"
          role="combobox"
          aria-expanded={aberto}
          className="min-h-11 w-full justify-between font-normal"
        >
          <span className={cn("truncate", !selecionado && "text-muted-foreground")}>
            {selecionado ? `${selecionado.nome} — ${selecionado.cpf}` : "Selecione o proprietário"}
          </span>
          <ChevronsUpDown className="ml-2 size-4 shrink-0 opacity-50" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-[var(--radix-popover-trigger-width)] min-w-72 p-0" align="start">
        {/* A filtragem é nossa: a padrão do cmdk é aproximada e mostraria nomes parecidos. */}
        <Command shouldFilter={false}>
          <CommandInput
            placeholder="Pesquisar por nome ou CPF…"
            value={busca}
            onValueChange={setBusca}
          />
          <CommandList>
            <CommandEmpty>
              {carregando ? "Carregando proprietários…" : "Nenhum proprietário encontrado."}
            </CommandEmpty>
            {visiveis.length > 0 && (
              <CommandGroup>
                {visiveis.map((u) => (
                  <CommandItem
                    key={u.id}
                    value={u.id}
                    onSelect={() => {
                      onChange(u.id);
                      alternar(false);
                    }}
                  >
                    <Check
                      className={cn("mr-2 size-4", u.id === valor ? "opacity-100" : "opacity-0")}
                    />
                    <span className="truncate">
                      {u.nome} — {u.cpf}
                    </span>
                  </CommandItem>
                ))}
              </CommandGroup>
            )}
            {resultados.length > LIMITE_RESULTADOS && (
              <p className="px-3 py-2 text-xs text-muted-foreground">
                Mostrando {LIMITE_RESULTADOS} de {resultados.length}. Digite mais para refinar.
              </p>
            )}
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  );
}
