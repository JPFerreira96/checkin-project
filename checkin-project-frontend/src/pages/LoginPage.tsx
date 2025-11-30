// src/pages/LoginPage.tsx
import React, { useState } from "react";
import { useAuth } from "../context/AuthContext";

export const LoginPage: React.FC = () => {
  const { login } = useAuth();
  const [email, setEmail] = useState("funcionario@empresa.com");
  const [password, setPassword] = useState("123456");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      await login(email, password);
    } catch (err: any) {
      const msg =
        err?.response?.data?.message ??
        "Falha no login. Verifique e-mail e senha.";
      setError(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      {/* Lado Moura Tech / mensagem */}
      <section className="login-hero">
        <h1 className="login-hero-title">
          Check-in de jornada para times Moura.
        </h1>
        <p className="login-hero-subtitle">
          Registro rápido de entrada e saída, com visão consolidada para
          gestores acompanharem a operação em tempo real.
        </p>
        <ul className="login-hero-list">
          <li>✔ Transparência nas horas trabalhadas.</li>
          <li>✔ Menos planilha, mais automação.</li>
          <li>✔ Experiência simples para o colaborador.</li>
        </ul>
      </section>

      {/* Card de login */}
      <section className="login-card">
        <h2>Login</h2>
        <form onSubmit={handleSubmit} className="login-form">
          <div className="login-field">
            <label htmlFor="email">E-mail</label>
            <input
              id="email"
              type="email"
              value={email}
              autoComplete="username"
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="login-field">
            <label htmlFor="password">Senha</label>
            <input
              id="password"
              type="password"
              value={password}
              autoComplete="current-password"
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          {error && <p className="login-error">{error}</p>}

          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? "Entrando..." : "Entrar"}
          </button>

          <p className="login-hint">
            Para testes, use <strong>funcionario@empresa.com</strong> ou{" "}
            <strong>gestor@empresa.com</strong> com a senha{" "}
            <strong>123456</strong>.
          </p>
        </form>
      </section>
    </div>
  );
};
