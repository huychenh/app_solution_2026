import { useEffect } from "react";
import { userManager } from "../authentication/auth-service";

export default function SigninOidc() {
  useEffect(() => {
    userManager.signinRedirectCallback().then(user => {
      console.log("Login success:", user);
      window.location.href = "/";
    });
  }, []);

  return <div>Processing login...</div>;
}
