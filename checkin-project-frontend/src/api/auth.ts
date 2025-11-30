// src/api/authApi.ts
import type { LoginRequest } from "../interfaces/login/LoginRequestInterface";
import type { LoginResponse } from "../interfaces/login/LoginResponseInterface";
import { api } from "./http";

export async function login(request: LoginRequest): Promise<LoginResponse> {
  const { data } = await api.post<LoginResponse>("/auth/login", request);
  return data;
}
