import { Product } from "./product.model";

export interface DashboardStats {
  totalProducts: number;
  totalSales: number;
  totalRevenue: number;
  lowStockItems: number;
  recentProducts: Product[];
}

export interface SalesData {
  date: string;
  amount: number;
}