import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import { getProjectsByUser, createProject, deleteProject } from '../../api/projects'

const DashboardPage = () => {
  const [projects, setProjects] = useState([])
  const [newProjectName, setNewProjectName] = useState('')
  const [newProjectDescription, setNewProjectDescription] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  useEffect(() => {
    fetchProjects()
  }, [])

  const fetchProjects = async () => {
    try {
      const response = await getProjectsByUser(user.userId)
      setProjects(response.data)
    } catch (err) {
      setError('Failed to load projects')
    } finally {
      setLoading(false)
    }
  }

  const handleCreateProject = async (e) => {
    e.preventDefault()
    try {
      await createProject({
        name: newProjectName,
        description: newProjectDescription,
        ownerId: user.userId
      })
      setNewProjectName('')
      setNewProjectDescription('')
      fetchProjects()
    } catch (err) {
      setError('Failed to create project')
    }
  }

  const handleDeleteProject = async (id) => {
    try {
      await deleteProject(id)
      fetchProjects()
    } catch (err) {
      setError('Failed to delete project')
    }
  }

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  if (loading) return <div>Loading...</div>

  return (
    <div style={{ maxWidth: '800px', margin: '2rem auto', padding: '2rem' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h1>Welcome, {user.firstName}!</h1>
        <button onClick={handleLogout}>Logout</button>
      </div>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      <h2>Create New Project</h2>
      <form onSubmit={handleCreateProject}>
        <div style={{ marginBottom: '1rem' }}>
          <input
            type="text"
            placeholder="Project name"
            value={newProjectName}
            onChange={(e) => setNewProjectName(e.target.value)}
            style={{ padding: '0.5rem', marginRight: '1rem' }}
            required
          />
          <input
            type="text"
            placeholder="Description"
            value={newProjectDescription}
            onChange={(e) => setNewProjectDescription(e.target.value)}
            style={{ padding: '0.5rem', marginRight: '1rem' }}
          />
          <button type="submit">Create</button>
        </div>
      </form>

      <h2>Your Projects</h2>
      {projects.length === 0 ? (
        <p>No projects yet — create one above!</p>
      ) : (
        projects.map((project) => (
          <div key={project.id} style={{
            border: '1px solid #ccc',
            borderRadius: '8px',
            padding: '1rem',
            marginBottom: '1rem',
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center'
          }}>
            <div>
              <h3 style={{ margin: 0 }}>{project.name}</h3>
              <p style={{ margin: 0, color: '#666' }}>{project.description}</p>
            </div>
            <div>
              <button onClick={() => navigate(`/projects/${project.id}`)} style={{ marginRight: '0.5rem' }}>
                Open
              </button>
              <button onClick={() => handleDeleteProject(project.id)} style={{ color: 'red' }}>
                Delete
              </button>
            </div>
          </div>
        ))
      )}
    </div>
  )
}

export default DashboardPage