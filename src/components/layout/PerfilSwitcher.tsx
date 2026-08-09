import { usePerfil } from "@/lib/perfil-context";
import { PERFIS, type Perfil } from "@/lib/constants";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Eye } from "lucide-react";

export function PerfilSwitcher() {
  const { perfil, setPerfil } = usePerfil();
  return (
    <div className="flex items-center gap-2 rounded-md bg-white/10 px-2 py-1">
      <Eye className="size-4 text-white/70" />
      <span className="hidden text-xs text-white/70 lg:inline">Visualizando como</span>
      <Select value={perfil} onValueChange={(v) => setPerfil(v as Perfil)}>
        <SelectTrigger className="h-9 w-[190px] border-white/20 bg-transparent text-sm text-white focus:ring-white/40">
          <SelectValue placeholder="Selecione o perfil">
            {PERFIS.find((p) => p.value === perfil)?.label}
          </SelectValue>
        </SelectTrigger>
        <SelectContent>
          {PERFIS.map((p) => (
            <SelectItem key={p.value} value={p.value}>
              {p.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}
