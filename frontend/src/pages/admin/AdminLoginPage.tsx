import { useState, type FormEvent } from 'react'
import { Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from '../../auth/AuthContext'
import { Navbar } from '../../components/layout/Navbar'

export function AdminLoginPage() {
  const { isAuthenticated, login } = useAuth()
  const navigate = useNavigate()
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  if (isAuthenticated) {
    return <Navigate to="/admin" replace />
  }

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setIsSubmitting(true)
    setErrorMessage(null)

    try {
      await login({ username, password })
      navigate('/admin', { replace: true })
    } catch (error) {
      const message = error instanceof Error ? error.message : 'No se pudo iniciar sesión. Intenta de nuevo.'
      setErrorMessage(message)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="page-shell">
      <Navbar />
      <main className="container admin-panel">
        <p className="eyebrow">Área privada</p>
        <h1>Admin Login</h1>
        <p>Ingresa con usuario y clave para administrar vinos, reseñas y contenido institucional.</p>
        <form className="admin-auth-form" onSubmit={handleSubmit}>
          <label className="field-group" htmlFor="username">
            <span>Username</span>
            <input
              id="username"
              name="username"
              type="text"
              autoComplete="username"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
              required
            />
          </label>
          <label className="field-group" htmlFor="password">
            <span>Password</span>
            <input
              id="password"
              name="password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
            />
          </label>
          {errorMessage ? <p className="state state--error">{errorMessage}</p> : null}
          <button className="button button--full" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Ingresando...' : 'Iniciar sesión'}
          </button>
        </form>
      </main>
    </div>
  )
}
