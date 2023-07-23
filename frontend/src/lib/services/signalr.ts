import * as signalR from '@microsoft/signalr';

const url = import.meta.env.RAILWAY_SERVICE_ORLEANS_URL
	? import.meta.env.RAILWAY_SERVICE_ORLEANS_URL + '/myhub'
	: 'http://localhost:5000/myhub';

export const connection = new signalR.HubConnectionBuilder()
	.withUrl(url)
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
