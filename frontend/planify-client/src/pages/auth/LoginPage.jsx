import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import './LoginPage.css'

const LoginPage = () => {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const { login } = useAuth()
  const navigate = useNavigate()

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setError('')
    try {
      await login({ email, password })
      navigate('/dashboard')
    } catch (err) {
      setError(err.response?.data || 'Invalid email or password')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="login-root">
      <div className="login-left">
        <div className="login-brand">Planify</div>
        <h1 className="login-heading">Welcome back.</h1>
        <p className="login-subtext">Sign in to manage your projects.</p>
        {error && <div className="login-error">{error}</div>}
        <form className="login-form" onSubmit={handleSubmit}>
          <div className="field-group">
            <label className="field-label">Email</label>
            <input
              className="field-input"
              type="email"
              placeholder="you@example.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>
          <div className="field-group">
            <label className="field-label">Password</label>
            <input
              className="field-input"
              type="password"
              placeholder="••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>
          <button className="login-btn" type="submit" disabled={loading}>
            {loading ? 'Signing in...' : 'Sign in'}
          </button>
        </form>
        <p className="login-footer">
          No account? <Link to="/register">Create one</Link>
        </p>
      </div>
      <div className="login-right">
        <div className="visual-card">
          <div className="visual-card-header">
            <div className="visual-avatar" />
            <div>
              <div className="visual-card-title">Q4 Product Launch</div>
              <div className="visual-card-sub">4 tasks remaining</div>
            </div>
          </div>
          {[
            { text: 'Design system update', badge: 'Done', badgeClass: 'badge-done' },
            { text: 'API integration', badge: 'In Progress', badgeClass: 'badge-progress' },
            { text: 'User testing', badge: 'To Do', badgeClass: 'badge-todo' },
            { text: 'Launch checklist', badge: 'To Do', badgeClass: 'badge-todo' },
          ].map((task, i) => (
            <div className="visual-task" key={i}>
              <span className="visual-task-text">{task.text}</span>
              <span className={`visual-badge ${task.badgeClass}`}>{task.badge}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

export default LoginPage