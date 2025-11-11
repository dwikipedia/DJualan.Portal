import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductDetailComponent } from './components/product-detail/product-detail.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: LoginComponent, pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { 
    path: 'dashboard', 
    component: DashboardComponent,
    canActivate: [authGuard]  // Make sure this is here
  },
  { 
    path: 'products', 
    component: ProductListComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'products/:id', 
    component: ProductDetailComponent,
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '' }
];