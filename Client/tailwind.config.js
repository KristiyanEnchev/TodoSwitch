/** @type {import('tailwindcss').Config} */
export default {
  mode: 'jit',
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        myTeal: '#008080',
        myPurple: '#6d28d9',
        myOrange: '#ffa500',
        myRed: '#dc2626',
        myGreen: '#008000',
        redExtra: '#dc2626',
      },
      fontFamily: {
        display: ['Open Sans', 'sans-serif'],
        body: ['Open Sans', 'sans-serif'],
      },
      fontSize: {
        '4xl': '2.5rem',
      },
      padding: {
        4: '1rem',
      },
      margin: {
        7: '1.75rem',
      },
      maxWidth: {
        '3xl': '48rem',
      },
      boxShadow: {
        teal: '0 4px 6px -1px rgba(0, 128, 128, 0.1), 0 2px 4px -1px rgba(0, 128, 128, 0.06)',
      },
    },
  },
  plugins: [],
};
