import { useEffect, useState } from "react";
import type { User } from "../../types/user";
import type { UserListProps } from "../../interfaces/user-list-props";
import { GET_USERS_LIST_URL } from "../../api/endpoints";

export default function UserList({
  onViewUser,
  onEditUser,
  onDeleteUser,
  refresh,
  loggedInUser,
}: UserListProps) {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    fetch(GET_USERS_LIST_URL)
      .then((res) => res.json())
      .then((data) => {
        console.log("API DATA:", data);
        setUsers(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error("Error when fetch API:", err);
        setLoading(false);
      });
  }, [refresh]);

  if (loading) return <p>Loading...</p>;

  return (
    <div>
      <table
        style={{ borderCollapse: "collapse", width: "100%" }}
        className="user-table"
      >
        <thead>
          <tr>
            <th style={{ border: "1px solid #ccc", padding: "8px" }}>No.</th>
            <th style={{ border: "1px solid #ccc", padding: "8px" }}>
              First Name
            </th>
            <th style={{ border: "1px solid #ccc", padding: "8px" }}>
              Last Name
            </th>
            <th style={{ border: "1px solid #ccc", padding: "8px" }}>Email</th>

            {/* NEW FIELDS */}
            <th style={{ border: "1px solid #ccc", padding: "8px" }}>
              Created Date
            </th>
            <th style={{ border: "1px solid #ccc", padding: "8px" }}>
              Updated Date
            </th>

            <th style={{ border: "1px solid #ccc", padding: "8px" }}>
              Actions
            </th>
          </tr>
        </thead>
        <tbody>
          {users.map((user, index) => (
            <tr key={user.id}>
              <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                {index + 1}
              </td>
              <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                {user.firstName}
              </td>
              <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                {user.lastName}
              </td>
              <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                {user.email}
              </td>

              {/* NEW FIELDS */}
              <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                {new Date(user.createdDate).toLocaleString()}
              </td>
              <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                {new Date(user.updatedDate).toLocaleString()}
              </td>

              <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                <a
                  href="#"
                  onClick={() => onViewUser && onViewUser(user.id)}
                  style={{ marginRight: 8 }}
                >
                  View
                </a>
                |
                <a
                  href="#"
                  style={{ marginLeft: "8px", marginRight: "8px" }}
                  onClick={(e) => {
                    e.preventDefault();
                    if (loggedInUser?.role.toLocaleLowerCase() !== "admin") {
                      alert("You are not allowed to edit users.");
                      return;
                    }
                    onEditUser && onEditUser(user.id);
                  }}
                >
                  Edit
                </a>
                |
                <a
                  href="#"
                  style={{ marginLeft: "8px", color: "red" }}
                  onClick={(e) => {
                    e.preventDefault();
                    if (loggedInUser?.role.toLocaleLowerCase() !== "admin") {
                      alert("You are not allowed to delete users.");
                      return;
                    }
                    onDeleteUser && onDeleteUser(user.id);
                  }}
                >
                  Delete
                </a>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
