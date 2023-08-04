import { configureStore, combineReducers } from '@reduxjs/toolkit';
import {
  persistStore,
  persistReducer,
  FLUSH,
  REHYDRATE,
  PAUSE,
  PERSIST,
  PURGE,
  REGISTER,
} from 'redux-persist';
import { apiSlice } from './apiSlice.jsx';
import authReducer from '../features/auth/authSlice.jsx';
import todoListReducer from '../features/todoLists/todoListSlice.jsx';
import selectedListReducer from '../features/todoLists/selectedListSlice.jsx';
import colorsReducer from '../features/todoLists/colorsSlice.jsx';
import themeReducer from '../theme/themeSlice.jsx';
import storage from 'redux-persist/lib/storage';
import modalReducer from '../features/modal/modalSlice.jsx';
import todoItemReducer from '../features/todoItems/todoItemSlice.jsx';

const rootReducer = combineReducers({
  [apiSlice.reducerPath]: apiSlice.reducer,
  auth: authReducer,
  theme: themeReducer,
  todoLists: todoListReducer,
  selectedList: selectedListReducer,
  colors: colorsReducer,
  modal: modalReducer,
  todoItems: todoItemReducer,
});

const persistConfig = {
  key: 'root',
  storage,
  whitelist: [
    'auth',
    'theme',
    'todoLists',
    'modal',
    'selectedList',
    'colors',
    'todoItems',
  ],
};

const persistedReducer = persistReducer(persistConfig, rootReducer);

export const store = configureStore({
  reducer: persistedReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
      },
    }).concat(apiSlice.middleware),
  devTools: true,
});

export const persistor = persistStore(store);
