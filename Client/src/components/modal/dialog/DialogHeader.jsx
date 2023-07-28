import { memo } from 'react';
import PropTypes from 'prop-types';
import { X } from 'lucide-react';

const DialogHeader = ({ children, onClose }) => (
  <div className="border-b px-6 py-4 flex justify-between items-center">
    {children}
    <button onClick={onClose} className="text-gray-600 hover:text-gray-800">
      <X className="h-5 w-5" />
    </button>
  </div>
);

DialogHeader.propTypes = {
  children: PropTypes.node.isRequired,
  onClose: PropTypes.func.isRequired,
};

DialogHeader.displayName = 'DialogHeader';

export default memo(DialogHeader);
