// src/api/workApi.ts
import type { CheckRequest } from "../interfaces/check/CheckRequestInterface";
import type { WorkListParams } from "../interfaces/work/WorkListParamsInterface";
import type { WorkRegister } from "../interfaces/work/WorkRegisterInterface";
import { api } from "./http";

export async function checkIn(employeeId: number): Promise<WorkRegister> {
  const { data } = await api.post<WorkRegister>("/work/checkin", { employeeId } as CheckRequest);
  return data;
}

export async function checkOut(employeeId: number): Promise<WorkRegister> {
  const { data } = await api.post<WorkRegister>("/work/checkout", { employeeId } as CheckRequest);
  return data;
}

export async function listWorkRecords(params: WorkListParams): Promise<WorkRegister[]> {
  const { data } = await api.get<WorkRegister[]>("/work/list", { params });
  return data;
}
