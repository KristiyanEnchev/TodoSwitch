import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

const baseUrl = import.meta.env.VITE_API_BASE_URL;

const baseQuery = fetchBaseQuery({
  baseUrl: `${baseUrl}`,
  credentials: 'include',
  prepareHeaders: (headers, { getState }) => {
    const token = getState().auth.token;
    if (token) {
      headers.set('Authorization', `Bearer ${token}`);
    }
    return headers;
  },
});

export const apiSlice = createApi({
  baseQuery: baseQuery,
  tagTypes: [],
  endpoints: () => ({}),
});
