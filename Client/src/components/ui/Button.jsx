import { memo } from 'react';
import PropTypes from 'prop-types';

const Button = ({ children, onClick, className, ...props }) => (
  <button
    onClick={onClick}
    className={`px-4 py-2 rounded ${className}`}
    {...props}
  >
    {children}
  </button>
);

Button.propTypes = {
  children: PropTypes.node.isRequired,
  onClick: PropTypes.func.isRequired,
  className: PropTypes.string,
};

Button.displayName = 'Button';

export default memo(Button);
