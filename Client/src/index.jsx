import React, { Suspense } from 'react';
import { createRoot } from 'react-dom/client';
import { Provider } from 'react-redux';
import { HelmetProvider } from 'react-helmet-async';
import { BrowserRouter } from 'react-router-dom';
import { PersistGate } from 'redux-persist/integration/react';

import { store, persistor } from './api/store.jsx';

import '@fortawesome/fontawesome-free/css/all.min.css';
import 'react-toastify/dist/ReactToastify.css';
import './index.css';

import App from './App.jsx';
import Spinner from './components/spinner/Spinner.jsx';

createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <HelmetProvider>
      <Provider store={store}>
        <PersistGate loading={null} persistor={persistor}>
          <BrowserRouter>
            <Suspense fallback={<Spinner></Spinner>}>
              <App />
            </Suspense>
          </BrowserRouter>
        </PersistGate>
      </Provider>
    </HelmetProvider>
  </React.StrictMode>
);
