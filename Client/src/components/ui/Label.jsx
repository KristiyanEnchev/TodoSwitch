import { memo } from 'react';
import PropTypes from 'prop-types';

const Label = ({ children, ...props }) => (
  <label className="block mb-2" {...props}>
    {children}
  </label>
);

Label.displayName = 'Label';

Label.propTypes = {
  children: PropTypes.node.isRequired,
};

export default memo(Label);
