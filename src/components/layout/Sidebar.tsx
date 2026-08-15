import { Link } from "@tanstack/react-router";
import { FolderOpen, LayoutDashboard, PlusCircle, Users } from "lucide-react";
import { useAuth } from "@/lib/auth-context";

export function Sidebar() {
  const { perfil } = useAuth();

  const itens = [
    { to: "/dashboard", label: "Cadernetas", icon: LayoutDashboard },
    ...(perfil === "administrador"
      ? [
          { to: "/obras/nova", label: "Nova obra", icon: PlusCircle },
          { to: "/usuarios", label: "Usuários", icon: Users },
        ]
      : []),
  ] as const;

  return (
    <aside className="hidden w-60 shrink-0 border-r border-border bg-card px-3 py-6 xl:block">
      <p className="px-3 pb-2 text-xs font-semibold tracking-wide text-muted-foreground uppercase">
        Navegação
      </p>
      <nav className="space-y-1">
        {itens.map((item) => (
          <Link
            key={item.to}
            to={item.to}
            activeProps={{ className: "bg-secondary text-primary font-semibold" }}
            className="flex min-h-11 items-center gap-3 rounded-md px-3 text-sm text-foreground transition-colors hover:bg-secondary/60 hover:text-primary"
          >
            <item.icon className="size-4" />
            {item.label}
          </Link>
        ))}
      </nav>
      <div className="mt-8 rounded-md bg-secondary/50 p-3">
        <FolderOpen className="mb-2 size-4 text-primary" />
        <p className="text-xs text-muted-foreground">
          Registros de visita assinados digitalmente ficam disponíveis para consulta
          permanente.
        </p>
      </div>
    </aside>
  );
}
