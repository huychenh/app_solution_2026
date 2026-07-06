import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router'; // Import Router to clear URL
import { OAuthService } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from './auth.config';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private oauthService = inject(OAuthService);
  private router = inject(Router); // Inject Angular Router

  constructor() {
    this.oauthService.configure(authCodeFlowConfig);
    
    // Listen to OAuth events to detect when token is successfully received
    this.oauthService.events.subscribe(event => {
    if (event.type === 'token_received') {      
      const returnUrl = sessionStorage.getItem('returnUrl');
      
      if (returnUrl) {
        sessionStorage.removeItem('returnUrl');
        this.router.navigateByUrl(returnUrl);
      } else {
        this.router.navigate(['/']);
      }
    }
  });

    // Load discovery document and parse the tokens from URL query parameters
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
  }

 login(returnUrl?: string): void {
  if (returnUrl) {
    sessionStorage.setItem('returnUrl', returnUrl);
  }
  this.oauthService.initLoginFlow();
}

  logout(): void {
    this.oauthService.logOut();
  }

  get isLoggedIn(): boolean {
    return this.oauthService.hasValidAccessToken() && this.oauthService.hasValidIdToken();
  }

  get identityClaims(): any {
    return this.oauthService.getIdentityClaims();
  }

  get accessToken(): string {
    return this.oauthService.getAccessToken();
  }
}