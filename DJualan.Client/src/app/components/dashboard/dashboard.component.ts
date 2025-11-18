// src/app/components/dashboard/dashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { DashboardStats } from '../../models/stats.model';
import { UtilsService } from '../../services/utils.service';

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

  constructor(
    private productService: ProductService,
    private utilsService: UtilsService
  ) {}

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
    return this.utilsService.formatPrice(price);
  }

  formatNumber(num: number): string {
    return this.utilsService.formatNumber(num);
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
    return this.utilsService.formatDate(dateString);
  }
}
