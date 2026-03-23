import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { getProject } from '../../api/projects'
import { getTasksByProject, createTask, updateTask, deleteTask } from '../../api/projectTasks'

const STATUS_OPTIONS = ['ToDo', 'InProgress', 'Done']

const STATUS_MAP = {
  'ToDo': 0,
  'InProgress': 1,
  'Done': 2
}

const ProjectPage = () => {
  const { id } = useParams()
  const navigate = useNavigate()
  const [project, setProject] = useState(null)
  const [tasks, setTasks] = useState([])
  const [newTask, setNewTask] = useState({ name: '', description: '', dueDate: '' })
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    fetchData()
  }, [id])

  const fetchData = async () => {
    try {
      const [projectRes, tasksRes] = await Promise.all([
        getProject(id),
        getTasksByProject(id)
      ])
      setProject(projectRes.data)
      setTasks(tasksRes.data)
    } catch (err) {
      setError('Failed to load project')
    } finally {
      setLoading(false)
    }
  }

  const handleCreateTask = async (e) => {
    e.preventDefault()
    try {
      await createTask({ ...newTask, projectId: parseInt(id) })
      setNewTask({ name: '', description: '', dueDate: '' })
      fetchData()
    } catch (err) {
      setError('Failed to create task')
    }
  }

    const handleStatusChange = async (task, newStatus) => {
    try {
        await updateTask(task.id, {
        ...task,
        status: STATUS_MAP[newStatus]
        })
        fetchData()
    } catch (err) {
        setError('Failed to update task')
    }
    }

  const handleDeleteTask = async (taskId) => {
    try {
      await deleteTask(taskId)
      fetchData()
    } catch (err) {
      setError('Failed to delete task')
    }
  }

  if (loading) return <div>Loading...</div>
  if (!project) return <div>Project not found</div>

    const tasksByStatus = {
    ToDo: tasks.filter(t => t.status === 0),
    InProgress: tasks.filter(t => t.status === 1),
    Done: tasks.filter(t => t.status === 2)
    }

  return (
    <div style={{ maxWidth: '1000px', margin: '2rem auto', padding: '2rem' }}>
      <button onClick={() => navigate('/dashboard')}>← Back</button>
      <h1>{project.name}</h1>
      <p>{project.description}</p>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      <h2>Add Task</h2>
      <form onSubmit={handleCreateTask}>
        <input
          type="text"
          placeholder="Task name"
          value={newTask.name}
          onChange={(e) => setNewTask({ ...newTask, name: e.target.value })}
          style={{ padding: '0.5rem', marginRight: '0.5rem' }}
          required
        />
        <input
          type="text"
          placeholder="Description"
          value={newTask.description}
          onChange={(e) => setNewTask({ ...newTask, description: e.target.value })}
          style={{ padding: '0.5rem', marginRight: '0.5rem' }}
        />
        <input
          type="date"
          value={newTask.dueDate}
          onChange={(e) => setNewTask({ ...newTask, dueDate: e.target.value })}
          style={{ padding: '0.5rem', marginRight: '0.5rem' }}
        />
        <button type="submit">Add Task</button>
      </form>

      <h2>Tasks</h2>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '1rem', marginTop: '1rem' }}>
        {STATUS_OPTIONS.map((status) => (
          <div key={status} style={{ border: '1px solid #ccc', borderRadius: '8px', padding: '1rem' }}>
            <h3>{status}</h3>
            {tasksByStatus[status].length === 0 ? (
              <p style={{ color: '#999' }}>No tasks</p>
            ) : (
              tasksByStatus[status].map((task) => (
                <div key={task.id} style={{
                  background: '#f9f9f9',
                  borderRadius: '6px',
                  padding: '0.75rem',
                  marginBottom: '0.5rem'
                }}>
                  <strong>{task.name}</strong>
                  <p style={{ fontSize: '0.85rem', color: '#666' }}>{task.description}</p>
                  <select
                    value={status}
                    onChange={(e) => handleStatusChange(task, e.target.value)}
                    style={{ marginRight: '0.5rem' }}
                  >
                    {STATUS_OPTIONS.map(s => <option key={s} value={s}>{s}</option>)}
                  </select>
                  <button onClick={() => handleDeleteTask(task.id)} style={{ color: 'red' }}>Delete</button>
                </div>
              ))
            )}
          </div>
        ))}
      </div>
    </div>
  )
}

export default ProjectPage