import { AuthConfig } from 'angular-oauth2-oidc';

export const authCodeFlowConfig: AuthConfig = {
  // Url of the Identity Provider
  issuer: 'https://localhost:7025',

  // MUST match exactly with the RedirectUris configured in Duende IdentityServer
  redirectUri: window.location.origin + '/signin-oidc',

  // The Angular app's client ID (This one is correct!)
  clientId: 'shop_online_angular_client',

  // Authorization Code Flow + PKCE
  responseType: 'code',

  // MUST match exactly with AllowedScopes in Backend Config.cs
  // Replaced 'api' with 'shop_online_api' and 'role' with 'roles'
  scope: 'openid profile shop_online_api roles offline_access',

  // Set to false for production to disable console debugging logs
  showDebugInformation: true,
};