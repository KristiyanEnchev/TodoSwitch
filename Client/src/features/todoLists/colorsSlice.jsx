import { createSlice } from '@reduxjs/toolkit';

const initialState = {
  colors: [],
  selectedColor: null,
};

const colorsSlice = createSlice({
  name: 'colors',
  initialState,
  reducers: {
    setColors: (state, action) => {
      state.colors = action.payload;
    },
    setSelectedColor: (state, action) => {
      state.selectedColor = action.payload;
    },
  },
});

export const { setColors, setSelectedColor } = colorsSlice.actions;
export default colorsSlice.reducer;
