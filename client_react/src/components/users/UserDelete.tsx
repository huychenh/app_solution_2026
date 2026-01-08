import { useState, useEffect } from "react";
import type { User } from "../../types/user";

import type { UserDeleteProps } from "../../interfaces/user-delete-props";
import "./User.css";
import { DELETE_USER_URL, GET_USER_BY_ID_URL } from "../../api/endpoints";
import { userManager } from "../../authentication/auth-service";

export default function UserDelete({ userId, onClose, onSuccess }: UserDeleteProps) {
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!userId) return;

        setLoading(true);
        setError(null);

        fetch(GET_USER_BY_ID_URL(userId))
            .then(res => {
                if (!res.ok) throw new Error("User not found");
                return res.json();
            })
            .then((data: User) => {
                setUser(data);
                setLoading(false);
            })
            .catch((err) => {
                setError(err.message);
                setLoading(false);
            });
    }, [userId]);

    const handleDeleteUser = async () => {
        if (!userId) return;

        setLoading(true);
        setError(null);

        try {
            const oidcUser = await userManager.getUser();

            if (!oidcUser || oidcUser.expired) {
                throw new Error("User not authenticated");
            }

            const res = await fetch(DELETE_USER_URL(userId), {
                method: "DELETE",
                headers: {
                    "Authorization": `Bearer ${oidcUser.access_token}`,
                },
            });

            if (!res.ok) {
                const msg = await res.text();
                throw new Error(msg || "User not found or cannot delete");
            }

            onSuccess?.(); // refresh + info message
            onClose();     // close modal
        } catch (err: any) {
            setError(err.message || "Error deleting user");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="modal-overlay">
            <div className="modal-box modal-animate">
                <div className="modal-header">
                    <h2>Delete User</h2>
                </div>

                {loading && <p className="modal-loading">Processing...</p>}
                {error && <p className="modal-loading" style={{ color: "red" }}>{error}</p>}

                {user && (
                    <div className="modal-info-wrapper">

                        <div className="modal-info-row">
                            <span className="modal-label">ID:</span>
                            <span className="modal-value">{user.id}</span>
                        </div>

                        <div className="modal-info-row">
                            <span className="modal-label">First Name:</span>
                            <span className="modal-value">{user.firstName}</span>
                        </div>

                        <div className="modal-info-row">
                            <span className="modal-label">Last Name:</span>
                            <span className="modal-value">{user.lastName}</span>
                        </div>

                        <div className="modal-info-row">
                            <span className="modal-label">Email:</span>
                            <span className="modal-value">{user.email}</span>
                        </div>

                        {/* NEW FIELDS */}
                        <div className="modal-info-row">
                            <span className="modal-label">Created Date:</span>
                            <span className="modal-value">
                                {new Date(user.createdDate).toLocaleString()}
                            </span>
                        </div>

                        <div className="modal-info-row">
                            <span className="modal-label">Updated Date:</span>
                            <span className="modal-value">
                                {new Date(user.updatedDate).toLocaleString()}
                            </span>
                        </div>

                    </div>
                )}

                <div style={{ padding: "0 20px 20px 20px" }}>
                    <button
                        className="modal-button"
                        onClick={handleDeleteUser}
                        style={{ background: "#dc3545" }}
                    >
                        Delete
                    </button>

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
