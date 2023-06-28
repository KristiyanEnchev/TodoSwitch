import { Route, Routes } from 'react-router-dom';
import PublicRoute from './components/auth/PublicRoute.jsx';
import ProtectedRoute from './components/auth/ProtectedRoute.jsx';

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
      <Route path="/" element={<div>Home</div>} />
      <Route path="/home" element={<div>Home</div>} />
      <Route path="*" element={<div>Page Not Found</div>} />
    </Routes>
  );
};

export default AppRouter;
