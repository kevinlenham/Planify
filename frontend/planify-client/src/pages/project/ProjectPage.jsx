import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { DragDropContext, Droppable, Draggable } from '@hello-pangea/dnd'
import { getProject } from '../../api/projects'
import { getTasksByProject, createTask, updateTask, deleteTask } from '../../api/projectTasks'
import './ProjectPage.css'

const STATUS_OPTIONS = ['ToDo', 'InProgress', 'Done']
const STATUS_MAP = { 'ToDo': 0, 'InProgress': 1, 'Done': 2 }
const STATUS_REVERSE = { 0: 'ToDo', 1: 'InProgress', 2: 'Done' }
const STATUS_LABELS = { ToDo: 'To Do', InProgress: 'In Progress', Done: 'Done' }

const ProjectPage = () => {
  const { id } = useParams()
  const navigate = useNavigate()
  const [project, setProject] = useState(null)
  const [tasks, setTasks] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [showModal, setShowModal] = useState(false)
  const [creating, setCreating] = useState(false)
  const [newTask, setNewTask] = useState({ name: '', description: '', dueDate: '', status: 'ToDo' })

  // eslint-disable-next-line
  useEffect(() => { fetchData() }, [id])

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
    setCreating(true)
    try {
      await createTask({
        name: newTask.name,
        description: newTask.description,
        dueDate: newTask.dueDate || new Date().toISOString(),
        status: STATUS_MAP[newTask.status],
        projectId: parseInt(id)
      })
      setNewTask({ name: '', description: '', dueDate: '', status: 'ToDo' })
      setShowModal(false)
      fetchData()
    } catch (err) {
      setError('Failed to create task')
    } finally {
      setCreating(false)
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

  const handleDragEnd = async (result) => {
    const { destination, source, draggableId } = result
    if (!destination) return
    if (destination.droppableId === source.droppableId && destination.index === source.index) return

    const taskId = parseInt(draggableId)
    const newStatus = STATUS_MAP[destination.droppableId]
    const task = tasks.find(t => t.id === taskId)

    setTasks(prev => prev.map(t => t.id === taskId ? { ...t, status: newStatus } : t))

    try {
      await updateTask(taskId, { ...task, status: newStatus })
    } catch (err) {
      setError('Failed to update task')
      fetchData()
    }
  }

  const getTasksByStatus = (status) => tasks.filter(t => STATUS_REVERSE[t.status] === status)

  const formatDueDate = (dateStr) => {
    if (!dateStr) return null
    const date = new Date(dateStr)
    const isOverdue = date < new Date()
    return { text: date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }), overdue: isOverdue }
  }

  if (loading) return <div className="proj-loading">Loading...</div>
  if (!project) return <div className="proj-loading">Project not found</div>

  return (
    <div className="proj-root">
      <nav className="proj-nav">
        <div className="proj-nav-left">
          <button className="btn-back" onClick={() => navigate('/dashboard')}>← Dashboard</button>
          <h1 className="proj-title">{project.name}</h1>
        </div>
        <button className="btn-add-task" onClick={() => setShowModal(true)}>+ Add Task</button>
      </nav>

      <div className="proj-body">
        {project.description && <p className="proj-desc">{project.description}</p>}
        {error && <div className="proj-error">{error}</div>}

        <DragDropContext onDragEnd={handleDragEnd}>
          <div className="kanban-board">
            {STATUS_OPTIONS.map((status) => {
              const columnTasks = getTasksByStatus(status)
              return (
                <div className={`kanban-col kanban-col-${status.toLowerCase()}`} key={status}>
                  <div className="kanban-col-header">
                    <div className="kanban-col-title">
                      <span className="col-dot" />
                      {STATUS_LABELS[status]}
                    </div>
                    <span className="col-count">{columnTasks.length}</span>
                  </div>
                  <Droppable droppableId={status}>
                    {(provided, snapshot) => (
                      <div
                        className={`kanban-drop-zone ${snapshot.isDraggingOver ? 'dragging-over' : ''}`}
                        ref={provided.innerRef}
                        {...provided.droppableProps}
                      >
                        {columnTasks.length === 0 && !snapshot.isDraggingOver && (
                          <div className="drop-placeholder">Drop tasks here</div>
                        )}
                        {columnTasks.map((task, index) => {
                          const due = formatDueDate(task.dueDate)
                          return (
                            <Draggable key={task.id} draggableId={String(task.id)} index={index}>
                              {(provided, snapshot) => (
                                <div
                                  className={`task-card ${snapshot.isDragging ? 'dragging' : ''}`}
                                  ref={provided.innerRef}
                                  {...provided.draggableProps}
                                >
                                  <div className="task-card-header">
                                    <div {...provided.dragHandleProps} className="task-grip">⠿</div>
                                    <span className="task-name">{task.name}</span>
                                    <button className="btn-task-delete" onClick={() => handleDeleteTask(task.id)}>✕</button>
                                  </div>
                                  {task.description && <p className="task-desc">{task.description}</p>}
                                  {due && (
                                    <span className={`task-due ${due.overdue ? 'overdue' : ''}`}>
                                      {due.overdue ? '⚠ ' : ''}Due {due.text}
                                    </span>
                                  )}
                                </div>
                              )}
                            </Draggable>
                          )
                        })}
                        {provided.placeholder}
                      </div>
                    )}
                  </Droppable>
                </div>
              )
            })}
          </div>
        </DragDropContext>
      </div>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2 className="modal-title">New Task</h2>
              <button className="btn-close" onClick={() => setShowModal(false)}>✕</button>
            </div>
            <form className="modal-form" onSubmit={handleCreateTask}>
              <div className="field-group">
                <label className="field-label">Task Name</label>
                <input className="field-input" type="text" placeholder="e.g. Design the homepage" value={newTask.name} onChange={(e) => setNewTask({ ...newTask, name: e.target.value })} required autoFocus />
              </div>
              <div className="field-group">
                <label className="field-label">Description</label>
                <input className="field-input" type="text" placeholder="Optional details..." value={newTask.description} onChange={(e) => setNewTask({ ...newTask, description: e.target.value })} />
              </div>
              <div className="field-group">
                <label className="field-label">Due Date</label>
                <input className="field-input" type="date" value={newTask.dueDate} onChange={(e) => setNewTask({ ...newTask, dueDate: e.target.value })} />
              </div>
              <div className="field-group">
                <label className="field-label">Status</label>
                <select className="field-select" value={newTask.status} onChange={(e) => setNewTask({ ...newTask, status: e.target.value })}>
                  <option value="ToDo">To Do</option>
                  <option value="InProgress">In Progress</option>
                  <option value="Done">Done</option>
                </select>
              </div>
              <div className="modal-actions">
                <button className="btn-cancel" type="button" onClick={() => setShowModal(false)}>Cancel</button>
                <button className="btn-create" type="submit" disabled={creating}>{creating ? 'Adding...' : 'Add Task'}</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}

export default ProjectPage