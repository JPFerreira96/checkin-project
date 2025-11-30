// src/components/CheckInCard.tsx
import React, { useState } from "react";
import { checkIn, checkOut } from "../api/work";
import { useAuth } from "../context/AuthContext";
import type { WorkRegister } from "../interfaces/work/WorkRegisterInterface";

export const CheckInCard: React.FC = () => {
  const { user } = useAuth();
  const [lastRecord, setLastRecord] = useState<WorkRegister | null>(null);
  const [loading, setLoading] = useState<"in" | "out" | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  if (!user) return null;

  const handleCheckIn = async () => {
    setMessage(null);
    setLoading("in");
    try {
      const record = await checkIn(user.employeeId);
      setLastRecord(record);
      setMessage("Check-in realizado com sucesso!");
    } catch (error: any) {
      const msg = error?.response?.data?.message ?? "Erro ao realizar check-in.";
      setMessage(msg);
    } finally {
      setLoading(null);
    }
  };

  const handleCheckOut = async () => {
    setMessage(null);
    setLoading("out");
    try {
      const record = await checkOut(user.employeeId);
      setLastRecord(record);
      setMessage("Check-out realizado com sucesso!");
    } catch (error: any) {
      const msg = error?.response?.data?.message ?? "Erro ao realizar check-out.";
      setMessage(msg);
    } finally {
      setLoading(null);
    }
  };

  const formatDateTime = (value?: string | null) => {
    if (!value) return "-";
    const d = new Date(value);
    return d.toLocaleString("pt-BR");
  };

  const formatDuration = (hours?: number | null) => {
    if (hours == null) return "-";
    const totalMinutes = Math.round(hours * 60);
    const h = Math.floor(totalMinutes / 60);
    const m = totalMinutes % 60;
    return `${h}h ${m}min`;
  };

  return (
    <div className="checkin-card">
      <div className="checkin-header">Área do Funcionário</div>
      <div className="checkin-sub">
        Olá, <strong>{user.name}</strong>. Use os botões abaixo para registrar seu ponto.
      </div>

      <div className="btn-row">
        <button
          onClick={handleCheckIn}
          disabled={loading !== null}
          className="btn btn-primary"
        >
          {loading === "in" ? "Registrando..." : "Check-in"}
        </button>

        <button
          onClick={handleCheckOut}
          disabled={loading !== null}
          className="btn btn-danger"
        >
          {loading === "out" ? "Registrando..." : "Check-out"}
        </button>
      </div>

      {message && (
        <p className="message">
          <strong>{message}</strong>
        </p>
      )}

      {lastRecord && (
        <div className="last-record">
          <h3>Último registro</h3>
          <p className="muted">Check-in: {formatDateTime(lastRecord.checkinTime)}</p>
          <p className="muted">Check-out: {formatDateTime(lastRecord.checkoutTime)}</p>
          <p className="muted">
            Tempo total trabalhado no dia:{" "}
            <strong>{formatDuration(lastRecord.durationHours)}</strong>
          </p>
        </div>
      )}
    </div>
  );
};
