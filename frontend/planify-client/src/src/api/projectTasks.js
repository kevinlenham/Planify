import client from './client'

export const getTasksByProject = (projectId) => client.get(`/projecttasks/project/${projectId}`)
export const getTask = (id) => client.get(`/projecttasks/${id}`)
export const createTask = (data) => client.post('/projecttasks', data)
export const updateTask = (id, data) => client.put(`/projecttasks/${id}`, data)
export const deleteTask = (id) => client.delete(`/projecttasks/${id}`)