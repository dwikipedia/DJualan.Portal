// src/app/services/product.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, Observable, throwError, of } from 'rxjs';
import { Product } from '../models/product.model';
import { AuthService } from './auth.service';
import {environment} from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = `${environment.apiUrl}/product`;

  constructor(
    private http: HttpClient,
    private authService: AuthService  // Add AuthService dependency
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

    return this.http.get<Product[]>(this.apiUrl, { headers: this.getHeaders() }).pipe(
      catchError(error => {
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

    return this.http.get<Product>(`${this.apiUrl}/${id}`, {
      headers: this.getHeaders(),
    }).pipe(
      catchError(error => {
        console.error(`Error fetching product ${id}:`, error);
        return throwError(() => error);
      })
    );
  }

  // Create new product
  createProduct(product: Product): Observable<Product> {
    if (!this.authService.isLoggedIn()) {
      console.warn('User not logged in. Cannot create product.');
      return throwError(() => new Error('User not authenticated'));
    }

    return this.http.post<Product>(this.apiUrl, product, {
      headers: this.getHeaders(),
    }).pipe(
      catchError(error => {
        console.error('Error creating product:', error);
        return throwError(() => error);
      })
    );
  }

  // Update product
  updateProduct(id: number, product: Product): Observable<Product> {
    if (!this.authService.isLoggedIn()) {
      console.warn('User not logged in. Cannot update product.');
      return throwError(() => new Error('User not authenticated'));
    }

    return this.http.put<Product>(`${this.apiUrl}/${id}`, product, {
      headers: this.getHeaders(),
    }).pipe(
      catchError(error => {
        console.error(`Error updating product ${id}:`, error);
        return throwError(() => error);
      })
    );
  }

  // Delete product
  deleteProduct(id: number): Observable<void> {
    if (!this.authService.isLoggedIn()) {
      console.warn('User not logged in. Cannot delete product.');
      return throwError(() => new Error('User not authenticated'));
    }

    return this.http.delete<void>(`${this.apiUrl}/${id}`, {
      headers: this.getHeaders(),
    }).pipe(
      catchError(error => {
        console.error(`Error deleting product ${id}:`, error);
        return throwError(() => error);
      })
    );
  }

  // Optional: Get products with retry logic
  getProductsWithRetry(retryCount: number = 3): Observable<Product[]> {
    return this.getProducts().pipe(
      catchError(error => {
        if (retryCount > 0) {
          console.log(`Retrying... ${retryCount} attempts left`);
          return this.getProductsWithRetry(retryCount - 1);
        }
        return throwError(() => error);
      })
    );
  }
}