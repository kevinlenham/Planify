import client from './client'

export const register = (data) => client.post('/users/register', data)
export const login = (data) => client.post('/users/login', data)
export const getUser = (id) => client.get(`/users/${id}`)
export const updateUser = (id, data) => client.put(`/users/${id}`, data)
export const changePassword = (id, data) => client.put(`/users/change-password/${id}`, data)
export const deleteUser = (id) => client.delete(`/users/${id}`)