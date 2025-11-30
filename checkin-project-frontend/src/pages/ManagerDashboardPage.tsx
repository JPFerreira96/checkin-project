// src/pages/ManagerDashboardPage.tsx
import React, { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { listWorkRecords } from "../api/work";
import { WorkRecordsTable } from "../components/WorkRegistersTable";
import type { WorkRegister } from "../interfaces/work/WorkRegisterInterface";

export const ManagerDashboardPage: React.FC = () => {
  const { user } = useAuth();
  const [nameFilter, setNameFilter] = useState("");
  const [dateFilter, setDateFilter] = useState<string>(() => {
    const today = new Date();
    return today.toISOString().slice(0, 10); // YYYY-MM-DD
  });
  const [page, setPage] = useState(1);
  const [records, setRecords] = useState<WorkRegister[]>([]);
  const [loading, setLoading] = useState(false);

  const pageSize = 10;

  const loadData = async () => {
    setLoading(true);
    try {
      const result = await listWorkRecords({
        name: nameFilter || undefined,
        date: dateFilter || undefined,
        page,
        pageSize,
      });
      setRecords(result);
    } catch (error) {
      console.error("Erro ao carregar registros:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user?.role === "manager") {
      loadData();
    }
  }, [page]); // eslint-disable-line react-hooks/exhaustive-deps

  if (!user || user.role !== "manager") {
    return (
      <main style={{ padding: 24 }}>
        <h1>Área restrita</h1>
        <p>Você não tem permissão para acessar esta página.</p>
      </main>
    );
  }

  const handleSearch = () => {
    setPage(1);
    loadData();
  };

  const canGoNext = records.length === pageSize;
  const canGoPrev = page > 1;

  return (
    <main style={{ padding: 24 }}>
      <h1>Dashboard do Gestor</h1>
      <p>Visualize os registros de ponto dos funcionários.</p>

      <div
        style={{
          display: "flex",
          gap: 12,
          alignItems: "flex-end",
          marginTop: 16,
          marginBottom: 8,
          flexWrap: "wrap",
        }}
      >
        <div style={{ display: "flex", flexDirection: "column" }}>
          <label>Nome do funcionário</label>
          <input
            type="text"
            value={nameFilter}
            onChange={(e) => setNameFilter(e.target.value)}
            style={{ padding: 8, borderRadius: 4, border: "1px solid #d1d5db", minWidth: 220 }}
          />
        </div>

        <div style={{ display: "flex", flexDirection: "column" }}>
          <label>Data</label>
          <input
            type="date"
            value={dateFilter}
            onChange={(e) => setDateFilter(e.target.value)}
            style={{ padding: 8, borderRadius: 4, border: "1px solid #d1d5db" }}
          />
        </div>

        <button
          onClick={handleSearch}
          disabled={loading}
          style={{
            padding: "8px 16px",
            borderRadius: 4,
            border: "none",
            backgroundColor: "#2563eb",
            color: "#fff",
            fontWeight: 600,
            cursor: "pointer",
          }}
        >
          {loading ? "Buscando..." : "Buscar"}
        </button>
      </div>

      <WorkRecordsTable records={records} />

      <div style={{ display: "flex", gap: 8, marginTop: 16 }}>
        <button
          disabled={!canGoPrev}
          onClick={() => canGoPrev && setPage((p) => p - 1)}
          style={{
            padding: "6px 12px",
            borderRadius: 4,
            border: "1px solid #d1d5db",
            backgroundColor: canGoPrev ? "#fff" : "#f9fafb",
            cursor: canGoPrev ? "pointer" : "default",
          }}
        >
          Anterior
        </button>
        <button
          disabled={!canGoNext}
          onClick={() => canGoNext && setPage((p) => p + 1)}
          style={{
            padding: "6px 12px",
            borderRadius: 4,
            border: "1px solid #d1d5db",
            backgroundColor: canGoNext ? "#fff" : "#f9fafb",
            cursor: canGoNext ? "pointer" : "default",
          }}
        >
          Próxima
        </button>
        <span style={{ alignSelf: "center", marginLeft: 8 }}>Página {page}</span>
      </div>
    </main>
  );
};
