import { memo } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Spinner } from 'components';
import { useRegisterMutation } from 'features';
import Form from 'components/form/Form';
import registrationSchema from './schema.jsx';
import { useSelector } from 'react-redux';
import { motion } from 'framer-motion';

const Register = memo(() => {
  const navigate = useNavigate();
  const [register, { isLoading }] = useRegisterMutation();
  const { currentColor } = useSelector((state) => state.theme);

  const handleSubmit = async (fields) => {
    try {
      await register(fields).unwrap();
      toast.success('Registration successful!');
      navigate('/');
    } catch (err) {
      toast.error(
        err.data?.errors?.[0] || 'Registration failed. Please try again later.'
      );
    }
  };

  const fields = [
    { name: 'firstName' },
    { name: 'lastName' },
    { name: 'email', type: 'email' },
    { name: 'password', type: 'password' },
    { name: 'confirmPassword', type: 'password' },
  ];

  const titleSection = (
    <div className="text-center mb-3">
      <h2
        style={{ color: currentColor }}
        className="text-4xl font-bold text-center"
      >
        SIGN UP
      </h2>
    </div>
  );

  const extraContent = (
    <div
      style={{ color: currentColor }}
      className="text-center text-emerald-300"
    >
      Already have an account?{' '}
      <Link
        to="/login"
        className="text-white dark:text-gray-600 dark:hover:text-black"
      >
        Login
      </Link>
    </div>
  );

  return (
    <>
      {isLoading && <Spinner />}
      <div className="grid grid-cols-1 w-full">
        <div className="flex flex-col justify-center py-5 bg-gray-300 dark:bg-black">
          <motion.div
            className="relative"
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
          >
            {' '}
            <Form
              initialValues={{
                firstName: '',
                lastName: '',
                email: '',
                password: '',
                confirmPassword: '',
              }}
              validationSchema={registrationSchema}
              onSubmit={handleSubmit}
              submitButtonText="SIGN UP"
              fields={fields}
              extraContent={extraContent}
              titleSection={titleSection}
              color={currentColor}
            />
          </motion.div>
        </div>
      </div>
    </>
  );
});

Register.displayName = 'Register';

export default Register;
