import * as signalR from '@microsoft/signalr';

export const connection = new signalR.HubConnectionBuilder()
	.withUrl('http://localhost:5000/myhub')
	.configureLogging(signalR.LogLevel.Information)
	.build();

export async function start() {
	try {
		await connection.start();
		console.log('SignalR Connected.');
	} catch (err) {
		setTimeout(start, 5000);
	}
}

connection.onclose(async () => {
	await start();
});
