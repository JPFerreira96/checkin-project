type Role = "employee" | "manager";

export interface AuthUser {
  employeeId: number;
  name: string;
  role: Role;
}