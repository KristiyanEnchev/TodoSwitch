import PropTypes from 'prop-types';
import { memo, useEffect } from 'react';

const Dialog = ({ open, onClose, children }) => {
  useEffect(() => {
    const handleEsc = (event) => {
      if (event.key === 'Escape') {
        onClose();
      }
    };

    window.addEventListener('keydown', handleEsc);

    return () => {
      window.removeEventListener('keydown', handleEsc);
    };
  }, [onClose]);

  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="fixed inset-0 bg-black opacity-50" onClick={onClose} />
      <div className="relative bg-gray-300 dark:bg-gray-800 rounded-lg shadow-lg max-w-lg w-full">
        {children}
      </div>
    </div>
  );
};

Dialog.propTypes = {
  children: PropTypes.node.isRequired,
  open: PropTypes.bool,
  onClose: PropTypes.func.isRequired,
};

Dialog.displayName = 'Dialog';

export default memo(Dialog);
