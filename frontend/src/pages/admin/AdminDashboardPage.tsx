import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../auth/AuthContext'
import { Navbar } from '../../components/layout/Navbar'
import { AdminReviewsManager } from '../../features/admin/components/AdminReviewsManager'
import { AdminSiteContentManager } from '../../features/admin/components/AdminSiteContentManager'
import { AdminWinesManager } from '../../features/admin/components/AdminWinesManager'

export function AdminDashboardPage() {
  const { logout, session } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/admin/login', { replace: true })
  }

  return (
    <div className="page-shell">
      <Navbar />
      <main className="container admin-panel">
        <p className="eyebrow">Dashboard admin</p>
        <h1>Panel operativo autenticado</h1>
        <p>
          Sesión activa con token JWT. Esta vista consume endpoints admin protegidos para vinos, reseñas y contenido del
          sitio.
        </p>
        <div className="admin-status">
          <span className="state state--success">Autenticado</span>
          <p>
            Expira: <strong>{session ? new Date(session.expiresAtUtc).toLocaleString() : 'Sin dato'}</strong>
          </p>
        </div>
        <button className="button" type="button" onClick={handleLogout}>
          Cerrar sesión
        </button>

        <AdminWinesManager />
        <AdminReviewsManager />
        <AdminSiteContentManager />
      </main>
    </div>
  )
}
