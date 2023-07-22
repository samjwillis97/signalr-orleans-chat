<script lang="ts">
	import { createLabel } from '@melt-ui/svelte';
	import { createEventDispatcher } from 'svelte';

	const root = createLabel();
	export let name = '';

	const dispatch = createEventDispatcher();

	function nextClicked() {
		if (name && name !== '') {
			dispatch('nextClicked');
		}
	}
</script>

<div class="flex flex-col items-start justify-center gap-2.5">
	<label use:root for="name" class="mb-0.5 font-medium" data-melt-part="root">
		<span>Name</span>
	</label>
	<div class="flex flex-row w-96">
		<input
			type="text"

			id="name"
			class="h-10 w-full rounded-md px-3 py-2 text-magnum-700 border mr-5"
			placeholder="Your name here"
			bind:value={name}
		/>
		<button
			class="h-10 text-orange-100 bg-orange-400 px-3 py-1 rounded-md font-medium hover:opacity-75 active:opacity-50"
			on:click={nextClicked}>Next</button
		>
	</div>
</div>
