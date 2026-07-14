/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './wwwroot/js/**/*.js'
    ],
    theme: {
        extend: {
            fontFamily: { sans: ['Inter', 'sans-serif'] },
            colors: {
                skynet: {
                    black: '#0a0a0a',
                    dark: '#111111',
                    card: '#161616',
                    border: '#222222',
                    accent: '#3b82f6',
                    muted: '#888888'
                },
                zinc: {
                    500: '#888888',
                    600: '#555555',
                    700: '#333333',
                    800: '#222222',
                    900: '#161616',
                    950: '#0a0a0a'
                },
                cyan: {
                    100: '#dbeafe',
                    200: '#bfdbfe',
                    300: '#93c5fd',
                    400: '#60a5fa',
                    500: '#3b82f6',
                    600: '#2563eb'
                }
            }
        }
    },
    // Status colors are applied at runtime by the toast/confirm helpers in
    // _Layout.cshtml (classList.add with literal strings). They live in inline
    // <script> that the content scanner also reads, but we safelist the full
    // set here so a future refactor that moves that JS out can't purge them.
    safelist: [
        'bg-green-500/10', 'text-green-500', 'border-green-500/20',
        'bg-red-500/10', 'text-red-500', 'border-red-500/20',
        'bg-yellow-500/10', 'text-yellow-500', 'border-yellow-500/20',
        'bg-skynet-accent/10', 'text-skynet-accent', 'border-skynet-accent/20',
        'bg-red-500', 'hover:bg-red-600'
    ],
    plugins: []
};
