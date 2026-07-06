import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { CategoryListComponent } from './pages/categories/category-list/category-list.component';
import { authGuard } from './core/guards/auth.guard';
import { CategoryCreateComponent } from './pages/categories/category-create/category-create.component';
import { CategoryEditComponent } from './pages/categories/category-edit/category-edit.component';


export const routes: Routes = [
  
  { path: '', component: HomeComponent },
  
  { path: 'signin-oidc', component: HomeComponent },
  { path: 'signout-callback-oidc', redirectTo: '', pathMatch: 'full' },

  { 
    path: 'categories', 
    component: CategoryListComponent, 
    canActivate: [authGuard]
  }, 

  { 
    path: 'categories/add', 
    component: CategoryCreateComponent 
  },
  { 
    path: 'categories/edit/:id', 
    component: CategoryEditComponent 
  },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];