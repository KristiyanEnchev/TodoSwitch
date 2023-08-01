import { useEffect, memo } from 'react';
import PropTypes from 'prop-types';
import { useDispatch, useSelector } from 'react-redux';

import { Spinner } from '../components/index.jsx';
import { setSelectedList } from '../features/todoLists/selectedListSlice.jsx';
import {
  useFetchDashboardData,
  useFetchTodoItemsData,
} from '../hooks/useFetchDashboardData.jsx';

const Dashboard = () => {
  const dispatch = useDispatch();
  const user = useSelector((state) => state.auth.user);
  const selectedList = useSelector((state) => state.selectedList);

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

  if (isLoading || isLoadingItems) return <Spinner />;

  return (
    <div className="flex dashboard">
      {todoLists && colors ? (
        <div>Sidebar here</div>
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
};

export default memo(Dashboard);
