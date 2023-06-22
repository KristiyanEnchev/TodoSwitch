import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react-swc';
import tailwindcss from 'tailwindcss';

import fs from 'fs';

const folders = fs.readdirSync('./src', { withFileTypes: true });
const fileNames = folders
  .filter((dirent) => dirent.isDirectory())
  .map((dirent) => dirent.name);

const filePaths = fileNames.reduce(
  (acc, cur) => ({
    ...acc,
    [cur]: `/src/${cur}`,
  }),
  {}
);

export default defineConfig({
  plugins: [react()],
  css: {
    postcss: {
      plugins: [tailwindcss()],
    },
  },
  resolve: {
    alias: {
      ...filePaths,
    },
  },
});
