// src/pages/EmployeeDashboardPage.tsx
import React from "react";
import { useAuth } from "../context/AuthContext";
import { CheckInCard } from "../components/CheckinCard";
// import { CheckInCard } from "../components/CheckInCard";

export const EmployeeDashboardPage: React.FC = () => {
  const { user } = useAuth();

  if (!user) return null;

  return (
    <div className="page-shell">
      <section className="card">
        <h1 className="card-title">Dashboard do Funcionário</h1>
        <p className="card-subtitle">
          Registre seu horário de entrada e saída com segurança. Seus registros
          ficam disponíveis para o gestor acompanhar as horas trabalhadas no dia.
        </p>

        <CheckInCard />
      </section>

      <section className="side-panel">
        <div>
          <p className="side-kpi-label">Funcionário</p>
          <p className="side-kpi-value">{user.name}</p>
        </div>

        <div className="side-description">
          <p>
            • Clique em <strong>Check-in</strong> ao iniciar o expediente.
          </p>
          <p>
            • Clique em <strong>Check-out</strong> ao finalizar o dia.
          </p>
          <p>
            • O sistema calcula automaticamente o total de horas trabalhadas.
          </p>
        </div>
      </section>
    </div>
  );
};
