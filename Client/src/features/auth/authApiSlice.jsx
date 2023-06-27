import { apiSlice } from '../../api/apiSlice.jsx';
import { persistor } from '../../api/store.jsx';
import { setCredentials, logOut } from '../auth/authSlice.jsx';
import { jwtDecode } from 'jwt-decode';

export const authApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation({
      query: (credentials) => ({
        url: '/identity/login',
        method: 'POST',
        body: credentials,
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          const { accessToken, refreshToken } = data;
          const user = jwtDecode(accessToken);
          const isAuthenticated = true;
          dispatch(
            setCredentials({ accessToken, refreshToken, user, isAuthenticated })
          );
        } catch (error) {
          console.error('Login failed: ', error);
        }
      },
    }),
    register: builder.mutation({
      query: (credentials) => ({
        url: '/identity/register',
        method: 'POST',
        body: credentials,
      }),
      async onQueryStarted(arg, { queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          return data;
        } catch (error) {
          console.error('Register failed: ', error);
        }
      },
    }),
    logout: builder.mutation({
      query: ({ email }) => ({
        url: '/identity/logout',
        method: 'POST',
        body: { email },
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          dispatch(logOut());
          await persistor.purge();
        } catch (error) {
          console.error('Error during logout or purge:', error);
        }

        try {
          const { data } = await queryFulfilled;
          return data;
        } catch (error) {
          console.error('Logout API call failed:', error);
        }
      },
    }),
  }),
});

export const { useLoginMutation, useRegisterMutation, useLogoutMutation } =
  authApiSlice;
