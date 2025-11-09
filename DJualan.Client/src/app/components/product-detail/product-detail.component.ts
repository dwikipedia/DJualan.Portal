// src/app/components/product-detail/product-detail.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    ReactiveFormsModule
  ],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {
  product: Product | null = null;
  loading: boolean = true;
  error: string = '';
  selectedImage: string = '';
  productForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private cartService: CartService,
    private fb: FormBuilder
  ) {
    this.productForm = this.fb.group({
      quantity: [1, [Validators.required, Validators.min(1)]]
    });
  }

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
        
        // Update form validators based on stock
        const maxQuantity = product.stock;
        this.quantityControl.setValidators([
          Validators.required,
          Validators.min(1),
          Validators.max(maxQuantity)
        ]);
        this.quantityControl.updateValueAndValidity();
        
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Product not found or failed to load';
        this.loading = false;
        console.error('Error loading product:', error);
      }
    });
  }

  // Getter for quantity control that ensures it's not null
  get quantityControl(): FormControl {
    return this.productForm.get('quantity') as FormControl;
  }

  get currentQuantity(): number {
    return this.quantityControl.value || 1;
  }

  addToCart(): void {
    if (this.productForm.valid && this.product) {
      const quantity = this.quantityControl.value;
      
      alert(`Added ${quantity} ${this.product.name} to cart!`);
      
      // If you have a CartService, you would use:
      // this.cartService.addToCart(this.product, quantity);
    }
  }

  incrementQuantity(): void {
    if (this.product) {
      const current = this.currentQuantity;
      const newQuantity = Math.min(current + 1, this.product.stock);
      this.quantityControl.setValue(newQuantity);
    }
  }

  decrementQuantity(): void {
    const current = this.currentQuantity;
    const newQuantity = Math.max(current - 1, 1);
    this.quantityControl.setValue(newQuantity);
  }

  formatPrice(price: number): string {
    return new Intl.NumberFormat('id-ID', {
      style: 'currency',
      currency: 'IDR',
      minimumFractionDigits: 0
    }).format(price);
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