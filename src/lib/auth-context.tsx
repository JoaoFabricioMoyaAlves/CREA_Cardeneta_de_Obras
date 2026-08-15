import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import { useNavigate } from "@tanstack/react-router";
import * as authApi from "@/lib/api/auth";
import { ApiError, clearToken, getToken, setToken } from "@/lib/api/client";
import { perfilDeApi, type UsuarioLogado } from "@/lib/api/types";
import type { Perfil } from "@/lib/constants";

const USUARIO_STORAGE_KEY = "caderneta_usuario";

type AuthContextValue = {
  usuario: UsuarioLogado | null;
  perfil: Perfil | null;
  carregando: boolean;
  login: (cpf: string, senha: string) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [usuario, setUsuario] = useState<UsuarioLogado | null>(null);
  const [carregando, setCarregando] = useState(true);

  useEffect(() => {
    const token = getToken();
    const usuarioSalvo = localStorage.getItem(USUARIO_STORAGE_KEY);
    if (token && usuarioSalvo) {
      try {
        setUsuario(JSON.parse(usuarioSalvo) as UsuarioLogado);
      } catch {
        clearToken();
        localStorage.removeItem(USUARIO_STORAGE_KEY);
      }
    }
    setCarregando(false);
  }, []);

  async function login(cpf: string, senha: string) {
    const resposta = await authApi.login(cpf, senha);
    setToken(resposta.token);
    localStorage.setItem(USUARIO_STORAGE_KEY, JSON.stringify(resposta.usuario));
    setUsuario(resposta.usuario);
  }

  function logout() {
    clearToken();
    localStorage.removeItem(USUARIO_STORAGE_KEY);
    setUsuario(null);
  }

  return (
    <AuthContext.Provider
      value={{
        usuario,
        perfil: usuario ? perfilDeApi(usuario.perfil) : null,
        carregando,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth deve ser usado dentro de AuthProvider");
  return ctx;
}

// Toda página protegida chama isso no topo. Enquanto `carregando` for true
// ou `usuario` for null, a página deve retornar null (o useEffect cuida do
// redirecionamento — nunca renderizar conteúdo protegido sem sessão válida).
export function useRequireAuth() {
  const { usuario, perfil, carregando, logout } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!carregando && !usuario) {
      navigate({ to: "/" });
    }
  }, [carregando, usuario, navigate]);

  return { usuario, perfil, carregando, logout };
}

export { ApiError };
