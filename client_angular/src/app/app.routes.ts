import { Routes } from '@angular/router';
import { CategoryListComponent } from './pages/categories/category-list/category-list.component';
import { HomeComponent } from './pages/home/home.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'signin-oidc', component: HomeComponent },
  { path: 'signout-callback-oidc', redirectTo: '', pathMatch: 'full' },
  { path: 'categories', component: CategoryListComponent },
  //{ path: 'categories/add', component: CategoryFormComponent },
  //{ path: 'categories/edit/:id', component: CategoryFormComponent },
  //{ path: 'products', component: CategoryListComponent } // Trỏ tạm về để giữ mạch route
  // Default fallback route when the URL path is completely empty
  { path: '', redirectTo: '', pathMatch: 'full' },

  // Wildcard fallback route to handle unexpected 404 URL paths safely
  { path: '**', redirectTo: 'home' }
];