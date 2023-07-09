import { memo, forwardRef } from 'react';
import { cn } from '../../utils.jsx';
import PropTypes from 'prop-types';

const Input = forwardRef(({ className, type, ...props }, ref) => {
  return (
    <input
      type={type}
      className={cn('w-full px-3 py-2 border rounded', className)}
      ref={ref}
      {...props}
    />
  );
});

Input.displayName = 'Input';

Input.propTypes = {
  children: PropTypes.node,
  type: PropTypes.node,
  className: PropTypes.string,
};

export default memo(Input);
