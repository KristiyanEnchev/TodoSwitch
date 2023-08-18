import { memo, useState, useCallback, useMemo } from 'react';
import PropTypes from 'prop-types';
import DeleteConfirmation from '../../components/modal/DeleteConfirmation.jsx';
import TodoItem from './TodoItem.jsx';
import CompletedTaskList from './CompletedTaskList.jsx';
import Modal from '../../components/modal/Modal.jsx';
import { useDispatch } from 'react-redux';
import { closeModal, openModal } from '../../features/index.jsx';
import TodoItemTitle from './TodoItemTitle.jsx';
import TodoItemForm from './TodoItemForm.jsx';
import { Button } from '../../components/index.jsx';
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
  const dispatch = useDispatch();
  const [modalContent, setModalContent] = useState(null);
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

  const handleEdit = useCallback(
    (task) => {
      setEditingTask(task);
      setModalContent('edit');
      dispatch(openModal('TodoItems'));
    },
    [dispatch]
  );

  const handleCreate = useCallback(() => {
    setEditingTask(null);
    setModalContent('create');
    dispatch(openModal('TodoItems'));
  }, [dispatch]);

  const handleDelete = useCallback(
    (taskId) => {
      setDeletingTaskId(taskId);
      setModalContent('delete');
      dispatch(openModal('TodoItems'));
    },
    [dispatch]
  );

  const handleSave = useCallback(
    (taskData) => {
      if (editingTask) {
        onEditTask(editingTask.id, taskData);
      } else {
        onCreateTask(taskData);
      }
      dispatch(closeModal());
    },
    [editingTask, onEditTask, onCreateTask, dispatch]
  );

  const handleConfirmDelete = useCallback(() => {
    onDeleteTask(deletingTaskId);
    dispatch(closeModal());
  }, [deletingTaskId, onDeleteTask, dispatch]);

  const handleCloseModal = useCallback(() => {
    dispatch(closeModal());
  }, [dispatch]);

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
      <Button
        onClick={() => onSaveOrder(reorderedTasks)}
        size="sm"
        className="bg-green-500 hover:bg-green-600 text-white font-semibold py-2 px-4 rounded-full shadow-lg transition-all duration-300 ease-in-out transform hover:scale-105 whitespace-nowrap mb-2"
      >
        Save Order
      </Button>
      <CompletedTaskList
        tasks={completedTasks}
        onToggle={onToggleTask}
        onDelete={handleDelete}
        onEdit={handleEdit}
      />
      <Modal
        title={
          modalContent === 'delete'
            ? 'Confirm Deletion'
            : editingTask
            ? 'Edit Task'
            : 'Create New Task'
        }
        showButtons={false}
        onClose={handleCloseModal}
        modalType={'TodoItems'}
      >
        {modalContent === 'delete' ? (
          <DeleteConfirmation
            message="Are you sure you want to delete this task?"
            showButtons={true}
            onConfirm={handleConfirmDelete}
            onCancel={handleCloseModal}
          />
        ) : (
          <TodoItemForm
            onSave={handleSave}
            showButton={true}
            initialData={editingTask}
            handleClose={handleCloseModal}
          />
        )}
      </Modal>
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
