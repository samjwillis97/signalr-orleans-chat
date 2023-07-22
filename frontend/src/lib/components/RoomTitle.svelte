<script lang="ts">
	export let title = '';
	export let users = 0;
</script>

<div class="flex flex-row w-full items-center justify-between pb-1">
	<h4 class="scroll-m-20 text-xl font-semibold tracking-tight">{title}</h4>
	<div>
		{#if users == 0}
			No Users
		{:else if users == 1}
			1 User
		{:else}
			{users} Users
		{/if}
	</div>
</div>
