/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './**/*.{razor,html,css,js,ts,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        primary: '#4f46e5',
        secondary: '#6b7280'
      },
      borderRadius: {
        'none': '0px',
        'sm': '4px',
        'DEFAULT': '8px',
        'md': '12px',
        'lg': '16px',
        'xl': '20px',
        '2xl': '24px',
        '3xl': '32px',
        'full': '9999px',
        'button': '8px'
      }
    },
  },
  plugins: [],
}
