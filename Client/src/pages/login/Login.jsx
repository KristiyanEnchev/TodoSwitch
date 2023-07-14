import { memo, lazy, Suspense } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { useSelector } from 'react-redux';
import { loginSchema } from './schema';
import { useLoginMutation } from '../../features/auth/authApiSlice.jsx';
import { cn } from '../../utils.jsx';
import Form from '../../components/form/Form.jsx';
import { motion } from 'framer-motion';

const Spinner = lazy(() => import('components/spinner/Spinner.jsx'));

const Login = memo(() => {
  const navigate = useNavigate();
  const [login, { isLoading }] = useLoginMutation();
  const { currentColor } = useSelector((state) => state.theme);

  const handleSubmit = async (fields) => {
    try {
      await login(fields).unwrap();
      toast.success('Welcome back!');
      navigate('/');
    } catch (err) {
      toast.error(
        err.data?.errors?.[0] || 'Login failed. Please try again later.'
      );
    }
  };

  const fields = [
    { name: 'email', type: 'email' },
    { name: 'password', type: 'password' },
  ];

  const titleSection = (
    <div className="text-center mb-3">
      <h2
        style={{ color: currentColor }}
        className="text-4xl font-bold text-center"
      >
        SIGN IN
      </h2>
    </div>
  );

  const extraContent = (
    <div className="mb-3" style={{ color: currentColor }}>
      Don`t have an account?{' '}
      <Link
        to="/register"
        className="text-white dark:text-gray-600 dark:hover:text-black"
      >
        Register
      </Link>
    </div>
  );

  return (
    <div
      className={cn(
        'grid grid-cols-1 dashboard w-full',
        'bg-gray-300 dark:bg-black'
      )}
    >
      <Suspense fallback={<Spinner />}>
        {isLoading && <Spinner />}
        <div className="flex flex-col justify-center">
          <motion.div
            className="relative"
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
          >
            {' '}
            <Form
              initialValues={{ email: '', password: '' }}
              validationSchema={loginSchema}
              onSubmit={handleSubmit}
              submitButtonText="SIGN IN"
              fields={fields}
              extraContent={extraContent}
              titleSection={titleSection}
              color={currentColor}
            />
          </motion.div>
        </div>
      </Suspense>
    </div>
  );
});

Login.displayName = 'Login';

export default Login;
