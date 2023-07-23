<script lang="ts">
	import { connection } from '$lib/services/signalr';
	import type { ChatMessage } from '$lib/models/chat';
	import { createEventDispatcher } from 'svelte';
	import ChatBubble from './ChatBubble.svelte';

	export let messages: ChatMessage[] = [];
	export let clientId: string = '';

	const dispatch = createEventDispatcher();

	let elemChat: HTMLElement;

	let message = '';
	let stoppedTypingSetTimeout: NodeJS.Timeout | undefined = undefined;

	async function handleMessageKeyDown() {
		if (stoppedTypingSetTimeout) {
			clearTimeout(stoppedTypingSetTimeout);
		} else {
			await connection.invoke('Typing');
		}

		stoppedTypingSetTimeout = setTimeout(() => {
			connection.invoke('StoppedTyping').then();
			stoppedTypingSetTimeout = undefined;
		}, 2000);
	}

	function handleKeyUp(event: KeyboardEvent) {
		switch (event.key) {
			case 'Enter':
				sendMessage();
				break;
			case 'Escape':
				leaveChat();
				break;
		}
	}

	$: if (messages && elemChat) {
		setTimeout(() => {
			scrollChatBottom('smooth');
		}, 0);
	}

	function scrollChatBottom(behavior?: ScrollBehavior): void {
		elemChat.scrollTo({ top: elemChat.scrollHeight, behavior });
	}

	async function sendMessage() {
		if (!message) return;
		await connection.invoke('SendMessage', message);
		messages = [...messages, { clientId, name: '', message, time: new Date() }];
		message = '';
	}

	async function leaveChat() {
		await connection.invoke('LeaveRoom');
		dispatch('leftRoom');
		message = '';
	}
</script>

<div bind:this={elemChat} class="h-96 border rounded-md py-1 px-0.5 overflow-y-scroll">
	<div class="h-full" />
	<div class="flex flex-col gap-1.5 w-full justify-end">
		{#each messages as message}
			<ChatBubble bind:message bind:clientId />
		{/each}
	</div>
</div>
<div class="flex flex-col w-full gap-2.5">
	<div class="flex flex-row w-full mt-4">
		<!-- svelte-ignore a11y-autofocus -->
		<input
			autofocus
			type="text"
			class="h-10 w-full rounded-md px-3 py-2 text-magnum-700 border mr-5"
			placeholder="Message"
			bind:value={message}
			on:keydown={handleMessageKeyDown}
			on:keyup={handleKeyUp}
		/>
		<button
			class="h-10 text-orange-100 bg-orange-400 px-3 py-1 rounded-md font-medium hover:opacity-75 active:opacity-50"
			on:click={sendMessage}>Send</button
		>
	</div>
	<div class="w-full">
		<button
			class="w-full h-10 text-red-100 bg-red-500 px-3 py-1 rounded-md font-medium hover:opacity-75 active:opacity-50"
			on:click={leaveChat}>Leave</button
		>
	</div>
</div>
