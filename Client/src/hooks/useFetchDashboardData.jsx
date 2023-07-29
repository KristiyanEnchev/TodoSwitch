import {
  useGetAllTodoListsQuery,
  useGetTodoListColorsQuery,
  useCreateTodoListMutation,
  useUpdateListMutation,
  useDeleteTodoListMutation,
} from '../features/todoLists/apiTodoListSlice.jsx';
import {
  useGetPagedTodosForUserQuery,
  useCreateTodoItemMutation,
  useToggleTodoStatusMutation,
  useUpdateTodoItemMutation,
  useDeleteTodoItemMutation,
} from '../features/todoItems/todoItemsApiSlice.jsx';
import { skipToken } from '@reduxjs/toolkit/query/react';

export const useFetchDashboardData = (userId) => {
  const todoListsQuery = useGetAllTodoListsQuery({ userId });
  const colorsQuery = useGetTodoListColorsQuery();

  const isLoading = todoListsQuery.isLoading || colorsQuery.isLoading;
  const isError = todoListsQuery.isError || colorsQuery.isError;
  const data = {
    todoLists: todoListsQuery.data,
    colors: colorsQuery.data,
  };

  const [createTodoList] = useCreateTodoListMutation();
  const [updateTodoList] = useUpdateListMutation();
  const [deleteTodoList] = useDeleteTodoListMutation();

  return {
    ...data,
    isLoading,
    isError,
    refetch: () => {
      todoListsQuery.refetch();
      colorsQuery.refetch();
    },
    createTodoList,
    updateTodoList,
    deleteTodoList,
  };
};

export const useFetchTodoItemsData = (userId, list) => {
  const todoItemsQuery = useGetPagedTodosForUserQuery(
    list && list.id
      ? {
          userId: userId,
          listId: list.id,
          pageNumber: 1,
          pageSize: 100,
        }
      : skipToken
  );

  const [createTodoItem] = useCreateTodoItemMutation();
  const [toggleTodoStatus] = useToggleTodoStatusMutation();
  const [updateTodoItem] = useUpdateTodoItemMutation();
  const [deleteTodoItem] = useDeleteTodoItemMutation();

  const isLoading = todoItemsQuery.isLoading;
  const isError = todoItemsQuery.isError;
  const data = {
    todoItems: todoItemsQuery.data,
  };

  return {
    ...data,
    isLoading,
    isError,
    refetch: () => {
      todoItemsQuery.refetch();
    },
    createTodoItem,
    toggleTodoStatus,
    updateTodoItem,
    deleteTodoItem,
  };
};
