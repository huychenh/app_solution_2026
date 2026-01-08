import { Injectable } from '@angular/core';
import { UserManager, User, WebStorageStateStore } from 'oidc-client-ts';
import { BehaviorSubject, from, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private userManager: UserManager;
  private _user$ = new BehaviorSubject<User | null>(null);

  constructor() {
    const config = {
      authority: 'https://localhost:7025',
      client_id: 'shop_online_angular_client',
      redirect_uri: 'http://localhost:4200/signin-oidc',
      post_logout_redirect_uri: 'http://localhost:4200/signout-callback-oidc',
      response_type: 'code',
      scope: 'openid profile shop_online_api',
      automaticSilentRenew: true,
      userStore: new WebStorageStateStore({ store: window.localStorage })
    };

    this.userManager = new UserManager(config);

    //check user on load
    this.userManager.getUser().then(user => {
      this._user$.next(user);
    });
  }

  get user$() {
    return this._user$.asObservable();
  }

  login() {
    this.userManager.signinRedirect();
  }

  async completeLogin() {
    const user = await this.userManager.signinRedirectCallback();
    console.log(user.profile);
    this._user$.next(user);
  }

  logout() {
    this.userManager.signoutRedirect();
  }

  async completeLogout() {
    await this.userManager.signoutRedirectCallback();
    this._user$.next(null);
  }

  getAccessToken(): Observable<string | null> {
    return from(this.userManager.getUser().then(user => user?.access_token || null));
  }


  isAuthenticated(): boolean {
    return !!this._user$.value;
  }

  getRole(): string | null {
    const role = this._user$.value?.profile?.['role'];

    if (typeof role === 'string') {
      return role;
    }

    return null;
  }

  isAdminRole(): boolean {
    return this.getRole()?.toUpperCase() === 'ADMIN';
  }

}
