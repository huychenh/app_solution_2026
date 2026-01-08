export const BASE_API_URL = "https://localhost:7210/api";

//Users
export const GET_USERS_LIST_URL = `${BASE_API_URL}/users/list`;
export const CREATE_USER_URL = `${BASE_API_URL}/users/create`;

export const GET_USER_BY_ID_URL = (userId: number | string) => `${BASE_API_URL}/users/getbyid/${userId}`;
export const UPDATE_USER_URL = (userId: number | string) => `${BASE_API_URL}/users/update/${userId}`;
export const DELETE_USER_URL = (userId: number | string) => `${BASE_API_URL}/users/delete/${userId}`;