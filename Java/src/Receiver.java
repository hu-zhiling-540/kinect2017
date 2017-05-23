import java.io.IOException;
import java.net.*;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.concurrent.TimeUnit;

/**
 * Created by Zhiling on 5/20/17.
 */
public class Receiver implements Runnable{
	
    // unbounded
    BlockingQueue<byte[]> receivedMsgs = new LinkedBlockingQueue<byte[]>();
    
    private DatagramSocket mySocket;
    private int myPort;

    private boolean isRunning = true;
    
    public int timeOut = 5000;
    
    public Receiver(int port)   {
        myPort = port;
        try {
            mySocket = new DatagramSocket(myPort);
            mySocket.setSoTimeout(timeOut);		// wait for 0.5 second for data
        } catch (SocketException e) {
            e.printStackTrace();
        }
    }

	@Override
	public void run() {
		DatagramPacket packet = null;
        while (isRunning)   {
            // buffer is filled with the data received
            try {
            	InetAddress address = InetAddress.getByName ("127.0.0.1");
            	//InetAddress address = InetAddress.getLocalHost();
                byte[] msg = new byte[1024];
            	packet = new DatagramPacket(msg, msg.length, address, myPort );
			    mySocket.receive(packet);	// blocks until it receives a datagram
			    receivedMsgs.offer(msg, timeOut, TimeUnit.MILLISECONDS);
			    System.out.println(new String(msg));
            } catch (IOException ioe) {
                ioe.printStackTrace();
            } catch (InterruptedException ie) {
				ie.printStackTrace();
			}
        }
	}
	
	public void stop(){
		// interrupt a blocked socket.
		isRunning = false;
		mySocket.close();
	}
	
	public BlockingQueue<byte[]> getMsgQueue()	{
		return receivedMsgs;
	}
	
    public static void main(String[] args)  {
        Receiver receiver = new Receiver(8008);
        new Thread(receiver).start();
    }
}
