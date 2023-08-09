import { useEffect, memo, useCallback } from 'react';
import { toast } from 'react-toastify';
import PropTypes from 'prop-types';
import { useDispatch, useSelector } from 'react-redux';
import Sidebar from './side/Sidebar.jsx';
import { setSelectedList } from '../features/todoLists/selectedListSlice.jsx';
import { Spinner } from '../components/index.jsx';
import {
  useFetchDashboardData,
  useFetchTodoItemsData,
} from '../hooks/useFetchDashboardData.jsx';
import {
  addTodoList,
  updateTodoList as updateTodoListAction,
  removeTodoList,
} from '../features/todoLists/todoListSlice.jsx';
import { useReorderListMutation } from '../features/todoLists/apiTodoListSlice.jsx';

const Dashboard = () => {
  const dispatch = useDispatch();
  const user = useSelector((state) => state.auth.user);
  const selectedList = useSelector((state) => state.selectedList);

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

  const { todoItems, isLoading: isLoadingItems } = useFetchTodoItemsData(
    user.Id,
    selectedList
  );

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
      console.log(changedList);

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
          <div>TodoList here</div>
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
