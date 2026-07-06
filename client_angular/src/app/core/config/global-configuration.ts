// src/app/core/config/global-configuration.ts

export const GlobalConfiguration = {
  // API base URL configuration
  apiBaseUrl: 'https://localhost:7210/api',

  // Authentication Settings (OIDC / OAuth2 Flow)
  authenticationSetting: {
    authority: 'https://localhost:7025',
    client_id: 'client_angular',        
    redirect_uri: 'https://localhost:4200/signin-oidc',
    monitorSession: false,
    post_logout_redirect_uri: 'https://localhost:4200/signout-callback-oidc',
    response_type: 'code', // Authorization Code Flow with PKCE
    scope: 'openid profile email phone',
    automaticSilentRenew: true,
    includeIdTokenInSilentRenew: true
  }
};