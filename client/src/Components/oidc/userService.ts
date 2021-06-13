import { UserManager } from 'oidc-client';

const config = {
  authority: "https://localhost:5001",
  client_id: "spa",
  redirect_uri: "http://localhost:3000/signin-oidc",
  response_type: "code",
  scope: "openid profile read",
  post_logout_redirect_uri: "http://localhost:3000/signout-oidc",
};

export const userManager = new UserManager(config)


export function signinRedirect() {
  return userManager.signinRedirect()
}

export function signinRedirectCallback() {
  return userManager.signinRedirectCallback()
}

export function signoutRedirect() {
  userManager.clearStaleState()
  userManager.removeUser()
  return userManager.signoutRedirect()
}

export function signoutRedirectCallback() {
  userManager.clearStaleState()
  userManager.removeUser()
  return userManager.signoutRedirectCallback()
}

export default userManager