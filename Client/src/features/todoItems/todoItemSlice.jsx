import { createSlice } from '@reduxjs/toolkit';

const todoItemSlice = createSlice({
  name: 'todoItems',
  initialState: [],
  reducers: {
    setTodoItems: (state, action) => action.payload,
    addTodoItem: (state, action) => {
      state.push(action.payload);
    },
    updateTodoItem: (state, action) => {
      const index = state.findIndex((item) => item.id === action.payload.id);
      if (index !== -1) {
        state[index] = action.payload;
      }
    },
    removeTodoItem: (state, action) => {
      return state.filter((item) => item.id !== action.payload);
    },
  },
});

export const { setTodoItems, addTodoItem, updateTodoItem, removeTodoItem } =
  todoItemSlice.actions;
export default todoItemSlice.reducer;
