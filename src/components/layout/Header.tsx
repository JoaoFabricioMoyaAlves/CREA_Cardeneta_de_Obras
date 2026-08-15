import { Link, useNavigate } from "@tanstack/react-router";
import { HardHat, LogOut } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/lib/auth-context";

export function Header() {
  const { usuario, logout } = useAuth();
  const navigate = useNavigate();

  function sair() {
    logout();
    navigate({ to: "/" });
  }

  return (
    <header className="sticky top-0 z-30 bg-primary text-primary-foreground shadow-md">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between gap-4 px-4 md:px-6">
        <Link to="/dashboard" className="flex items-center gap-3">
          <span className="flex size-10 items-center justify-center rounded-md bg-white/15">
            <HardHat className="size-5" />
          </span>
          <span className="leading-tight">
            <span className="block text-sm font-semibold tracking-tight md:text-base">
              Caderneta de Obras Digital
            </span>
            <span className="hidden text-xs text-white/70 md:block">
              Sistema de acompanhamento técnico — CREA
            </span>
          </span>
        </Link>

        {usuario && (
          <div className="flex items-center gap-3">
            <div className="hidden text-right md:block">
              <p className="text-sm font-medium">{usuario.nome}</p>
              <p className="text-xs text-white/70">{usuario.numeroRegistro ?? usuario.cpf}</p>
            </div>
            <Button
              variant="ghost"
              aria-label="Sair"
              className="min-h-11 text-white hover:bg-white/15 hover:text-white"
              onClick={sair}
            >
              <LogOut className="size-4" />
            </Button>
          </div>
        )}
      </div>
    </header>
  );
}
