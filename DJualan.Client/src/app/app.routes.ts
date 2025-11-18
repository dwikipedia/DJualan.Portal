import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductDetailComponent } from './components/product-detail/product-detail.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { authGuard } from './guards/auth.guard';
import { AddProductComponent } from './components/add-product/add-product.component';
import { EditProductComponent } from './components/edit-product/edit-product.component';
import { DeleteProductComponent } from './components/delete-product/delete-product.component';

export const routes: Routes = [
  { path: '', component: LoginComponent, pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { 
    path: 'dashboard', 
    component: DashboardComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'products', 
    component: ProductListComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'products/add', 
    component: AddProductComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'products/edit/:id', 
    component: EditProductComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'products/delete/:id', 
    component: DeleteProductComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'products/:id', 
    component: ProductDetailComponent,
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '' }
];