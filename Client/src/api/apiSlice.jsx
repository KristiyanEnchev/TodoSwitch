import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { setCredentials, logOut } from '../features/auth/authSlice.jsx';
import { jwtDecode } from 'jwt-decode';

const baseUrl = import.meta.env.VITE_API_BASE_URL;

const baseQuery = fetchBaseQuery({
  baseUrl: `${baseUrl}`,
  credentials: 'include',
  prepareHeaders: (headers, { getState }) => {
    const token = getState().auth.token || localStorage.getItem('token');
    if (token) {
      headers.set('Authorization', `Bearer ${token}`);
    }
    return headers;
  },
});

function debounce(func, wait) {
  let timeout;
  return function executedFunction(...args) {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
}

let isRefreshing = false;

const baseQueryWithReauth = async (args, api, extraOptions) => {
  const state = api.getState();
  let result = await baseQuery(args, api, extraOptions);

  if (
    (result?.error?.status === 403 || result?.error?.status === 401) &&
    state?.auth?.isAuthenticated
  ) {
    const refreshToken = api.getState().auth.refreshToken;
    const email = api.getState().auth.user?.Email;

    if (refreshToken && email && !isRefreshing) {
      isRefreshing = true;

      const debouncedRefresh = debounce(async () => {
        const refreshResult = await baseQuery(
          {
            url: '/identity/refresh',
            method: 'POST',
            body: {
              refreshToken,
              email,
            },
          },
          api,
          extraOptions
        );

        isRefreshing = false;

        if (refreshResult?.data) {
          const { accessToken, refreshToken } = refreshResult.data;

          const user = jwtDecode(accessToken);
          const isAuthenticated = true;
          api.dispatch(
            setCredentials({ accessToken, refreshToken, user, isAuthenticated })
          );
          result = await baseQuery(args, api, extraOptions);
        } else {
          api.dispatch(logOut());
        }
      }, 500);

      await debouncedRefresh();
    } else if (isRefreshing) {
      while (isRefreshing) {
        await new Promise((resolve) => setTimeout(resolve, 100));
      }

      result = await baseQuery(args, api, extraOptions);
    } else {
      api.dispatch(logOut());
      return { error: { status: 403, data: 'Refresh token or email missing' } };
    }
  }

  return result;
};

export const apiSlice = createApi({
  baseQuery: baseQueryWithReauth,
  tagTypes: ['TodoList', 'TodoItem'],
  endpoints: () => ({}),
});
