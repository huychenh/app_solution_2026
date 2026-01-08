import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from './services/auth.service';
import { User } from 'oidc-client-ts';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('simple-crud');


  user$: Observable<User | null>;

  constructor(private auth: AuthService) {
    this.user$ = this.auth.user$;
  }

  login() {
    this.auth.login();
  }

  logout() {
    this.auth.logout();
  }
}
