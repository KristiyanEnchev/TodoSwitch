import PropTypes from 'prop-types';
import { memo, useMemo } from 'react';
import { Plus, Search } from 'lucide-react';
import { motion } from 'framer-motion';
import Button from '../../components/ui/Button.jsx';
import CircularProgress from '../../components/circularProgress/CircularProgress.jsx';

const TodoItemTitle = ({
  color,
  title,
  searchTerm,
  setSearchTerm,
  handleCreate,
  completedTasks,
  filteredTasks,
}) => {
  const completionPercentage = useMemo(
    () =>
      filteredTasks.length > 0
        ? (completedTasks.length / filteredTasks.length) * 100
        : 0,
    [filteredTasks.length, completedTasks.length]
  );

  return (
    <div
      style={{ backgroundColor: color }}
      className="flex items-center justify-between mb-6 rounded-xl px-4 py-3"
    >
      <h2 className="text-2xl font-bold mr-4 text-white">{title}</h2>

      <div className="flex-grow relative mr-4">
        <motion.div
          className="relative"
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
        >
          <input
            type="text"
            placeholder="Search tasks..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-4 py-2 text-gray-700 bg-white rounded-full focus:outline-none focus:ring-2 focus:ring-blue-400 transition-all duration-300 ease-in-out"
          />
          <Search
            className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400"
            size={20}
          />
        </motion.div>
      </div>

      <div className="flex items-center space-x-4">
        <CircularProgress value={completionPercentage} />
        <div className="flex flex-col items-end">
          <Button
            onClick={handleCreate}
            size="sm"
            className="bg-green-500 hover:bg-green-600 text-white font-semibold py-2 px-4 rounded-full shadow-lg transition-all duration-300 ease-in-out transform hover:scale-105 whitespace-nowrap mb-2"
          >
            <Plus className="h-5 w-5 mr-2 inline-block" /> Add Task
          </Button>
          <div className="text-sm text-white font-medium whitespace-nowrap">
            {completedTasks.length} of {filteredTasks.length} tasks
          </div>
        </div>
      </div>
    </div>
  );
};

TodoItemTitle.propTypes = {
  color: PropTypes.string.isRequired,
  title: PropTypes.string.isRequired,
  searchTerm: PropTypes.string.isRequired,
  setSearchTerm: PropTypes.func.isRequired,
  completedTasks: PropTypes.array.isRequired,
  filteredTasks: PropTypes.array.isRequired,
  handleCreate: PropTypes.func.isRequired,
};

export default memo(TodoItemTitle);
