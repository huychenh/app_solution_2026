import { useState, useEffect } from "react";
import type { User } from "../../types/user";
import type { UserUpdateProps } from "../../interfaces/user-update-props";
import "./User.css";
import { GET_USER_BY_ID_URL, UPDATE_USER_URL } from "../../api/endpoints";
import { userManager } from "../../authentication/auth-service";
import type { UserForm } from "../../types/user-form";

export default function UserUpdate({
  userId,
  onClose,
  onSuccess,
}: UserUpdateProps) {
  const [user, setUser] = useState<User | null>(null);

  const [userForm, setUserForm] = useState<UserForm>({
    firstName: "",
    lastName: "",
    email: "",
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Load user info
  useEffect(() => {
    if (!userId) return;

    setLoading(true);

    fetch(GET_USER_BY_ID_URL(userId))
      .then((res) => {
        if (!res.ok) throw new Error("User not found");
        return res.json();
      })
      .then((data: User) => {
        setUser(data);
        setUserForm({
          firstName: data.firstName,
          lastName: data.lastName,
          email: data.email,
        });
        setLoading(false);
      })
      .catch((err) => {
        setError(err.message);
        setLoading(false);
      });
  }, [userId]);

  // Handle update
  const handleUpdateUser = async () => {
    const { firstName, lastName, email } = userForm;

    if (!firstName || !lastName || !email) {
      setError("Please fill in all fields");
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const oidcUser = await userManager.getUser();

      if (!oidcUser || oidcUser.expired) {
        throw new Error("User not authenticated");
      }

      const res = await fetch(UPDATE_USER_URL(userId), {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${oidcUser.access_token}`,
        },
        body: JSON.stringify(userForm),
      });

      if (!res.ok) {
        const msg = await res.text();
        throw new Error(msg || "Error updating user");
      }

      onSuccess?.();
      onClose();
    } catch (err: any) {
      setError(err.message || "Error updating user");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-box modal-animate">
        <div className="modal-header">
          <h2>Update User</h2>
        </div>

        {loading && <p className="modal-loading">Processing...</p>}
        {error && <p style={{ color: "red" }}>{error}</p>}

        {user && (
          <div className="modal-info-wrapper">
            {/* FIRST NAME */}
            <div className="modal-info-row">
              <span className="modal-label">First Name:</span>
              <input
                type="text"
                value={userForm.firstName}
                onChange={(e) =>
                  setUserForm({ ...userForm, firstName: e.target.value })
                }
                className="modal-input"
              />
            </div>

            {/* LAST NAME */}
            <div className="modal-info-row">
              <span className="modal-label">Last Name:</span>
              <input
                type="text"
                value={userForm.lastName}
                onChange={(e) =>
                  setUserForm({ ...userForm, lastName: e.target.value })
                }
                className="modal-input"
              />
            </div>

            {/* EMAIL */}
            <div className="modal-info-row">
              <span className="modal-label">Email:</span>
              <input
                type="email"
                value={userForm.email}
                onChange={(e) =>
                  setUserForm({ ...userForm, email: e.target.value })
                }
                className="modal-input"
              />
            </div>
          </div>
        )}

        <div style={{ padding: "0 20px 20px" }}>
          {user && (
            <button
              className="modal-button"
              onClick={handleUpdateUser}
              style={{ background: "#007bff" }}
            >
              Update
            </button>
          )}

          <button
            className="modal-button"
            style={{ marginLeft: 10, background: "#6c757d" }}
            onClick={onClose}
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
}
