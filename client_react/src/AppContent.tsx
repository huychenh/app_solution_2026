import reactLogo from "./assets/react.svg";
import "./App.css";
import UserList from "./components/users/UserList";
import UserUpdate from "./components/users/UserUpdate";
import UserDelete from "./components/users/UserDelete";
import UserAdd from "./components/users/UserAdd";
import { useEffect, useState } from "react";
import UserDetail from "./components/users/UserDetail";
import type { UserInfo } from "./interfaces/user-info";
import { userManager } from "./authentication/auth-service";

export default function AppContent() {
  const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [showAddModal, setShowAddModal] = useState(false);
  const [showUpdateModal, setShowUpdateModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);

  const [message, setMessage] = useState<string | null>(null);
  const [refreshFlag, setRefreshFlag] = useState(0);

  // State login
  const [user, setUser] = useState<UserInfo | null>(null);

  const handleViewUser = (userId: number) => {
    setSelectedUserId(userId);
    setShowDetailModal(true);
  };
  const handleEditUser = (userId: number) => {
    setSelectedUserId(userId);
    setShowUpdateModal(true);
  };
  const handleDeleteUser = (userId: number) => {
    setSelectedUserId(userId);
    setShowDeleteModal(true);
  };
  const handleLogin = () => {
    userManager.signinRedirect();
  };

  const handleLogout = () => {
    userManager.signoutRedirect();
    setUser(null);
  };

  // Auto hide message
  useEffect(() => {
    if (!message) return;
    const timer = setTimeout(() => setMessage(null), 5000);
    return () => clearTimeout(timer);
  }, [message]);

  useEffect(() => {
    userManager.getUser().then((oidcUser) => {
      console.log("OIDC user claims:", oidcUser?.profile);

      if (oidcUser && !oidcUser.expired) {
        const rawRole = oidcUser.profile?.role;
        const role = typeof rawRole === "string" ? rawRole : "";

        setUser({
          name:
            oidcUser.profile?.name ||
            oidcUser.profile?.preferred_username ||
            "User",
          role,
        });
      }
    });
  }, []);

  return (
    <>
      {/* Header + User menu */}
      <div style={{ display: "flex", alignItems: "center", gap: 16 }}>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
        <h1>Simple CRUD React</h1>
      </div>

      <div
        style={{
          display: "flex",
          justifyContent: "flex-end",
          marginTop: 10,
          marginBottom: 20,
        }}
      >
        {!user ? (
          <div>
            <span style={{ fontWeight: 500, marginRight: 12 }}>Hi Guest</span> |
            &nbsp;
            <a
              href="#"
              onClick={(e) => {
                e.preventDefault();
                handleLogin();
              }}
              style={{
                textDecoration: "none",
                color: "#007bff",
                fontWeight: 500,
                cursor: "pointer",
              }}
            >
              Login
            </a>
          </div>
        ) : (
          <div>
            <span style={{ fontWeight: 500, marginRight: 12 }}>
              Hi {user.name} ({user.role})
            </span>{" "}
            | &nbsp;
            <a
              href="#"
              onClick={(e) => {
                e.preventDefault();
                handleLogout();
              }}
              style={{
                textDecoration: "none",
                color: "#dc3545",
                fontWeight: 500,
                cursor: "pointer",
              }}
            >
              Logout
            </a>
          </div>
        )}
      </div>

      {message && (
        <div
          style={{
            background: "#d1e7dd",
            border: "1px solid #0f5132",
            padding: "10px 15px",
            borderRadius: 6,
            color: "#0f5132",
            marginTop: 10,
          }}
        >
          {message}
        </div>
      )}

      {/* User List Header */}
      <div className="header-container" style={{ marginTop: 20 }}>
        <h2>User List</h2>
        <button
          className="add-user-button"
          onClick={() => {
            if (user?.role.toLocaleLowerCase() !== "admin") {
              alert("You do not have rights to do this.");
              return;
            }
            setShowAddModal(true);
          }}
        >
          Add New User
        </button>
      </div>

      {/* List */}
      <UserList
        refresh={refreshFlag}
        onViewUser={handleViewUser}
        onEditUser={handleEditUser}
        onDeleteUser={handleDeleteUser}
        loggedInUser={user}
      />

      {/* Modals */}
      {showDetailModal && selectedUserId && (
        <UserDetail
          userId={selectedUserId}
          onClose={() => setShowDetailModal(false)}
        />
      )}
      {showAddModal && (
        <UserAdd
          onClose={() => setShowAddModal(false)}
          onSuccess={() => {
            setMessage("User added successfully!");
            setRefreshFlag((prev) => prev + 1);
            setShowAddModal(false);
          }}
        />
      )}
      {showUpdateModal && selectedUserId && (
        <UserUpdate
          userId={selectedUserId}
          onClose={() => setShowUpdateModal(false)}
          onSuccess={() => {
            setMessage("User updated successfully!");
            setRefreshFlag((prev) => prev + 1);
          }}
        />
      )}
      {showDeleteModal && selectedUserId && (
        <UserDelete
          userId={selectedUserId}
          onClose={() => setShowDeleteModal(false)}
          onSuccess={() => {
            setMessage("User deleted successfully!");
            setRefreshFlag((prev) => prev + 1);
            setShowDeleteModal(false);
          }}
        />
      )}
    </>
  );
}
