import { Link } from "@tanstack/react-router";
import { PageContainer } from "@/components/layout/PageContainer";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { ArrowLeft, ShieldAlert } from "lucide-react";

export function AcessoRestrito({ mensagem }: { mensagem: string }) {
  return (
    <PageContainer>
      <Card className="mx-auto max-w-lg border-warning/40 bg-warning-soft">
        <CardContent className="space-y-4 p-8 text-center">
          <ShieldAlert className="mx-auto size-10 text-warning" />
          <h1 className="text-lg font-semibold text-primary">Acesso restrito</h1>
          <p className="text-sm text-foreground">{mensagem}</p>
          <Button asChild variant="outline" className="min-h-11">
            <Link to="/dashboard">
              <ArrowLeft className="mr-2 size-4" /> Voltar às cadernetas
            </Link>
          </Button>
        </CardContent>
      </Card>
    </PageContainer>
  );
}
