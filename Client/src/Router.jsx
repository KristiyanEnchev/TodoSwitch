import { Route, Routes } from 'react-router-dom';
import PublicRoute from './components/auth/PublicRoute.jsx';
import ProtectedRoute from './components/auth/ProtectedRoute.jsx';
import Home from './pages/Home.jsx';
import Login from './pages/login/Login.jsx';
import PageNotFound from './pages/PageNotFound.jsx';
import Register from './pages/register/Register.jsx';
import Dashboard from './pages/Dashboard.jsx';

const AppRouter = () => {
  return (
    <Routes>
      <Route element={<PublicRoute />}>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
      </Route>
      <Route element={<ProtectedRoute />}>
        <Route path="/dashboard" element={<Dashboard />} />
      </Route>
      <Route path="/" element={<Home />} />
      <Route path="/home" element={<Home />} />
      <Route path="*" element={<PageNotFound />} />
    </Routes>
  );
};

export default AppRouter;
