import { createSlice } from '@reduxjs/toolkit';

const todoListSlice = createSlice({
  name: 'todoLists',
  initialState: [],
  reducers: {
    setTodoLists: (state, action) => action.payload,
    addTodoList: (state, action) => {
      state.push(action.payload);
    },
    updateTodoList: (state, action) => {
      const index = state.findIndex((list) => list.id === action.payload.id);
      if (index !== -1) {
        state[index] = action.payload;
      }
    },
    removeTodoList: (state, action) => {
      return state.filter((list) => list.id !== action.payload);
    },
  },
});

export const { setTodoLists, addTodoList, updateTodoList, removeTodoList } =
  todoListSlice.actions;
export default todoListSlice.reducer;
