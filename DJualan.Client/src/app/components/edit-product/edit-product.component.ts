import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-edit-product',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './edit-product.component.html',
  styleUrls: ['./edit-product.component.scss']
})
export class EditProductComponent implements OnInit {
  productForm: FormGroup;
  loading = false;
  loadingProduct = true;
  error = '';
  success = false;
  productId: number = 0;

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
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
      price: ['', [Validators.required, Validators.min(0), Validators.max(100000000)]],
      stock: ['', [Validators.required, Validators.min(0), Validators.max(10000)]],
      category: ['', Validators.required],
      imageUrl: ['', Validators.pattern('https?://.+')],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.productId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadProduct();
  }

  loadProduct(): void {
    this.loadingProduct = true;
    this.productService.getProductById(this.productId).subscribe({
      next: (product) => {
        this.productForm.patchValue({
          name: product.name,
          description: product.description,
          price: product.price,
          stock: product.stock,
          category: product.category,
          imageUrl: product.imageUrl,
          isActive: product.isActive
        });
        this.loadingProduct = false;
      },
      error: (error) => {
        this.loadingProduct = false;
        this.error = 'Failed to load product. Please try again.';
        console.error('Error loading product:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      this.loading = true;
      this.error = '';
      this.success = false;

      const productData: Product = {
        ...this.productForm.value,
        id: this.productId,
        price: Number(this.productForm.value.price),
        stock: Number(this.productForm.value.stock),
        updatedAt: new Date().toISOString()
      };

      this.productService.updateProduct(this.productId, productData).subscribe({
        next: (product) => {
          this.loading = false;
          this.success = true;
          
          // Auto-redirect after 2 seconds
          setTimeout(() => {
            this.router.navigate(['/products', product.id]);
          }, 2000);
        },
        error: (error) => {
          this.loading = false;
          this.error = 'Failed to update product. Please try again.';
          console.error('Error updating product:', error);
        }
      });
    } else {
      Object.keys(this.productForm.controls).forEach(key => {
        this.productForm.get(key)?.markAsTouched();
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/products', this.productId]);
  }

  get name() { return this.productForm.get('name'); }
  get description() { return this.productForm.get('description'); }
  get price() { return this.productForm.get('price'); }
  get stock() { return this.productForm.get('stock'); }
  get category() { return this.productForm.get('category'); }
  get imageUrl() { return this.productForm.get('imageUrl'); }
}