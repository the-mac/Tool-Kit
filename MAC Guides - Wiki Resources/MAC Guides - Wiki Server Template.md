### This MAC Guide describes how to create a Java TCP Server application using the Eclipse IDE.

Prerequisites for this guide are HowTo:
* [Build a Java Application (in Eclipse)](https://github.com/the-mac/Tool-Kit/wiki/Java-(Eclipse)---How-To)
* Build an Android or iOS Client App
    * [Build an Android Network Application (in Eclipse)](https://github.com/the-mac/Tool-Kit/wiki/Android-Network-(Eclipse)--HowTo)
    * [Build an iOS Network Application (in Xcode)](#)

In this guide, you will be covering the following topics:
* [Creating a project by copying and pasting source code](https://github.com/the-mac/Tool-Kit/wiki/Java-Server-(Eclipse)---HowTo#1-creating-server-project-by-copying-and-pasting-source-code)
* [Starting the Server implementation](https://github.com/the-mac/Tool-Kit/wiki/Java-Server-(Eclipse)---HowTo#2-starting-the-server-implementation)
* [Connecting the Client Application (As a Mobile Developer)](https://github.com/the-mac/Tool-Kit/wiki/Java-Server-(Eclipse)---HowTo#3-connecting-the-client-application-as-a-mobile-developer)

## 1 Creating Server project by copying and pasting source code

### 1.1 Copy source code

Highlight the source code below and copy it (CTRL/CMD + C or Right Click)


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
	
See more on the main method at [Oracle's Getting Started Guide](http://docs.oracle.com/javase/tutorial/getStarted/application/index.html).

### 1.2 Paste the Server source code

Paste the source code by right clicking in the Package Explorer and pasting the copied content.

![Paste Source](http://cse.spsu.edu/cslab/tutors/Tutorials/Java/template_imgs/shot_0_1.png)

### 1.3 Server project created

Your project is now generated for you in the package explorer, with the following project structure. The only difference from the [Build a Java Application (in Eclipse)](https://github.com/the-mac/Tool-Kit/wiki/Java-(Eclipse)---How-To) MAC Guide, is that the file name is now Server.java.

![First Project](http://cse.spsu.edu/cslab/tutors/Tutorials/Java/template_imgs/shot_0_1_1.png)

## 2 Starting the Server implementation

Now that you have a project, you can run your Server application. To start the Java Server Application, select your project by right clicking on it, Then select Run-As-> Java Application . You should get the following result:

       Server started and waiting for client connection!

       Listening on IP: 10.0.0.6

Note: The second line that contains the Server IP Address will be dependent upon your own Wifi configuration.

## 3 Connecting the Client Application (As a Mobile Developer)
		
### 3.1 Connect the Client Project

Once you connect your completed Client Application, you will be presented with the IP Address of the client and a "waiting for client input" message. The result would look as follows:

       Server started and waiting for client connection!

       Listening on IP: 10.0.0.6

       Connected to 10.0.0.3, and waiting for client input!

### 3.2 Send a Message from Client App

The final result would look as follows:

       Server started and waiting for client connection!

       Listening on IP: 10.0.0.6

       Connected to 10.0.0.3, and waiting for client input!

       In from client (10.0.0.3): my input
       Out to client (10.0.0.3): MY INPUT
	