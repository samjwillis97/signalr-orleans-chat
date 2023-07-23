<script lang="ts">
	import Chat from '$lib/components/Chat.svelte';
	import NameInput from '$lib/components/NameInput.svelte';
	import RoomInput from '$lib/components/RoomInput.svelte';
	import RoomTitle from '$lib/components/RoomTitle.svelte';
	import TypingBanner from '$lib/components/TypingBanner.svelte';
	import { connection, start } from '$lib/services/signalr';
	import { document } from 'postcss';

	let socketId = '';
	let name = '';
	let room = '';
	let currentUserCount = 0;

	let messages: { clientId: string; name: string; message: string; time: Date }[] = [];

	let state: 'waitingForName' | 'waitingForRoom' | 'inChatRoom' = 'waitingForName';

	function nameNextClicked() {
		state = 'waitingForRoom';
	}

	function roomLeft() {
		state = 'waitingForRoom';
		messages = [];
		room = '';
		currentUserCount = 0;
	}

	async function joinClicked() {
		await start();

		if (!connection.connectionId) throw new Error('Unable to connect to SignalR');

		socketId = connection.connectionId;
		await connection.invoke('JoinRoom', name, room);
		connection.on('messageSent', handleMessageSent);
		connection.on('userJoined', handleUserJoined);
		connection.on('userLeft', handleUserLeft);

		state = 'inChatRoom';
	}

	function handleKeyUp(event: KeyboardEvent) {
		switch (event.key) {
			case 'Escape':
				if (state === 'waitingForRoom') {
					state = 'waitingForName';
				}
				if (state === 'waitingForName') {
          name = '';
				}
		}
	}

	function handleMessageSent(clientId: string, name: string, message: string, date: string) {
		if (clientId === socketId) return;
		const parsedDate = new Date(date);
		messages = [...messages, { clientId, name, message, time: parsedDate }];
	}

	function handleUserJoined(clientId: string, name: string, currentCount: number) {
		currentUserCount = currentCount;
	}

	function handleUserLeft(clientId: string, name: string, currentCount: number) {
		currentUserCount = currentCount;
	}
</script>

<svelte:window on:keyup={handleKeyUp} />

<div class="flex flex-col items-center justify-between h-screen">
	<div class="mt-5">
		<h1 class="scroll-m-20 text-4xl font-extrabold tracking-tight lg:text-5xl">SignalR Chat</h1>
	</div>

	<div class="w-96">
		<div class="flex flex-col w-full">
			{#if state === 'waitingForName'}
				<NameInput bind:name on:nextClicked={nameNextClicked} />
			{:else if state === 'waitingForRoom'}
				<RoomInput bind:room on:joinClicked={joinClicked} />
			{:else if state === 'inChatRoom'}
				<RoomTitle bind:title={room} bind:users={currentUserCount} />
				<TypingBanner bind:name />
				<Chat bind:messages bind:clientId={socketId} on:leftRoom={roomLeft} />
			{/if}
		</div>
	</div>
	<div />
</div>
