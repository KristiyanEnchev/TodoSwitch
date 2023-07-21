import { apiSlice } from '../../api/apiSlice.jsx';
import { setTodoLists, updateTodoList } from '../todoLists/todoListSlice.jsx';
import { setColors } from '../todoLists/colorsSlice.jsx';

export const todoListsApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getAllTodoLists: builder.query({
      query: ({ userId }) => ({
        url: '/todolist/getforuser',
        params: { userId },
      }),
      providesTags: ['TodoList'],
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setTodoLists(data));
        } catch (error) {
          console.error('Error fetching todo lists:', error);
        }
      },
    }),
    getTodoListColors: builder.query({
      query: () => ({
        url: '/todolist/getcolors',
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setColors(data));
        } catch (error) {
          console.error('Error fetching todo list colors:', error);
        }
      },
    }),
    createTodoList: builder.mutation({
      query: (listCommand) => ({
        url: '/todolist/create',
        method: 'POST',
        body: listCommand,
      }),
      invalidatesTags: ['TodoList'],
    }),
    updateList: builder.mutation({
      query: (listCommand) => ({
        url: '/todolist/updatelist',
        method: 'PUT',
        body: listCommand,
      }),
      invalidatesTags: ['TodoList'],
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(updateTodoList(data));
        } catch (error) {
          console.error('Error updating todo list:', error);
        }
      },
    }),
    updateTodoListTitle: builder.mutation({
      query: (titleCommand) => ({
        url: '/todolist/updatetitle',
        method: 'PUT',
        body: titleCommand,
      }),
      invalidatesTags: ['TodoList'],
    }),
    updateTodoListColor: builder.mutation({
      query: (colorCommand) => ({
        url: '/todolist/updatecolor',
        method: 'PUT',
        body: colorCommand,
      }),
      invalidatesTags: ['TodoList'],
    }),
    changeTodoListOrder: builder.mutation({
      query: (orderCommand) => ({
        url: '/todolist/changeorder',
        method: 'PUT',
        body: orderCommand,
      }),
      invalidatesTags: ['TodoList'],
    }),
    deleteTodoList: builder.mutation({
      query: ({ userId, listId }) => ({
        url: '/todolist/delete',
        method: 'DELETE',
        params: { userId, listId },
      }),
      invalidatesTags: ['TodoList'],
    }),
    reorderList: builder.mutation({
      query: (orderCommand) => ({
        url: '/todolist/reorder',
        method: 'PUT',
        body: orderCommand,
      }),
      invalidatesTags: ['TodoList'],
    }),
  }),
});

export const {
  useGetAllTodoListsQuery,
  useGetTodoListColorsQuery,
  useCreateTodoListMutation,
  useUpdateListMutation,
  useUpdateTodoListTitleMutation,
  useUpdateTodoListColorMutation,
  useDeleteTodoListMutation,
  useChangeTodoListOrderMutation,
  useReorderListMutation,
} = todoListsApiSlice;
