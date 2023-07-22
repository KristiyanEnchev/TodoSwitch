import { createSlice } from '@reduxjs/toolkit';

const selectedListSlice = createSlice({
  name: 'selectedList',
  initialState: null,
  reducers: {
    setSelectedList: (state, action) => action.payload,
    updateSelectedList: (state, action) => {
      return { ...state, ...action.payload };
    },
  },
});

export const { setSelectedList, updateSelectedList } =
  selectedListSlice.actions;
export default selectedListSlice.reducer;
