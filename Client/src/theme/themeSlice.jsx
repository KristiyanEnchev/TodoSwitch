import { createSlice } from '@reduxjs/toolkit';
import { persistReducer } from 'redux-persist';
import storage from 'redux-persist/lib/storage';

const initialState = {
  themeMode: localStorage.getItem('themeMode') || 'dark',
  currentColor: localStorage.getItem('colorMode') || '#008080',
  themeSettings: false,
};

const themeSlice = createSlice({
  name: 'theme',
  initialState,
  reducers: {
    setThemeMode: (state, action) => {
      state.themeMode = action.payload;
      localStorage.setItem('themeMode', action.payload);
    },
    setCurrentColor: (state, action) => {
      state.currentColor = action.payload;
      localStorage.setItem('colorMode', action.payload);
    },
    toggleThemeSettings: (state) => {
      state.themeSettings = !state.themeSettings;
    },
    closeThemeSettings: (state) => {
      state.themeSettings = false;
    },
  },
});

export const {
  setThemeMode,
  setCurrentColor,
  toggleThemeSettings,
  closeThemeSettings,
} = themeSlice.actions;

const persistConfig = {
  key: 'theme',
  storage,
};

export const persistedThemeReducer = persistReducer(
  persistConfig,
  themeSlice.reducer
);

export const selectThemeMode = (state) => state.theme.themeMode;
export const selectCurrentColor = (state) => state.theme.currentColor;
export const selectThemeSettings = (state) => state.theme.themeSettings;

export default persistedThemeReducer;
