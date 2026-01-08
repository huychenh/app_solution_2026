import { useEffect } from "react";
import { userManager } from "../authentication/auth-service";

export default function SignoutOidc() {
  useEffect(() => {
    userManager.signoutRedirectCallback().then(() => {      
      window.location.href = "/";
    }).catch(err => {
      console.error("Signout callback error:", err);
    });
  }, []);

  return <p>Logging out...</p>;
}
