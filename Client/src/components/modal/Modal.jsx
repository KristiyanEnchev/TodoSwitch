import { memo } from 'react';
import PropTypes from 'prop-types';
import { useSelector, useDispatch } from 'react-redux';
import { closeModal } from '../../features/modal/modalSlice.jsx';
import { motion, AnimatePresence } from 'framer-motion';
import Dialog from './dialog/Dialog.jsx';
import DialogContent from './dialog/DialogContent.jsx';
import DialogHeader from './dialog/DialogHeader.jsx';
import DialogTitle from './dialog/DialogTitle.jsx';

const Modal = ({
  title,
  children,
  onConfirm,
  showConfirmButton,
  modalType,
}) => {
  const dispatch = useDispatch();
  const { isOpen, type } = useSelector((state) => state.modal);
  const { currentColor } = useSelector((state) => state.theme);

  const handleClose = () => dispatch(closeModal());
  const handleConfirm = () => {
    onConfirm();
    handleClose();
  };

  const backdropVariants = {
    visible: { opacity: 1 },
    hidden: { opacity: 0 },
  };

  const modalVariants = {
    hidden: { opacity: 0, y: '-100vh' },
    visible: { opacity: 1, y: '0', transition: { delay: 0.1 } },
    exit: { opacity: 0, y: '100vh' },
  };

  return (
    <AnimatePresence>
      {isOpen && modalType === type && (
        <Dialog open={isOpen} onClose={handleClose}>
          <motion.div
            className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center"
            initial="hidden"
            animate="visible"
            exit="hidden"
            variants={backdropVariants}
            onClick={handleClose}
          >
            <motion.div
              className="bg-gray-300 dark:bg-gray-800 rounded-lg w-full max-w-md mx-4 max-h-[80vh] overflow-y-auto"
              variants={modalVariants}
              onClick={(e) => e.stopPropagation()}
            >
              <DialogContent>
                <DialogHeader onClose={handleClose}>
                  <DialogTitle className="flex justify-between items-center">
                    {title}
                  </DialogTitle>
                </DialogHeader>
                {children}
                <div className="mt-4 flex justify-end space-x-2">
                  {showConfirmButton && (
                    <>
                      <button
                        className="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-700"
                        onClick={handleClose}
                      >
                        Cancel
                      </button>
                      <button
                        style={{ backgroundColor: currentColor }}
                        className="text-white px-4 py-2 rounded hover:opacity-80"
                        onClick={handleConfirm}
                      >
                        Confirm
                      </button>
                    </>
                  )}
                </div>
              </DialogContent>
            </motion.div>
          </motion.div>
        </Dialog>
      )}
    </AnimatePresence>
  );
};

Modal.propTypes = {
  title: PropTypes.string.isRequired,
  children: PropTypes.node.isRequired,
  onConfirm: PropTypes.func,
  showConfirmButton: PropTypes.bool,
  modalType: PropTypes.string.isRequired,
};

export default memo(Modal);
