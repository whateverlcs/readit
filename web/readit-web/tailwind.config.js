/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'readit-primary': '#14213D',  // Roxo moderno (sugestão)
        'readit-dark': '#14171C',     // Dark mode personalizado
        'readit-light': '#F0F0F5',    // Light mode clean
        'readit-text': '#F0F0F5',
      },
    },
  },
  plugins: [],
}