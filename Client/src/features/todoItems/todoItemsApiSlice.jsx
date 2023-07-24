import { apiSlice } from '../../api/apiSlice.jsx';
import { setTodoItems } from '../todoItems/todoItemSlice.jsx';

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
  }),
});

export const { useGetPagedTodosForUserQuery } = todoItemsApiSlice;
