import type { UserInfo } from "./user-info";

export interface UserListProps {
  onViewUser?: (userId: number) => void;
  onEditUser?: (userId: number) => void;
  onDeleteUser?: (userId: number) => void;
  refresh: number;
  loggedInUser: UserInfo | null;
}
