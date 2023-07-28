import { memo } from 'react';
import PropTypes from 'prop-types';
import Button from '../ui/Button.jsx';
import { useSelector } from 'react-redux';

const DeleteConfirmation = ({
  message,
  onConfirm,
  onCancel,
  showButtons = true,
}) => {
  const { currentColor } = useSelector((state) => state.theme);

  return (
    <div className="py-4">
      <p className="mb-4">{message}</p>
      <div className="flex justify-end space-x-2">
        {showButtons && (
          <>
            <Button
              className="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-700"
              onClick={onCancel}
            >
              Cancel
            </Button>
            <Button
              style={{ backgroundColor: currentColor }}
              className="text-white px-4 py-2 rounded hover:opacity-80"
              onClick={onConfirm}
            >
              Confirm
            </Button>{' '}
          </>
        )}
      </div>
    </div>
  );
};

DeleteConfirmation.displayName = 'DeleteConfirmation';

DeleteConfirmation.propTypes = {
  message: PropTypes.string.isRequired,
  onConfirm: PropTypes.func,
  onCancel: PropTypes.func,
  showButtons: PropTypes.bool,
};

export default memo(DeleteConfirmation);
