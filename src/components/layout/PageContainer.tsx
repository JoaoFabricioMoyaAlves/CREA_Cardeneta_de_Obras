import type { ReactNode } from "react";
import { Header } from "./Header";
import { Sidebar } from "./Sidebar";
import { AssistenteButton } from "@/features/assistente-ia/components/AssistenteButton";

export function PageContainer({
  children,
  titulo,
  descricao,
  acoes,
}: {
  children: ReactNode;
  titulo?: string;
  descricao?: string;
  acoes?: ReactNode;
}) {
  return (
    <div className="min-h-screen bg-background">
      <Header />
      <div className="mx-auto flex max-w-7xl">
        <Sidebar />
        <main className="min-w-0 flex-1 px-4 py-6 pb-28 md:px-6 md:py-8">
          {(titulo || acoes) && (
            <div className="mb-6 flex flex-wrap items-start justify-between gap-4">
              <div>
                {titulo && (
                  <h1 className="text-2xl font-bold tracking-tight text-primary md:text-3xl">
                    {titulo}
                  </h1>
                )}
                {descricao && (
                  <p className="mt-1 text-sm text-muted-foreground">{descricao}</p>
                )}
              </div>
              {acoes}
            </div>
          )}
          {children}
        </main>
      </div>
      <AssistenteButton />
    </div>
  );
}
