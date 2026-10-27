import { ResetPasswordForm } from "@/features/auth/components/ResetPasswordForm";

export function ResetPasswordPage({ token, primeiroAcesso }: { token: string; primeiroAcesso: boolean }) {
  return (
    <div className="flex min-h-screen items-center justify-center bg-secondary/60 px-4 py-10">
      <ResetPasswordForm token={token} primeiroAcesso={primeiroAcesso} />
    </div>
  );
}
