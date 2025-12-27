import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import { fileURLToPath, URL } from 'node:url';

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'https://localhost:5225',
        changeOrigin: true,
        secure: false, // 允许自签名证书
      },
    },
  },
  build: {
    outDir: 'dist',
    sourcemap: false,
  },
});
