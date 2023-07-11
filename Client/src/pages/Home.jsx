import {
  MdOutlineCode,
  MdSecurity,
  MdMobileFriendly,
  MdDataUsage,
} from 'react-icons/md';
import { FaReact, FaDatabase } from 'react-icons/fa';
import { Helmet } from 'react-helmet-async';
import { useSelector } from 'react-redux';
import { motion } from 'framer-motion';

const features = [
  {
    name: 'Modern Frontend with React',
    description:
      'Building an interactive and dynamic user interface using React, Redux, Redux Toolkit, and Framer Motion for smooth animations.',
    icon: FaReact,
  },
  {
    name: 'Robust Backend with .NET',
    description:
      'Utilizing .NET 7 for a solid and scalable backend, following Domain-Driven Design (DDD) patterns.',
    icon: MdOutlineCode,
  },
  {
    name: 'Secure Authentication',
    description:
      'Implementing secure user authentication and authorization with .NET Identity.',
    icon: MdSecurity,
  },
  {
    name: 'Flexible Data Storage',
    description:
      'Leveraging MongoDB for flexible and scalable NoSQL data storage.',
    icon: FaDatabase,
  },
  {
    name: 'Seamless Data Fetching',
    description:
      'Efficiently fetching and managing data with Redux Toolkit Query.',
    icon: MdDataUsage,
  },
  {
    name: 'Responsive Mobile Design',
    description:
      'Ensuring a mobile-friendly experience with responsive design principles.',
    icon: MdMobileFriendly,
  },
];

const Home = () => {
  const { currentColor } = useSelector((state) => state.theme);
  return (
    <>
      <Helmet>
        <title>Home</title>
      </Helmet>
      <div className="py-20 sm:py-16 lg:py-20 bg-gray-300 dark:bg-black">
        <motion.div
          className="relative"
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
        >
          {' '}
          <div className="mx-auto max-w-7xl px-6 lg:px-8">
            <div className="sm:text-center">
              <h2
                style={{ color: currentColor }}
                className={`text-lg font-semibold leading-8`}
              >
                Todo App Technology Stack
              </h2>
              <p className="mt-2 text-3xl font-bold tracking-tight sm:text-4xl dark:text-white text-gray-700">
                Empowering Your Productivity
              </p>
              <p className="mx-auto mt-6 max-w-2xl text-lg leading-8 text-gray-600 dark:text-gray-400">
                Discover the modern technologies and best practices used in
                building this efficient and powerful todo application.
              </p>
            </div>

            <div className="mt-20 max-w-lg sm:mx-auto md:max-w-none">
              <div className="grid grid-cols-1 gap-y-16 md:grid-cols-2 md:gap-x-12 md:gap-y-16">
                {features.map((feature) => (
                  <div
                    key={feature.name}
                    className="bg-gray-800 rounded p-4 relative flex flex-col gap-6 sm:flex-row md:flex-col lg:flex-row"
                  >
                    <div
                      style={{ color: currentColor }}
                      className="flex justify-center  sm:shrink-0"
                    >
                      <feature.icon className="h-8 w-8" aria-hidden="true" />
                    </div>
                    <div className="sm:min-w-0 sm:flex-1">
                      <p className="text-lg font-semibold leading-8 text-white">
                        {feature.name}
                      </p>
                      <p className="mt-2 text-base leading-7 text-gray-400">
                        {feature.description}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </motion.div>
      </div>
    </>
  );
};

export default Home;
