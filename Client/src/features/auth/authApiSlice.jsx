import { apiSlice } from '../../api/apiSlice.jsx';
import { setCredentials } from '../auth/authSlice.jsx';
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
  }),
});

export const { useLoginMutation, useRegisterMutation, useLogoutMutation } =
  authApiSlice;
