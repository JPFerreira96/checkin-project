export interface WorkRegister {
  id: number;
  employeeName: string;
  checkinTime: string;  // ISO string vinda da API
  checkoutTime?: string | null;
  durationHours?: number | null;
}