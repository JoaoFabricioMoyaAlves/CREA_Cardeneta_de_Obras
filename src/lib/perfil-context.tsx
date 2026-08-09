import { createContext, useContext, useState, type ReactNode } from "react";
import type { Perfil } from "./constants";
import { usuarioAtualPorPerfil, type Usuario } from "./mock-data";

type PerfilContextValue = {
  perfil: Perfil;
  setPerfil: (p: Perfil) => void;
  usuario: Usuario;
};

const PerfilContext = createContext<PerfilContextValue | null>(null);

export function PerfilProvider({ children }: { children: ReactNode }) {
  const [perfil, setPerfil] = useState<Perfil>("administrador");
  return (
    <PerfilContext.Provider
      value={{ perfil, setPerfil, usuario: usuarioAtualPorPerfil[perfil] }}
    >
      {children}
    </PerfilContext.Provider>
  );
}

export function usePerfil() {
  const ctx = useContext(PerfilContext);
  if (!ctx) throw new Error("usePerfil deve ser usado dentro de PerfilProvider");
  return ctx;
}
