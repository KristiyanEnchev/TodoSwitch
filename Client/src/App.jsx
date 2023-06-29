import { Helmet } from 'react-helmet-async';
import { ToastContainer } from 'react-toastify';
import MainLayout from 'layouts/MainLayout';
import AppRouter from './Router';
import ErrorBoundary from 'components/error/ErrorBoundary';

function App() {
  return (
    <ErrorBoundary>
      <Helmet>
        <title>TodoSwitch</title>
      </Helmet>
      <ToastContainer position="top-center" />
      <MainLayout>
        <AppRouter />
      </MainLayout>
    </ErrorBoundary>
  );
}

export default App;
