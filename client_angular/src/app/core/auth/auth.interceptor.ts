import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Inject OAuthService directly to break the circular dependency loop with AuthService
  const oauthService = inject(OAuthService);
  const token = oauthService.getAccessToken();

  // Bypass token attachment for the OIDC discovery document request
  if (req.url.includes('/.well-known/openid-configuration')) {
    return next(req);
  }

  // Clone the request and attach the Bearer token if it exists
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req);
};