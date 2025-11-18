import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './add-product.component.html',
  styleUrls: ['./add-product.component.scss']
})
export class AddProductComponent {
  productForm: FormGroup;
  loading = false;
  error = '';
  success = false;

  categories = [
    'Electronics',
    'Clothing',
    'Books',
    'Home & Garden',
    'Sports',
    'Toys & Games',
    'Beauty',
    'Food & Beverages',
    'Automotive',
    'Other'
  ];

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private router: Router
  ) {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
      price: ['', [Validators.required, Validators.min(0), Validators.max(100000000)]],
      stock: ['', [Validators.required, Validators.min(0), Validators.max(10000)]],
      category: ['', Validators.required],
      imageUrl: ['', Validators.pattern('https?://.+')]
    });
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      this.loading = true;
      this.error = '';
      this.success = false;

      const productData: Product = {
        ...this.productForm.value,
        price: Number(this.productForm.value.price),
        stock: Number(this.productForm.value.stock),
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        priceInRp: '', // Will be set by backend
        id: 0 // Will be set by backend
      };

      this.productService.createProduct(productData).subscribe({
        next: (product) => {
          this.loading = false;
          this.success = true;
          this.productForm.reset();
          
          // Auto-redirect after 2 seconds
          setTimeout(() => {
            this.router.navigate(['/products', product.id]);
          }, 2000);
        },
        error: (error) => {
          this.loading = false;
          this.error = 'Failed to create product. Please try again.';
          console.error('Error creating product:', error);
        }
      });
    } else {
      // Mark all fields as touched to trigger validation messages
      Object.keys(this.productForm.controls).forEach(key => {
        this.productForm.get(key)?.markAsTouched();
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/products']);
  }

  get name() { return this.productForm.get('name'); }
  get description() { return this.productForm.get('description'); }
  get price() { return this.productForm.get('price'); }
  get stock() { return this.productForm.get('stock'); }
  get category() { return this.productForm.get('category'); }
  get imageUrl() { return this.productForm.get('imageUrl'); }
}