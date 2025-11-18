import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { UtilsService } from '../../services/utils.service';
import { Product } from '../../models/product.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {
  product: Product | null = null;
  loading: boolean = true;
  error: string = '';
  selectedImage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private utilsService: UtilsService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    
    if (isNaN(id)) {
      this.error = 'Invalid product ID';
      this.loading = false;
      return;
    }

    this.productService.getProductById(id).subscribe({
      next: (product) => {
        this.product = product;
        this.selectedImage = product.imageUrl;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Product not found or failed to load';
        this.loading = false;
        console.error('Error loading product:', error);
      }
    });
  }

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  formatPrice(price: number): string {
    return this.utilsService.formatPrice(price);
  }

  formatDate(dateString: string): string {
    return this.utilsService.formatDate(dateString);
  }

  getStockStatus(): string {
    if (!this.product) return '';
    
    if (this.product.stock === 0) {
      return 'Out of Stock';
    } else if (this.product.stock <= 10) {
      return 'Only ' + this.product.stock + ' left in stock';
    } else {
      return 'In Stock';
    }
  }

  getStockStatusClass(): string {
    if (!this.product) return '';
    
    if (this.product.stock === 0) {
      return 'out-of-stock';
    } else if (this.product.stock <= 10) {
      return 'low-stock';
    } else {
      return 'in-stock';
    }
  }

  goBack(): void {
    this.router.navigate(['/products']);
  }
}