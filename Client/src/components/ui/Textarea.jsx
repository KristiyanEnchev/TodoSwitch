import { memo, forwardRef } from 'react';
import { cn } from '../../utils.jsx';
import PropTypes from 'prop-types';

const Textarea = forwardRef(({ className, ...props }, ref) => {
  return (
    <textarea
      className={cn('w-full px-3 py-2 border rounded', className)}
      ref={ref}
      {...props}
    />
  );
});

Textarea.displayName = 'Textarea';

Textarea.propTypes = {
  className: PropTypes.string,
};

export default memo(Textarea);
