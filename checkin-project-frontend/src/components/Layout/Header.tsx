// src/components/Layout/Header.tsx
import React from "react";
import { useAuth } from "../../context/AuthContext";
import logoMoura from "../../assets/logo-moura.png";

export const Header: React.FC = () => {
  const { user, logout } = useAuth();

  return (
   <header className="app-header">
      <div className="app-header-left">
        <img src={logoMoura} alt="Grupo Moura" className="app-logo" />
        <span className="app-header-brand">Sistema de Check-in</span>
      </div>

      {user && (
        <div className="app-header-user">
          <span>
            Logado como: <strong>{user.name}</strong> ({user.role})
          </span>
          <button className="btn-outline" onClick={logout}>
            Sair
          </button>
        </div>
      )}
    </header>
  );
};
