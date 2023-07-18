import { apiSlice } from '../../api/apiSlice.jsx';

export const todoListsApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getAllTodoLists: builder.query({
      query: ({ userId }) => ({
        url: '/todolist/getforuser',
        params: { userId },
      }),
      providesTags: ['TodoList'],
      async onQueryStarted(arg, { queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
        } catch (error) {
          console.error('Error fetching todo lists:', error);
        }
      },
    }),
    getTodoListColors: builder.query({
      query: () => ({
        url: '/todolist/getcolors',
      }),
      async onQueryStarted(arg, { queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
        } catch (error) {
          console.error('Error fetching todo list colors:', error);
        }
      },
    }),
  }),
});

export const { useGetAllTodoListsQuery, useGetTodoListColorsQuery } =
  todoListsApiSlice;
