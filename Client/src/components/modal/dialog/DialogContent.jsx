import PropTypes from 'prop-types';
import { memo } from 'react';

const DialogContent = ({ children }) => {
  return <div className="p-6 text-black dark:text-gray-400">{children}</div>;
};

DialogContent.displayName = 'DialogContent';

DialogContent.propTypes = {
  children: PropTypes.node.isRequired,
};

export default memo(DialogContent);
