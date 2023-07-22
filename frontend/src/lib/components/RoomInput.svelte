<script lang="ts">
	import { createLabel } from '@melt-ui/svelte';
	import { createEventDispatcher } from 'svelte';

	const root = createLabel();
	export let room = '';

	const dispatch = createEventDispatcher();

	function joinClicked() {
		if (room && room !== '') {
			dispatch('joinClicked');
		}
	}
</script>

<div class="flex flex-col items-start justify-center gap-2.5">
	<label use:root for="name" class="mb-0.5 font-medium" data-melt-part="root">
		<span>Room</span>
	</label>
	<div class="flex flex-row w-96">
		<input
			type="text"
			id="room"
			class="h-10 w-full rounded-md px-3 py-2 text-magnum-700 border mr-5"
			placeholder="Enter Room Name"
			bind:value={room}
		/>
		<button
			class="h-10 text-orange-100 bg-orange-400 px-3 py-1 rounded-md font-medium hover:opacity-75 active:opacity-50"
			on:click={joinClicked}>Join</button
		>
	</div>
</div>
