import { Routes } from '@angular/router';
import { CategoryListComponent } from './components/category/category-list/category-list.component';
import { HomeComponent } from './components/home/home.component';
import { AuthGuard } from './services/auth.guard';
import { SignInOidcComponent } from './components/authentication/signin-oidc/signin-oidc.component';
import { SignoutOidcComponent } from './components/authentication/signout-oidc/signout-oidc.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'categories', component: CategoryListComponent, canActivate: [AuthGuard] },
  //{ path: 'products', component: ProductListComponent }
  { path: 'signin-oidc', component: SignInOidcComponent },
  { path: 'signout-callback-oidc', component: SignoutOidcComponent },
  { path: '**', redirectTo: '' }
];
