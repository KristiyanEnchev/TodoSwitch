import { Route, Routes } from 'react-router-dom';
import PublicRoute from './components/auth/PublicRoute.jsx';
import ProtectedRoute from './components/auth/ProtectedRoute.jsx';
import Home from './pages/Home.jsx';

const AppRouter = () => {
  return (
    <Routes>
      <Route element={<PublicRoute />}>
        <Route path="/login" element={<div>Login</div>} />
        <Route path="/register" element={<div>Register</div>} />
      </Route>
      <Route element={<ProtectedRoute />}>
        <Route path="/ToDo" element={<div>ToDo</div>} />
      </Route>
      <Route path="/" element={<Home />} />
      <Route path="/home" element={<Home />} />
      <Route path="*" element={<div>Page Not Found</div>} />
    </Routes>
  );
};

export default AppRouter;
