import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-delete-product',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './delete-product.component.html',
  styleUrls: ['./delete-product.component.scss']
})
export class DeleteProductComponent implements OnInit {
  product: Product | null = null;
  loading = true;
  deleting = false;
  error = '';
  productId: number = 0;

  constructor(
    private productService: ProductService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.productId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadProduct();
  }

  loadProduct(): void {
    this.loading = true;
    this.productService.getProductById(this.productId).subscribe({
      next: (product) => {
        this.product = product;
        this.loading = false;
      },
      error: (error) => {
        this.loading = false;
        this.error = 'Failed to load product. Please try again.';
        console.error('Error loading product:', error);
      }
    });
  }

  onDelete(): void {
    if (this.product) {
      this.deleting = true;
      this.error = '';

      this.productService.deleteProduct(this.productId).subscribe({
        next: () => {
          this.deleting = false;
          this.router.navigate(['/products'], {
            queryParams: { deleted: 'true' }
          });
        },
        error: (error) => {
          this.deleting = false;
          this.error = 'Failed to delete product. Please try again.';
          console.error('Error deleting product:', error);
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/products', this.productId]);
  }
}