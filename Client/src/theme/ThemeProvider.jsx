import { useEffect, useMemo, Suspense, useCallback } from 'react';
import PropTypes from 'prop-types';
import { useDispatch, useSelector } from 'react-redux';
import { motion } from 'framer-motion';
import { MdOutlineCancel } from 'react-icons/md';
import { FiSettings } from 'react-icons/fi';
import { BsCheck } from 'react-icons/bs';
import {
  setThemeMode,
  setCurrentColor,
  toggleThemeSettings,
  closeThemeSettings,
  selectThemeMode,
  selectCurrentColor,
  selectThemeSettings,
} from './themeSlice.jsx';
import { themeColors } from './themeColors.jsx';

const ThemingProvider = ({ children }) => {
  const dispatch = useDispatch();
  const themeMode = useSelector(selectThemeMode);
  const currentColor = useSelector(selectCurrentColor);
  const themeSettings = useSelector(selectThemeSettings);

  useEffect(() => {
    document.documentElement.style.setProperty('--color-current', currentColor);
    document.documentElement.classList.toggle('dark', themeMode === 'dark');
  }, [currentColor, themeMode]);

  const handleModeChange = useCallback(
    (e) => {
      dispatch(setThemeMode(e.target.value));
    },
    [dispatch]
  );

  const handleColorChange = useCallback(
    (color) => {
      dispatch(setCurrentColor(color));
    },
    [dispatch]
  );

  const settingsModal = useMemo(
    () => (
      <motion.div
        initial={{ opacity: 0, x: 200 }}
        animate={{ opacity: 1, x: 0 }}
        exit={{ opacity: 0, x: 200 }}
        className="bg-half-transparent w-screen fixed top-0 right-0 z-50"
      >
        <div
          className="backdrop"
          onClick={() => dispatch(closeThemeSettings())}
        />
        <motion.div
          initial={{ x: 200 }}
          animate={{ x: 0 }}
          exit={{ x: 200 }}
          className="absolute right-0 top-0 h-screen dark:text-gray-200 bg-gray-200 dark:bg-gray-800 w-400 rounded-tl-3xl overflow-y-auto"
        >
          <div className="flex justify-between items-center p-4 ml-4">
            <p className="font-semibold text-lg">Settings</p>
            <button
              type="button"
              onClick={() => dispatch(toggleThemeSettings())}
              style={{ color: currentColor }}
              className="text-2xl p-3"
            >
              <MdOutlineCancel />
            </button>
          </div>
          <div className="p-4 border-t border-gray-200">
            <p className="font-semibold text-xl">Theme Option</p>
            <div className="mt-4">
              <input
                type="radio"
                id="light"
                name="theme"
                value="light"
                className="cursor-pointer h-4 w-4 border-gray-300 focus:ring-teal-500"
                onChange={handleModeChange}
                checked={themeMode === 'light'}
              />
              <label htmlFor="light" className="ml-2 text-md cursor-pointer">
                Light
              </label>
            </div>
            <div className="mt-2">
              <input
                type="radio"
                id="dark"
                name="theme"
                value="dark"
                className="cursor-pointer h-4 w-4 border-gray-300 focus:ring-teal-500"
                onChange={handleModeChange}
                checked={themeMode === 'dark'}
              />
              <label htmlFor="dark" className="ml-2 text-md cursor-pointer">
                Dark
              </label>
            </div>
          </div>
          <div className="p-4 border-t border-gray-200">
            <p className="font-semibold text-xl">Theme Colors</p>
            <div className="flex gap-3">
              {themeColors.map((item) => (
                <div
                  className="relative mt-2 cursor-pointer flex gap-5 items-center"
                  key={item.name}
                >
                  <button
                    type="button"
                    className="h-8 w-8 rounded-full cursor-pointer flex items-center justify-center transition-all transform hover:scale-110"
                    style={{ backgroundColor: item.color }}
                    onClick={() => handleColorChange(item.color)}
                  >
                    <BsCheck
                      className={`text-xl text-white ${
                        item.color === currentColor ? 'block' : 'hidden'
                      }`}
                    />
                  </button>
                </div>
              ))}
            </div>
          </div>
        </motion.div>
      </motion.div>
    ),
    [currentColor, themeMode, handleModeChange, handleColorChange, dispatch]
  );

  return (
    <div className={`min-h-screen bg-primary text-text`}>
      {children}
      {themeSettings && (
        <Suspense fallback={<div>Loading...</div>}>{settingsModal}</Suspense>
      )}
      <button
        className="fixed bottom-4 right-4 p-2 bg-gray-800 dark:bg-gray-600 text-white rounded-full text-3xl z-50"
        onClick={() => dispatch(toggleThemeSettings())}
      >
        <FiSettings />
      </button>
    </div>
  );
};

ThemingProvider.propTypes = {
  children: PropTypes.node.isRequired,
};

export default ThemingProvider;
