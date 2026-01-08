import { useState } from "react";
import "./User.css";
import type { UserAddProps } from "../../interfaces/user-add-props";
import { CREATE_USER_URL } from "../../api/endpoints";
import { userManager } from "../../authentication/auth-service";
import type { UserForm } from "../../types/user-form";

export default function UserAdd({ onClose, onSuccess }: UserAddProps) {
  const [userForm, setUserForm] = useState<UserForm>({
    firstName: "",
    lastName: "",
    email: "",
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setUserForm(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleAddUser = async () => {
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

      const response = await fetch(CREATE_USER_URL, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${oidcUser.access_token}`,
        },
        body: JSON.stringify(userForm),
      });

      if (!response.ok) {
        const msg = await response.text();
        throw new Error(msg || "Failed to add user");
      }

      await response.json();

      onSuccess?.();
      onClose();

      // reset form
      setUserForm({
        firstName: "",
        lastName: "",
        email: "",
      });
    } catch (err: any) {
      setError(err.message || "Error adding user");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-box modal-animate">
        <div className="modal-header">
          <h2>Add New User</h2>
        </div>

        <div className="modal-info-wrapper">
          <div className="modal-info-row">
            <span className="modal-label">First Name:</span>
            <span className="modal-value">
              <input
                type="text"
                name="firstName"
                placeholder="First Name"
                value={userForm.firstName}
                onChange={handleChange}
                className="modal-input"
              />
            </span>
          </div>

          <div className="modal-info-row">
            <span className="modal-label">Last Name:</span>
            <span className="modal-value">
              <input
                type="text"
                name="lastName"
                placeholder="Last Name"
                value={userForm.lastName}
                onChange={handleChange}
                className="modal-input"
              />
            </span>
          </div>

          <div className="modal-info-row">
            <span className="modal-label">Email:</span>
            <span className="modal-value">
              <input
                type="email"
                name="email"
                placeholder="Email"
                value={userForm.email}
                onChange={handleChange}
                className="modal-input"
              />
            </span>
          </div>

          {error && (
            <p className="modal-loading" style={{ color: "red" }}>
              {error}
            </p>
          )}
          {loading && <p className="modal-loading">Adding user...</p>}
        </div>

        <div style={{ padding: "0 20px 20px 20px" }}>
          <button className="modal-button" onClick={handleAddUser} disabled={loading}>
            Save
          </button>
          <button
            className="modal-button"
            style={{ marginLeft: 10, background: "#6c757d" }}
            onClick={onClose}
            disabled={loading}
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
}
