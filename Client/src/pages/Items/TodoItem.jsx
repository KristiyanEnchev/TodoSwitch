import { memo, useCallback } from 'react';
import PropTypes from 'prop-types';
import { CheckCircle, Circle, Edit, Trash2 } from 'lucide-react';
import { motion } from 'framer-motion';
import AlarmClock from '../../components/ui/AlarmClock.jsx';
import PriorityBadge from '../../components/ui/PriorityBadge.jsx';

const TodoItem = ({ task, onToggle, onDelete, onEdit }) => {
  const handleToggleClick = useCallback(
    () => onToggle(task.id),
    [onToggle, task.id]
  );
  const handleEditClick = useCallback(() => onEdit(task), [onEdit, task]);
  const handleDeleteClick = useCallback(
    () => onDelete(task.id),
    [onDelete, task.id]
  );

  return (
    <motion.div
      className="relative bg-white dark:bg-gray-700 rounded-lg shadow-md overflow-hidden mb-3"
      initial={{ opacity: 0, y: -20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
      whileHover={{ scale: 1.02 }}
    >
      <div className="p-3">
        <div className="flex items-center justify-between mb-1">
          <div className="flex items-center space-x-2">
            <button
              onClick={handleToggleClick}
              className={`rounded-full p-1 ${
                task.isDone
                  ? 'bg-green-500 text-white'
                  : 'bg-red-500 text-white'
              }`}
            >
              {task.isDone ? <CheckCircle size={16} /> : <Circle size={16} />}
            </button>
            <h3
              className={`font-medium ${
                task.isDone ? 'line-through text-gray-500' : ''
              }`}
            >
              {task.title}
            </h3>
            <PriorityBadge priority={task.priority} />
          </div>
          <span className="text-xs text-gray-400">{task.createdOn}</span>
        </div>
        <p
          className={`text-sm ${
            task.isDone
              ? 'text-gray-400'
              : 'text-gray-600 dark:text-white font-semibold'
          } mb-2`}
        >
          {task.note}
        </p>
        <div className="flex items-center justify-between text-xs text-gray-500">
          <div className="flex items-center space-x-2">
            <div>
              <AlarmClock
                deadline={task.reminder}
                isDone={task.isDone}
                message="Not completed on time"
              />
            </div>
          </div>
          <div className="flex items-center space-x-2">
            <button
              onClick={handleEditClick}
              className="p-1 bg-blue-100 text-blue-600 rounded hover:bg-blue-200 transition-colors"
            >
              <Edit size={18} />
            </button>
            <button
              onClick={handleDeleteClick}
              className="p-1 bg-red-100 text-red-600 rounded hover:bg-red-200 transition-colors"
            >
              <Trash2 size={18} />
            </button>
          </div>
        </div>
      </div>
    </motion.div>
  );
};

TodoItem.propTypes = {
  task: PropTypes.object.isRequired,
  onToggle: PropTypes.func.isRequired,
  onDelete: PropTypes.func.isRequired,
  onEdit: PropTypes.func.isRequired,
};

export default memo(TodoItem);
