import { memo } from 'react';
import PropTypes from 'prop-types';

const PriorityBadge = ({ priority }) => {
  const colors = {
    None: 'bg-blue-100 text-blue-800',
    Low: 'bg-yellow-100 text-yellow-800',
    Medium: 'bg-orange-100 text-orange-800',
    High: 'bg-red-100 text-red-800',
  };

  return (
    <span
      className={`px-2 py-1 rounded-full text-xs font-semibold ${colors[priority]}`}
    >
      {priority}
    </span>
  );
};

PriorityBadge.propTypes = {
  priority: PropTypes.string.isRequired,
};

export default memo(PriorityBadge);
