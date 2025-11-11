// src/app/app.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, RouterOutlet, Router } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'DJualan.Client';

  constructor(private authService: AuthService, private router: Router) {}

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  // src/app/app.component.ts - update the getUserName method
  getUserName(): string {
    // Get the actual user name from auth service
    return this.authService.getCurrentUser();
  }

  // Optional: Add a method to get full name if needed
  getUserFullName(): string {
    return this.authService.getFullName();
  }

  // Optional: Add a method to get greeting with proper capitalization
  getUserGreeting(): string {
    const name = this.authService.getCurrentUser();
    return name.charAt(0).toUpperCase() + name.slice(1);
  }

  onSearch(searchTerm: string): void {
    if (searchTerm.trim()) {
      console.log('Searching for:', searchTerm);
      // Implement search functionality
      // this.router.navigate(['/search'], { queryParams: { q: searchTerm } });
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  ngOnInit(): void {
    // Redirect to products if already logged in and on login page
    if (this.authService.isLoggedIn() && this.router.url === '/') {
      this.router.navigate(['/dashboard']);
    }
  }
}
