<script lang="ts">
	import type { ChatMessage } from '$lib/models/chat';
	import { fade } from 'svelte/transition';

	export let message: ChatMessage;
	export let clientId: string = '';

	let hovered = false;
	let isOwnMessage = true;

	$: {
		isOwnMessage = message.clientId === clientId;
	}

	function messageHoverStart() {
		hovered = true;
	}

	function messageHoverEnd() {
		setTimeout(() => {
			hovered = false;
		}, 550);
	}

	function getBubbleClass(): string {
		return isOwnMessage
			? 'bg-orange-400 text-orange-100 rounded-br-sm'
			: 'rounded-bl-sm text-slate-800';
	}
</script>

<!-- svelte-ignore a11y-no-static-element-interactions -->
<div
	class="flex flex-row w-full justify-start items-center"
	class:justify-end={isOwnMessage}
	class:justify-start={!isOwnMessage}
>
	{#if hovered && isOwnMessage}
		<div in:fade out:fade class="pr-2 text-xs text-slate-400 text-muted-foreground">
			{message.time.toLocaleTimeString()}
		</div>
	{/if}
	<div
		class="max-w-[65%] border rounded-lg px-3 py-2 justify-self-start text-sm font-medium leading-none justify-self-end {getBubbleClass()}"
		on:mouseenter={() => messageHoverStart()}
		on:mouseleave={() => messageHoverEnd()}
	>
		{message.message}
	</div>
	{#if !isOwnMessage}
		<div class="pl-2 font-light text-xs text-slate-400">
			{message.name}
		</div>
		{#if hovered}
			<div in:fade out:fade class="pl-2 font-light text-xs text-slate-400">
				{message.time.toLocaleTimeString()}
			</div>
		{/if}
	{/if}
</div>
