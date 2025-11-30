// src/routes/AppRouter.tsx
import React from "react";
import { useAuth } from "../context/AuthContext";
import { LoginPage } from "../pages/LoginPage";
import { EmployeeDashboardPage } from "../pages/EmployeeDashboardPage";
import { ManagerDashboardPage } from "../pages/ManagerDashboardPage";

export const AppRouter: React.FC = () => {
  const { user } = useAuth();

  if (!user) {
    return <LoginPage />;
  }

  // Se for gestor, mostra a view do gestor + eventualmente aba de funcionário se quiser
  if (user.role === "manager") {
    return <ManagerDashboardPage />;
  }

  // Funcionário
  return <EmployeeDashboardPage />;
};
