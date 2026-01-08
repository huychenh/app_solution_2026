import { BrowserRouter, Routes, Route } from "react-router-dom";
import AppContent from "./AppContent";
import SigninOidc from "./pages/SigninOidc";
import SignoutOidc from "./pages/SignoutOidc";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/signin-oidc" element={<SigninOidc />} />
        <Route path="/signout-callback-oidc" element={<SignoutOidc />} />
        <Route path="/*" element={<AppContent />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
