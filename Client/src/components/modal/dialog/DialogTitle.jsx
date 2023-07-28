import { memo } from 'react';
import PropTypes from 'prop-types';

const DialogTitle = ({ children }) => (
  <h2 className="text-xl font-semibold text-black dark:text-gray-300">
    {children}
  </h2>
);

DialogTitle.propTypes = {
  children: PropTypes.node.isRequired,
};

DialogTitle.displayName = 'DialogTitle';

export default memo(DialogTitle);
