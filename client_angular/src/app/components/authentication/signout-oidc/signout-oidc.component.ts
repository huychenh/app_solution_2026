import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-signout-oidc',
  templateUrl: './signout-oidc.component.html',
  styleUrls: ['./signout-oidc.component.css']
})
export class SignoutOidcComponent implements OnInit {

  constructor(private auth: AuthService, private router: Router) {}

  async ngOnInit(): Promise<void> {
    try {      
      await this.auth.completeLogout();
      this.router.navigate(['/']);
    } catch (error) {
      console.error('Error during signout callback', error);
      this.router.navigate(['/']); // fallback
    }
  }

}
