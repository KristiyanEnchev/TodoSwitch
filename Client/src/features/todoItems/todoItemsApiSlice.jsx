import { apiSlice } from '../../api/apiSlice.jsx';
import { setTodoItems, updateTodoItem } from '../todoItems/todoItemSlice.jsx';

export const todoItemsApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getPagedTodosForUser: builder.query({
      query: ({ pageNumber, pageSize, userId, listId }) => ({
        url: '/todoitem/getpagedtodosforuser',
        params: { pageNumber, pageSize, userId, listId },
      }),
      providesTags: ['TodoItem'],
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setTodoItems(data.data));
        } catch (error) {
          console.error('Error fetching paged todos:', error);
        }
      },
    }),
    getTodoItemById: builder.query({
      query: ({ userId, listId, itemId }) => ({
        url: '/todoitem/getbyid',
        params: { userId, listId, itemId },
      }),
      providesTags: (result, error, arg) => [
        { type: 'TodoItem', id: arg.itemId },
      ],
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(updateTodoItem(data));
        } catch (error) {
          console.error('Error fetching todo item by ID:', error);
        }
      },
    }),
    createTodoItem: builder.mutation({
      query: (todoItem) => ({
        url: '/todoitem/createtodo',
        method: 'POST',
        body: todoItem,
      }),
      invalidatesTags: ['TodoItem', 'TodoList'],
    }),
    toggleTodoStatus: builder.mutation({
      query: (toggleCommand) => ({
        url: '/todoitem/togglestatus',
        method: 'PUT',
        body: toggleCommand,
      }),
      invalidatesTags: (result, error, arg) => [
        { type: 'TodoItem', id: arg.itemId },
        'TodoList',
      ],
    }),
    changeTodoPriority: builder.mutation({
      query: (priorityCommand) => ({
        url: '/todoitem/changepriority',
        method: 'PUT',
        body: priorityCommand,
      }),
      invalidatesTags: (result, error, arg) => [
        { type: 'TodoItem', id: arg.itemId },
      ],
    }),
    changeTodoOrder: builder.mutation({
      query: (orderCommand) => ({
        url: '/todoitem/changeorder',
        method: 'PUT',
        body: orderCommand,
      }),
      invalidatesTags: ['TodoItem'],
    }),
    reorder: builder.mutation({
      query: (orderCommand) => ({
        url: '/todoitem/reorder',
        method: 'PUT',
        body: orderCommand,
      }),
      invalidatesTags: ['TodoItem'],
    }),
    updateTodoItem: builder.mutation({
      query: (itemCommand) => ({
        url: '/todoitem/updateitem',
        method: 'PUT',
        body: itemCommand,
      }),
      invalidatesTags: (result, error, arg) => [
        { type: 'TodoItem', id: arg.itemId },
      ],
    }),
    deleteTodoItem: builder.mutation({
      query: ({ userId, listId, itemId }) => ({
        url: '/todoitem/delete',
        method: 'DELETE',
        params: { userId, listId, itemId },
      }),
      invalidatesTags: ['TodoItem', 'TodoList'],
    }),
    changeTodoItemOrder: builder.mutation({
      query: (orderCommand) => ({
        url: '/todoitem/changeorder',
        method: 'PUT',
        body: orderCommand,
      }),
      invalidatesTags: ['TodoItem'],
    }),
  }),
});

export const {
  useGetPagedTodosForUserQuery,
  useGetTodoItemByIdQuery,
  useCreateTodoItemMutation,
  useToggleTodoStatusMutation,
  useChangeTodoPriorityMutation,
  useUpdateTodoItemMutation,
  useDeleteTodoItemMutation,
  useChangeTodoItemOrderMutation,
  useReorderMutation,
} = todoItemsApiSlice;
