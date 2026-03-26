import { Navigate, Route, Routes } from 'react-router-dom'
import { AdminDashboardPage } from '../pages/admin/AdminDashboardPage'
import { AdminLoginPage } from '../pages/admin/AdminLoginPage'
import { HomePage } from '../pages/home/HomePage'
import { ProtectedRoute } from './ProtectedRoute'

export function AppRouter() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/admin/login" element={<AdminLoginPage />} />
      <Route element={<ProtectedRoute />}>
        <Route path="/admin" element={<AdminDashboardPage />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
