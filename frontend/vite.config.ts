import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

export default defineConfig({
	plugins: [sveltekit()],
	define: {
		'import.meta.env.RAILWAY_SERVICE_ORLEANS_URL': JSON.stringify(
			process.env.RAILWAY_SERVICE_ORLEANS_URL
		)
	}
});
