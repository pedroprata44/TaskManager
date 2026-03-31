import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { useAuthStore } from './store/authStore';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import ProtectedRoute from './components/ProtectedRoute';

function App() {
  const { token, setToken } = useAuthStore((state) => ({
    token: state.token,
    setToken: state.setToken,
    clearToken: state.clearToken,
  }));
  const clearToken = useAuthStore((state) => state.clearToken);
  const [isInitialized, setIsInitialized] = useState(false);

  useEffect(() => {
    // Restaurar token do localStorage ao montar o app
    const storedToken = localStorage.getItem('token');
    if (storedToken) {
      setToken(storedToken);
    } else {
      clearToken();
    }
    setIsInitialized(true);
  }, [setToken, clearToken]);

  if (!isInitialized) {
    return <div>Loading...</div>;
  }

  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          }
        />
        <Route path="/" element={<Navigate to={token ? "/dashboard" : "/login"} replace />} />
        <Route path="*" element={<Navigate to={token ? "/dashboard" : "/login"} replace />} />
      </Routes>
    </Router>
  );
}

export default App;
