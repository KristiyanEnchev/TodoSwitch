import { useEffect, memo, useCallback } from 'react';
import { toast } from 'react-toastify';
import PropTypes from 'prop-types';
import { useDispatch, useSelector } from 'react-redux';
import Sidebar from './side/Sidebar.jsx';
import TodoItemList from './Items/TodoItemList.jsx';
import { setSelectedList } from '../features/todoLists/selectedListSlice.jsx';
import { Spinner } from '../components/index.jsx';
import { removeTodoItem } from '../features/todoItems/todoItemSlice.jsx';
import { useReorderMutation } from '../features/todoItems/todoItemsApiSlice.jsx';
import { useReorderListMutation } from '../features/todoLists/apiTodoListSlice.jsx';
import {
  useFetchDashboardData,
  useFetchTodoItemsData,
} from '../hooks/useFetchDashboardData.jsx';
import {
  addTodoList,
  updateTodoList as updateTodoListAction,
  removeTodoList,
} from '../features/todoLists/todoListSlice.jsx';

const Dashboard = () => {
  const dispatch = useDispatch();
  const user = useSelector((state) => state.auth.user);
  const selectedList = useSelector((state) => state.selectedList);

  const [reorderTasks] = useReorderMutation();
  const tasks = useSelector((state) => state.todoItems);

  const [reorderList] = useReorderListMutation();
  const lists = useSelector((state) => state.todoLists);

  const {
    todoLists,
    colors,
    isLoading,
    createTodoList,
    updateTodoList,
    deleteTodoList,
  } = useFetchDashboardData(user.Id);

  const {
    todoItems,
    isLoading: isLoadingItems,
    refetch,
    createTodoItem,
    toggleTodoStatus,
    updateTodoItem,
    deleteTodoItem,
  } = useFetchTodoItemsData(user.Id, selectedList);

  useEffect(() => {
    if (todoLists?.length > 0 && !selectedList) {
      dispatch(setSelectedList(todoLists[0]));
    }
  }, [todoLists, selectedList, dispatch]);

  const handleSelectList = useCallback(
    (listId) => {
      const selected = todoLists.find((list) => list.id === listId);
      if (selected) {
        dispatch(setSelectedList(selected));
      }
    },
    [todoLists, dispatch]
  );

  const handleCreateOrUpdateList = useCallback(
    async (listData) => {
      try {
        Object.assign(listData, { userId: user.Id });
        if (listData.listId) {
          const result = await updateTodoList(listData).unwrap();
          dispatch(updateTodoListAction(result));
        } else {
          const result = await createTodoList(listData).unwrap();
          dispatch(addTodoList(result));
        }
      } catch (error) {
        console.error('Error creating/updating list:', error);
      }
    },
    [user.Id, updateTodoList, createTodoList, dispatch]
  );

  const handleDeleteList = useCallback(
    async (id) => {
      try {
        await deleteTodoList({ userId: user.Id, listId: id }).unwrap();
        dispatch(removeTodoList(id));
        if (selectedList?.id === id) {
          dispatch(setSelectedList(todoLists[0] || null));
        }
      } catch (error) {
        console.error('Error deleting list:', error);
      }
    },
    [user.Id, selectedList, todoLists, deleteTodoList, dispatch]
  );

  const handleToggleTask = useCallback(
    async (id) => {
      try {
        await toggleTodoStatus({
          userId: user.Id,
          listId: selectedList?.id,
          itemId: id,
        }).unwrap();
        refetch();
      } catch (error) {
        console.error('Error toggling task status:', error);
      }
    },
    [user.Id, selectedList, toggleTodoStatus, refetch]
  );

  const handleDeleteTask = useCallback(
    async (id) => {
      try {
        await deleteTodoItem({
          userId: user.Id,
          listId: selectedList?.id,
          itemId: id,
        }).unwrap();
        dispatch(removeTodoItem(id));
      } catch (error) {
        console.error('Error deleting task:', error);
      }
    },
    [user.Id, selectedList, deleteTodoItem, dispatch]
  );

  const handleEditTask = useCallback(
    async (id, updatedData) => {
      try {
        await updateTodoItem({
          userId: user.Id,
          listId: selectedList?.id,
          itemId: id,
          title: updatedData.title,
          note: updatedData.note,
          priority: updatedData.priority,
        }).unwrap();
        refetch();
      } catch (error) {
        console.error('Error editing task:', error);
      }
    },
    [user.Id, selectedList, updateTodoItem, refetch]
  );

  const handleCreateTask = useCallback(
    async (taskData) => {
      try {
        const todo = { ...taskData, listId: selectedList?.id };
        await createTodoItem({
          todo,
          userId: user.Id,
        }).unwrap();
        refetch();
      } catch (error) {
        console.error('Error creating task:', error);
      }
    },
    [user.Id, selectedList, createTodoItem, refetch]
  );

  const handleSaveOrder = useCallback(
    async (reorderedTasks) => {
      if (!reorderedTasks || !tasks) {
        return;
      }

      const changedItems = {};
      reorderedTasks.forEach((reorderedTask, newIndex) => {
        const originalTask = tasks.find((task) => task.id === reorderedTask.id);
        if (originalTask && originalTask.orderIndex !== newIndex) {
          changedItems[reorderedTask.id] = newIndex;
        }
      });

      if (Object.keys(changedItems).length > 0) {
        try {
          await reorderTasks({
            userId: user.Id,
            listId: selectedList?.id,
            changedItems: changedItems,
          });
        } catch (error) {
          console.error('Failed to save new order:', error);
          toast.error('Failed to save new order:', error);
        }
      }
    },
    [reorderTasks, user.Id, selectedList?.id, tasks]
  );

  const handleSaveOrderList = useCallback(
    async (reorderedList) => {
      if (!reorderedList || !lists) {
        return;
      }

      const changedList = {};
      reorderedList.forEach((reorderedList, newIndex) => {
        const originalList = lists.find((list) => list.id === reorderedList.id);
        if (originalList && originalList.orderIndex !== newIndex) {
          changedList[reorderedList.id] = newIndex;
        }
      });

      if (Object.keys(changedList).length > 0) {
        try {
          await reorderList({
            userId: user.Id,
            changedList: changedList,
          });
        } catch (error) {
          console.error('Failed to save new order:', error);
          toast.error('Failed to save new order:', error);
        }
      }
    },
    [lists, reorderList, user.Id]
  );

  if (isLoading || isLoadingItems) return <Spinner />;

  const color = selectedList?.color?.code || 'transparent';

  return (
    <div className="flex dashboard">
      {todoLists && colors ? (
        <Sidebar
          lists={todoLists}
          colors={colors}
          onSelectList={handleSelectList}
          onCreateOrUpdateList={handleCreateOrUpdateList}
          onDeleteList={handleDeleteList}
          onSaveOrderList={handleSaveOrderList}
        />
      ) : (
        <div>Select a list or create a new Item to get started!</div>
      )}
      <div className="flex-grow p-4 bg-gray-300 dark:bg-black dark:text-white text-black border-l border-gray-400 dark:border-gray-700 shadow-inner">
        {selectedList && todoItems.data ? (
          <TodoItemList
            title={selectedList.title}
            tasks={todoItems.data}
            onToggleTask={handleToggleTask}
            onDeleteTask={handleDeleteTask}
            onEditTask={handleEditTask}
            onCreateTask={handleCreateTask}
            color={color}
            onSaveOrder={handleSaveOrder}
          />
        ) : (
          <div>Select a list or create a new Item to get started!</div>
        )}
      </div>
    </div>
  );
};

Dashboard.propTypes = {
  user: PropTypes.object,
  selectedList: PropTypes.object,
  todoLists: PropTypes.array,
  colors: PropTypes.array,
  isLoading: PropTypes.bool,
  isLoadingItems: PropTypes.bool,
  refetch: PropTypes.func,
  createTodoList: PropTypes.func,
  updateTodoList: PropTypes.func,
  deleteTodoList: PropTypes.func,
  todoItems: PropTypes.object,
  toggleTodoStatus: PropTypes.func,
  updateTodoDescription: PropTypes.func,
  deleteTodoItem: PropTypes.func,
};

export default memo(Dashboard);
