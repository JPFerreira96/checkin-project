// src/context/AuthContext.tsx
import React, { createContext, useContext, useEffect, useState } from "react";
import { login as loginApi } from "../api/auth";
import type { LoginResponse } from "../interfaces/login/LoginResponseInterface";
import type { AuthUser } from "../interfaces/auth/AuthUserInterface";
import type { AuthContextValue } from "../interfaces/auth/AuthContextValueInterface";


type Role = "employee" | "manager";

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

const STORAGE_KEY = "checkin_auth_user";

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<AuthUser | null>(null);

  useEffect(() => {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (raw) {
      try {
        const parsed = JSON.parse(raw) as AuthUser;
        setUser(parsed);
      } catch {
        localStorage.removeItem(STORAGE_KEY);
      }
    }
  }, []);

  const login = async (email: string, password: string) => {
    const response: LoginResponse = await loginApi({ email, password });

    const mapped: AuthUser = {
      employeeId: response.employeeId,
      name: response.name,
      role: (response.role as Role) || "employee",
    };

    setUser(mapped);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(mapped));
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem(STORAGE_KEY);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth deve ser usado dentro de AuthProvider");
  }
  return ctx;
}
