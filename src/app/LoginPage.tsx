import { LoginForm } from "@/features/auth/components/LoginForm";

export function LoginPage() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-secondary/60 px-4 py-10">
      <LoginForm />
    </div>
  );
}
