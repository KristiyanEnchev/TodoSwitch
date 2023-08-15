import { memo, useState, useMemo, useCallback } from 'react';
import PropTypes from 'prop-types';
import TodoItemTitle from './TodoItemTitle.jsx';
import TodoItem from './TodoItem.jsx';
import DraggableList from '../../components/draggable/DraggableList.jsx';

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
  const [editingTask, setEditingTask] = useState(null);
  const [deletingTaskId, setDeletingTaskId] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [reorderedTasks, setReorderedTasks] = useState(tasks);

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

  const handleEdit = useCallback((task) => {
    setEditingTask(task);
  }, []);

  const handleCreate = useCallback(() => {
    setEditingTask(null);
  }, []);

  const handleDelete = useCallback((taskId) => {
    setDeletingTaskId(taskId);
  }, []);

  const handleReorder = useCallback((newOrder) => {
    const updatedOrder = newOrder.map((item, index) => ({
      ...item,
      orderIndex: index,
    }));
    setReorderedTasks(updatedOrder);
  }, []);

  const renderTaskItem = useCallback(
    (task) => (
      <TodoItem
        task={task}
        onToggle={onToggleTask}
        onDelete={() => handleDelete(task.id)}
        onEdit={() => handleEdit(task)}
      />
    ),
    [onToggleTask, handleDelete, handleEdit]
  );

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
      <DraggableList
        items={incompleteTasks}
        renderItem={renderTaskItem}
        onReorder={handleReorder}
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
