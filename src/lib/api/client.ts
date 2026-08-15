const API_BASE_URL = import.meta.env["VITE_API_URL"] ?? "http://localhost:5282";
const TOKEN_STORAGE_KEY = "caderneta_token";

export class ApiError extends Error {
  status: number;
  constructor(message: string, status: number) {
    super(message);
    this.status = status;
  }
}

export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(TOKEN_STORAGE_KEY);
}

export function setToken(token: string) {
  localStorage.setItem(TOKEN_STORAGE_KEY, token);
}

export function clearToken() {
  localStorage.removeItem(TOKEN_STORAGE_KEY);
}

async function parseErrorMessage(res: Response): Promise<string> {
  try {
    const data = await res.json();
    if (typeof data?.erro === "string") return data.erro;
    if (typeof data?.title === "string") return data.title;
  } catch {
    // corpo não é JSON (ex: erro de rede/proxy) — cai no fallback abaixo
  }
  return `Erro ${res.status} ao comunicar com o servidor.`;
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const token = getToken();
  const res = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...init?.headers,
    },
  });

  if (!res.ok) {
    throw new ApiError(await parseErrorMessage(res), res.status);
  }

  // 204 (ex: GET termo-conclusao quando ainda não existe) vira `null`, nunca
  // `undefined` — o React Query rejeita `undefined` como retorno de query.
  if (res.status === 204) return null as T;
  return (await res.json()) as T;
}

export const api = {
  get: <T>(path: string) => request<T>(path, { method: "GET" }),

  post: <T>(path: string, body?: unknown) =>
    request<T>(path, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      ...(body !== undefined ? { body: JSON.stringify(body) } : {}),
    }),

  postForm: <T>(path: string, formData: FormData) =>
    request<T>(path, { method: "POST", body: formData }),
};
