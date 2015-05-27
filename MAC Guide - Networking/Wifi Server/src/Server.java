import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.UnknownHostException;

public class Server {

	private static String SERVERIP;

	/**
	 * @param args
	 * @throws IOException
	 */

	public static void main(String[] args) throws IOException {
		String clientSentence;
		String capitalizedSentence;

		ServerSocket welcomeSocket = new ServerSocket(8888);
		SERVERIP = getLocalIpAddress();

		System.out.println("Server started and waiting for client connection!\n");
		System.out.println("Listening on IP: " + SERVERIP + "\n\n");

		while (true) {
			Socket connectionSocket = welcomeSocket.accept();
			String ip = connectionSocket.getInetAddress().toString().substring(1);
			System.out.println("Connected to "+ip+", and waiting for client input!\n");

			BufferedReader inFromClient = new BufferedReader(
					new InputStreamReader(connectionSocket.getInputStream()));

			DataOutputStream outToClient = new DataOutputStream(connectionSocket.getOutputStream());

			clientSentence = inFromClient.readLine();
			System.out.println("In from client (" + ip + "): " + clientSentence);

			if (clientSentence != null) {
				capitalizedSentence = clientSentence.toUpperCase() + '\n';
				System.out.println("Out to client (" + ip + "): " + capitalizedSentence);
				outToClient.writeBytes(capitalizedSentence + "\n");
			}

		}
	}

	private static String getLocalIpAddress() {
		try {
			return InetAddress.getLocalHost().getHostAddress().toString();
		} catch (UnknownHostException e) {

			return null;
		}
	}
}