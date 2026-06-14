// src/app/core/config/global-configuration.ts

export const GlobalConfiguration = {
  // API base URL configuration
  apiBaseUrl: 'https://localhost:7210/api',

  // Authentication Settings (OIDC / OAuth2 Flow)
  authenticationSetting: {
    authority: 'https://localhost:44321/',
    client_id: 'angular_client',
    redirect_uri: 'http://localhost:4200/login-callback',
    monitorSession: false,
    post_logout_redirect_uri: 'http://localhost:4200/logout-callback',
    response_type: 'code', // Authorization Code Flow with PKCE
    scope: 'openid profile email phone',
    automaticSilentRenew: true,
    includeIdTokenInSilentRenew: true
  }
};