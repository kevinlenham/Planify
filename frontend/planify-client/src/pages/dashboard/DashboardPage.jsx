import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import { getProjectsByUser, createProject, deleteProject } from '../../api/projects'
import './DashboardPage.css'

const DashboardPage = () => {
  const [projects, setProjects] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [showModal, setShowModal] = useState(false)
  const [newProject, setNewProject] = useState({ name: '', description: '' })
  const [creating, setCreating] = useState(false)
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  // eslint-disable-next-line
  useEffect(() => { fetchProjects() }, [])

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
    setCreating(true)
    try {
      await createProject({ ...newProject, ownerId: user.userId })
      setNewProject({ name: '', description: '' })
      setShowModal(false)
      fetchProjects()
    } catch (err) {
      setError('Failed to create project')
    } finally {
      setCreating(false)
    }
  }

  const handleDeleteProject = async (e, id) => {
    e.stopPropagation()
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

  const formatDate = (dateStr) => {
    return new Date(dateStr).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
  }

  if (loading) return <div className="dash-loading">Loading...</div>

  return (
    <div className="dash-root">
      <nav className="dash-nav">
        <div className="dash-brand">Planify</div>
        <div className="dash-nav-right">
          <span className="dash-user">Hello, <span>{user.firstName}</span></span>
          <button className="btn-logout" onClick={handleLogout}>Sign out</button>
        </div>
      </nav>

      <div className="dash-body">
        <div className="dash-header">
          <h1 className="dash-greeting">Good to see you, {user.firstName}.</h1>
          <p className="dash-tagline">Here's what you're working on.</p>
        </div>

        {error && <div className="dash-error">{error}</div>}

        <div className="dash-section-header">
          <span className="dash-section-title">Projects — {projects.length}</span>
          <button className="btn-new" onClick={() => setShowModal(true)}>+ New Project</button>
        </div>

        <div className="projects-grid">
          {projects.length === 0 ? (
            <div className="empty-state">
              <p>No projects yet — create your first one</p>
            </div>
          ) : (
            projects.map((project) => (
              <div className="project-card" key={project.id} onClick={() => navigate(`/projects/${project.id}`)}>
                <div className="project-card-top">
                  <div className="project-icon">📁</div>
                  <button className="btn-delete" onClick={(e) => handleDeleteProject(e, project.id)}>✕</button>
                </div>
                <div className="project-name">{project.name}</div>
                <div className="project-desc">{project.description || 'No description'}</div>
                <div className="project-card-footer">
                  <span className="project-date">{formatDate(project.createdAt)}</span>
                  <button className="btn-open">Open →</button>
                </div>
              </div>
            ))
          )}
        </div>
      </div>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2 className="modal-title">New Project</h2>
              <button className="btn-close" onClick={() => setShowModal(false)}>✕</button>
            </div>
            <form className="modal-form" onSubmit={handleCreateProject}>
              <div className="field-group">
                <label className="field-label">Project Name</label>
                <input
                  className="field-input"
                  type="text"
                  placeholder="e.g. Q4 Product Launch"
                  value={newProject.name}
                  onChange={(e) => setNewProject({ ...newProject, name: e.target.value })}
                  required
                  autoFocus
                />
              </div>
              <div className="field-group">
                <label className="field-label">Description</label>
                <input
                  className="field-input"
                  type="text"
                  placeholder="What's this project about?"
                  value={newProject.description}
                  onChange={(e) => setNewProject({ ...newProject, description: e.target.value })}
                />
              </div>
              <div className="modal-actions">
                <button className="btn-cancel" type="button" onClick={() => setShowModal(false)}>Cancel</button>
                <button className="btn-create" type="submit" disabled={creating}>
                  {creating ? 'Creating...' : 'Create Project'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}

export default DashboardPage