<script lang="ts">
	import { connection } from '$lib/services/signalr';
	import { fade } from 'svelte/transition';

	export let name = '';
	let users: string[] = [];

	connection.on('usersTyping', (typing: string[]) => {
		users = typing.filter((v) => v !== name);
	});
</script>

<div class="flex flex-row justify-center pl-2 mb-2 h-5">
	{#if users.length > 0}
		<div transition:fade class="text-sm text-muted-foreground">
			{#if users.length == 1}
				{users[0]} is typing...
			{:else if users.length == 2}
				{users[0]}, and {users[1]} are typing...
			{:else}
				{users[0]}, {users[1]}, and {users.length - 2} others are typing...
			{/if}
		</div>
	{/if}
</div>
