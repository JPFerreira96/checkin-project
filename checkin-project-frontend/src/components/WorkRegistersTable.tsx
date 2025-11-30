// src/components/WorkRecordsTable.tsx
import React from "react";
import type { WorkRegister } from "../interfaces/work/WorkRegisterInterface";

interface Props {
  records: WorkRegister[];
}

export const WorkRecordsTable: React.FC<Props> = ({ records }) => {
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
    <table
      style={{
        width: "100%",
        borderCollapse: "collapse",
        marginTop: 16,
        fontSize: 14,
      }}
    >
      <thead>
        <tr>
          <th style={{ borderBottom: "1px solid #e5e7eb", textAlign: "left", padding: 8 }}>Funcionário</th>
          <th style={{ borderBottom: "1px solid #e5e7eb", textAlign: "left", padding: 8 }}>Check-in</th>
          <th style={{ borderBottom: "1px solid #e5e7eb", textAlign: "left", padding: 8 }}>Check-out</th>
          <th style={{ borderBottom: "1px solid #e5e7eb", textAlign: "left", padding: 8 }}>Duração</th>
        </tr>
      </thead>
      <tbody>
        {records.map((r) => (
          <tr key={r.id}>
            <td style={{ borderBottom: "1px solid #f3f4f6", padding: 8 }}>{r.employeeName}</td>
            <td style={{ borderBottom: "1px solid #f3f4f6", padding: 8 }}>{formatDateTime(r.checkinTime)}</td>
            <td style={{ borderBottom: "1px solid #f3f4f6", padding: 8 }}>{formatDateTime(r.checkoutTime)}</td>
            <td style={{ borderBottom: "1px solid #f3f4f6", padding: 8 }}>{formatDuration(r.durationHours)}</td>
          </tr>
        ))}
        {records.length === 0 && (
          <tr>
            <td colSpan={4} style={{ padding: 8, textAlign: "center", color: "#6b7280" }}>
              Nenhum registro encontrado.
            </td>
          </tr>
        )}
      </tbody>
    </table>
  );
};
