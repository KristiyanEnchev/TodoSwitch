import { memo, useState, useMemo, useCallback } from 'react';
import PropTypes from 'prop-types';
import TodoItemTitle from './TodoItemTitle.jsx';

const TodoItemList = ({
  title,
  tasks,
  onToggleTask,
  onDeleteTask,
  onEditTask,
  onCreateTask,
  color,
  onSaveOrder,
}) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [editingTask, setEditingTask] = useState(null);

  const filteredTasks = useMemo(
    () =>
      tasks.filter((task) =>
        task.title.toLowerCase().includes(searchTerm.toLowerCase())
      ),
    [tasks, searchTerm]
  );

  const completedTasks = useMemo(
    () => filteredTasks.filter((task) => task.isDone),
    [filteredTasks]
  );

  const incompleteTasks = useMemo(
    () => filteredTasks.filter((task) => !task.isDone),
    [filteredTasks]
  );

  const handleCreate = useCallback(() => {
    setEditingTask(null);
  }, []);

  return (
    <div className="bg-white dark:bg-gray-800 shadow-lg rounded-lg p-6 mb-4">
      <TodoItemTitle
        color={color}
        title={title}
        searchTerm={searchTerm}
        setSearchTerm={setSearchTerm}
        handleCreate={handleCreate}
        completedTasks={completedTasks}
        filteredTasks={filteredTasks}
      />
    </div>
  );
};

TodoItemList.propTypes = {
  title: PropTypes.string.isRequired,
  tasks: PropTypes.array.isRequired,
  onToggleTask: PropTypes.func.isRequired,
  onDeleteTask: PropTypes.func.isRequired,
  onEditTask: PropTypes.func.isRequired,
  onCreateTask: PropTypes.func.isRequired,
  color: PropTypes.string.isRequired,
  onSaveOrder: PropTypes.func.isRequired,
};

export default memo(TodoItemList);
