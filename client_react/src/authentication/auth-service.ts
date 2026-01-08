import { UserManager, WebStorageStateStore } from "oidc-client-ts";

const config = {
  authority: "https://localhost:7025", 
  client_id: "shop_online_react_client",
  redirect_uri: "http://localhost:5173/signin-oidc",
  post_logout_redirect_uri: "http://localhost:5173/signout-callback-oidc",
  response_type: "code",
  scope: "openid profile shop_online_api",
  automaticSilentRenew: true,
  userStore: new WebStorageStateStore({ store: window.localStorage })
};

export const userManager = new UserManager(config);
