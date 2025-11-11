// src/app/components/dashboard/dashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { DashboardStats } from '../../models/stats.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  stats: DashboardStats = {
    totalProducts: 0,
    totalSales: 0,
    totalRevenue: 0,
    lowStockItems: 0,
    recentProducts: [],
  };

  loading: boolean = true;
  error: string = '';

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadDashboardStats();
  }

  loadDashboardStats(): void {
    this.loading = true;
    this.productService.getDashboardStats().subscribe({
      next: (stats) => {
        this.stats = stats;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load dashboard statistics';
        this.loading = false;
        console.error('Error loading dashboard:', error);
      },
    });
  }

  formatPrice(price: number): string {
    return new Intl.NumberFormat('id-ID', {
      style: 'currency',
      currency: 'IDR',
      minimumFractionDigits: 0,
    }).format(price);
  }

  formatNumber(num: number): string {
    return new Intl.NumberFormat('id-ID').format(num);
  }

  getRevenueGrowth(): number {
    // Mock growth percentage - replace with actual calculation
    return 12.5;
  }

  getSalesGrowth(): number {
    // Mock growth percentage - replace with actual calculation
    return 8.3;
  }

  // Add to dashboard.component.ts
  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }
}
