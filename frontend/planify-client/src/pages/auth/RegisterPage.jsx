import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import './RegisterPage.css'

const RegisterPage = () => {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: ''
  })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const { register } = useAuth()
  const navigate = useNavigate()

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value })
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setError('')
    try {
      await register(formData)
      navigate('/dashboard')
    } catch (err) {
      setError(err.response?.data || 'Registration failed')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="register-root">
      <div className="register-left">
        <div className="register-brand">Planify</div>
        <h1 className="register-heading">Start building.</h1>
        <p className="register-subtext">Create your account and start managing projects.</p>
        {error && <div className="register-error">{error}</div>}
        <form className="register-form" onSubmit={handleSubmit}>
          <div className="field-row">
            <div className="field-group">
              <label className="field-label">First Name</label>
              <input className="field-input" type="text" name="firstName" placeholder="John" value={formData.firstName} onChange={handleChange} required />
            </div>
            <div className="field-group">
              <label className="field-label">Last Name</label>
              <input className="field-input" type="text" name="lastName" placeholder="Doe" value={formData.lastName} onChange={handleChange} required />
            </div>
          </div>
          <div className="field-group">
            <label className="field-label">Email</label>
            <input className="field-input" type="email" name="email" placeholder="you@example.com" value={formData.email} onChange={handleChange} required />
          </div>
          <div className="field-group">
            <label className="field-label">Password</label>
            <input className="field-input" type="password" name="password" placeholder="••••••••" value={formData.password} onChange={handleChange} required />
          </div>
          <button className="register-btn" type="submit" disabled={loading}>
            {loading ? 'Creating account...' : 'Create account'}
          </button>
        </form>
        <p className="register-footer">
          Already have an account? <Link to="/login">Sign in</Link>
        </p>
      </div>
      <div className="register-right">
        {[
          { icon: '📋', title: 'Project Management', desc: 'Organise your work into projects with tasks and deadlines.' },
          { icon: '🎯', title: 'Kanban Boards', desc: 'Drag and drop tasks across To Do, In Progress, and Done.' },
          { icon: '⚡', title: 'Built on .NET + React', desc: 'Fast, reliable, modern full-stack architecture.' },
        ].map((f, i) => (
          <div className="feature-item" key={i}>
            <div className="feature-icon">{f.icon}</div>
            <div>
              <div className="feature-title">{f.title}</div>
              <div className="feature-desc">{f.desc}</div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default RegisterPage