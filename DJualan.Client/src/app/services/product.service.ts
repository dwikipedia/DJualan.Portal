// src/app/services/product.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, Observable, throwError, of, map } from 'rxjs';
import { Product } from '../models/product.model';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';
import { DashboardStats } from '../models/stats.model';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = `${environment.apiUrl}/product`;

  constructor(
    private http: HttpClient,
    private authService: AuthService // Add AuthService dependency
  ) {}

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    if (!token) {
      console.warn('No JWT token available');
      return new HttpHeaders({
        'Content-Type': 'application/json',
      });
    }

    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    });
  }

  // Get all products
  getProducts(): Observable<Product[]> {
    if (!this.authService.isLoggedIn()) {
      console.warn('User not logged in. Please login to access products.');
      return of([]); // Return empty array if not logged in
    }

    return this.http
      .get<Product[]>(this.apiUrl, { headers: this.getHeaders() })
      .pipe(
        catchError((error) => {
          console.error('API Error:', error);
          if (error.status === 401) {
            console.error('Authentication failed. Redirecting to login...');
            // You might want to trigger logout or redirect here
            this.authService.logout();
          }
          return of([]); // Return empty array on error
        })
      );
  }

  // Get product by ID
  getProductById(id: number): Observable<Product> {
    if (!this.authService.isLoggedIn()) {
      console.warn('User not logged in. Cannot fetch product details.');
      return throwError(() => new Error('User not authenticated'));
    }

    return this.http
      .get<Product>(`${this.apiUrl}/${id}`, {
        headers: this.getHeaders(),
      })
      .pipe(
        catchError((error) => {
          console.error(`Error fetching product ${id}:`, error);
          return throwError(() => error);
        })
      );
  }

  // src/app/services/product.service.ts - ensure these methods exist
  // Create new product
  createProduct(product: Product): Observable<Product> {
    if (!this.authService.isLoggedIn()) {
      return throwError(() => new Error('User not authenticated'));
    }

    return this.http
      .post<Product>(this.apiUrl, product, {
        headers: this.getHeaders(),
      })
      .pipe(
        catchError((error) => {
          console.error('Error creating product:', error);
          return throwError(() => error);
        })
      );
  }

  // Update product
  updateProduct(id: number, product: Product): Observable<Product> {
    if (!this.authService.isLoggedIn()) {
      return throwError(() => new Error('User not authenticated'));
    }

    return this.http
      .put<Product>(`${this.apiUrl}/${id}`, product, {
        headers: this.getHeaders(),
      })
      .pipe(
        catchError((error) => {
          console.error(`Error updating product ${id}:`, error);
          return throwError(() => error);
        })
      );
  }

  // Delete product
  deleteProduct(id: number): Observable<void> {
    if (!this.authService.isLoggedIn()) {
      return throwError(() => new Error('User not authenticated'));
    }

    return this.http
      .delete<void>(`${this.apiUrl}/${id}`, {
        headers: this.getHeaders(),
      })
      .pipe(
        catchError((error) => {
          console.error(`Error deleting product ${id}:`, error);
          return throwError(() => error);
        })
      );
  }

  getProductsWithRetry(retryCount: number = 3): Observable<Product[]> {
    return this.getProducts().pipe(
      catchError((error) => {
        if (retryCount > 0) {
          console.log(`Retrying... ${retryCount} attempts left`);
          return this.getProductsWithRetry(retryCount - 1);
        }
        return throwError(() => error);
      })
    );
  }

  getDashboardStats(): Observable<DashboardStats> {
    if (!this.authService.isLoggedIn()) {
      console.warn('User not logged in. Please login to access dashboard.');
      return of({
        totalProducts: 0,
        totalSales: 0,
        totalRevenue: 0,
        lowStockItems: 0,
        recentProducts: [],
      });
    }

    return this.http
      .get<Product[]>(this.apiUrl, { headers: this.getHeaders() })
      .pipe(
        map((products) => this.calculateStatsFromProducts(products)),
        catchError((error) => {
          console.error('Error loading dashboard stats:', error);
          return of({
            totalProducts: 0,
            totalSales: 0,
            totalRevenue: 0,
            lowStockItems: 0,
            recentProducts: [],
          });
        })
      );
  }

  private calculateStatsFromProducts(products: Product[]): DashboardStats {
    const totalProducts = products.length;
    const lowStockItems = products.filter(
      (p) => p.stock > 0 && p.stock <= 10
    ).length;

    // Mock sales data - replace with actual sales data from your API
    const totalSales = products.reduce(
      (sum, product) =>
        sum + (product.stock < 50 ? Math.floor(Math.random() * 20) : 0),
      0
    );
    const totalRevenue = products.reduce(
      (sum, product) =>
        sum +
        product.price *
          (product.stock < 50 ? Math.floor(Math.random() * 10) : 0),
      0
    );

    // Get recent products (last 3 created)
    const recentProducts = [...products]
      .sort(
        (a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      )
      .slice(0, 3);

    return {
      totalProducts,
      totalSales,
      totalRevenue,
      lowStockItems,
      recentProducts,
    };
  }
}
