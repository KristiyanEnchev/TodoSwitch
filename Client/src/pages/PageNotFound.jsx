import { Helmet } from 'react-helmet-async';
import { Link } from 'react-router-dom';
import { useSelector } from 'react-redux';
import Pick404 from '../assets/404_38.jpg';

const PageNotFound = () => {
  const { currentColor } = useSelector((state) => state.theme);

  return (
    <>
      <Helmet>
        <title>404 - Page Not Found</title>
      </Helmet>
      <div className="flex items-center justify-center min-h-screen bg-gray-100 dark:bg-gray-900">
        <div className="max-w-md w-full px-6 py-8 bg-white dark:bg-gray-800 shadow-md rounded-lg">
          <img src={Pick404} alt="404 Error" className="w-full h-auto mb-6" />
          <h1 className="text-3xl font-bold text-center mb-4 dark:text-white">
            Ooooops.....
          </h1>
          <p className="text-center text-gray-600 dark:text-gray-300 mb-6">
            The resource that you are looking for is missing or has been
            relocated!
          </p>
          <Link
            to="/"
            style={{ backgroundColor: currentColor }}
            className="block w-full text-center py-2 px-4 rounded text-white hover:opacity-90 transition-opacity"
          >
            Home Page
          </Link>
        </div>
      </div>
    </>
  );
};

export default PageNotFound;
