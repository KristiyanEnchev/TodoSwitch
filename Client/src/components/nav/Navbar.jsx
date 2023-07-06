import { useState, useEffect, memo } from 'react';
import { Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import { useSelector } from 'react-redux';
import { useLogoutMutation } from 'features';
import { Menu, X } from 'lucide-react';

const Navbar = () => {
  const { isAuthenticated, user } = useSelector((state) => state.auth);
  const [logout] = useLogoutMutation();
  const { currentColor } = useSelector((state) => state.theme);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const handleLogOut = async (e) => {
    e.preventDefault();
    try {
      const response = await logout({ email: user.Email }).unwrap();
      toast.success(response.message);
    } catch (error) {
      toast.error(`An error occurred: ${error}`);
    }
  };

  const toggleMobileMenu = () => {
    setIsMobileMenuOpen((prev) => !prev);
  };

  const closeMobileMenu = () => {
    setIsMobileMenuOpen(false);
  };

  useEffect(() => {
    const handleEsc = (event) => {
      if (event.key === 'Escape') {
        closeMobileMenu();
      }
    };
    window.addEventListener('keydown', handleEsc);
    return () => window.removeEventListener('keydown', handleEsc);
  }, []);

  return (
    <nav className="p-4 bg-gray-900 relative z-50">
      <div className="container mx-auto flex justify-between items-center">
        {/* Logo */}
        <Link
          to="/"
          className="text-2xl font-bold"
          style={{ color: currentColor }}
        >
          TodoSwitch
        </Link>

        {/* Mobile Menu Button */}
        <button
          onClick={toggleMobileMenu}
          className="md:hidden text-xl text-white"
        >
          {isMobileMenuOpen ? <X /> : <Menu />}
        </button>

        {/* Navigation Links */}
        <ul
          className={`${
            isMobileMenuOpen ? 'flex' : 'hidden'
          } flex-col md:flex-row md:flex space-y-4 md:space-y-0 md:space-x-4 mt-4 md:mt-0 items-center w-full md:w-auto absolute top-0 left-0 md:static bg-gray-900 p-4 md:p-0`}
        >
          {/* Close Button on Mobile */}
          {isMobileMenuOpen && (
            <div className="flex justify-between w-full md:hidden">
              <Link
                to="/"
                className="text-2xl font-bold"
                style={{ color: currentColor }}
              >
                TodoSwitch
              </Link>
              <button onClick={closeMobileMenu} className="text-xl text-white">
                <X />
              </button>
            </div>
          )}
          {isAuthenticated ? (
            <>
              <li>
                <Link
                  to="/dashboard"
                  className="font-bold text-lg text-white dark:text-white hover:text-gray-400"
                  onClick={closeMobileMenu}
                >
                  Dashboard
                </Link>
              </li>
              <li>
                <button
                  onClick={(e) => {
                    handleLogOut(e);
                    closeMobileMenu();
                  }}
                  style={{ color: currentColor }}
                  className="font-bold text-lg hover:text-white"
                >
                  Logout
                </button>
              </li>
            </>
          ) : (
            <>
              <li>
                <Link
                  to="/login"
                  className="font-bold text-lg hover:text-white"
                  style={{ color: currentColor }}
                  onClick={closeMobileMenu}
                >
                  Login
                </Link>
              </li>
              <li>
                <Link
                  to="/register"
                  style={{ backgroundColor: currentColor }}
                  className="rounded-md font-bold text-lg text-white px-4 py-1"
                  onClick={closeMobileMenu}
                >
                  Register
                </Link>
              </li>
            </>
          )}
        </ul>
      </div>
    </nav>
  );
};

export default memo(Navbar);
