
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;
import org.json.simple.parser.ParseException;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.awt.image.DataBufferInt;
import java.io.*;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.util.concurrent.ArrayBlockingQueue;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.TimeUnit;

import static javax.imageio.ImageIO.*;

/**
 * Created by Zhiling on 5/20/17.
 */
public class Receiver implements Runnable {

    // unbounded
    ArrayBlockingQueue<byte[]> receivedMsgs = new ArrayBlockingQueue<byte[]>(2000);

    private DatagramSocket mySocket;
//    private int myPort;

    private boolean isRunning = true;

    private int timeOut = 5000;

    public static final int WIDTH = 1920;
    public static final int HEIGHT = 1080;

    public Receiver(int port) {

        try {
            mySocket = new DatagramSocket(port);
            mySocket.setSoTimeout(timeOut);        // wait for 0.5 second for data
        } catch (SocketException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void run() {
        DatagramPacket packet;
        while (isRunning) {
            /* buffer is filled with the data received */
            try {
                InetAddress address = InetAddress.getByName("127.0.0.1");
                byte[] msg = new byte[6000];
                packet = new DatagramPacket(msg, msg.length);
                /* blocks until it receives a datagram */
                mySocket.receive(packet);
                receivedMsgs.offer(msg, timeOut, TimeUnit.MILLISECONDS);
                System.out.println(new String(msg));
            } catch (IOException | InterruptedException ioe) {
                ioe.printStackTrace();
            }
        }
    }

    private void HandlePacket(DatagramPacket packet, byte[] msg){

        System.out.println("hi");
    }

    private void SaveAsColorIMG(byte[] imageBytes) throws IOException {

//        BufferedImage img = new BufferedImage(WIDTH,HEIGHT BufferedImage.TYPE_INT_ARGB);
        System.out.println("hi");
        InputStream in = new ByteArrayInputStream(imageBytes);
        BufferedImage img = read(in);
        if (img == null)
            System.out.println("jho");

        write(img, "jpg", new File("C:\\Users\\durian_milk\\Pictures\\hi.jpg"));
    }
    /**
     * Stops the thread that is running and shut down
     */
    public void stop() {
        // interrupt a blocked socket.
        isRunning = false;
        mySocket.close();
    }


    public BlockingQueue<byte[]> getMsgQueue() {
        return receivedMsgs;
    }

    public static void main(String[] args) {
//        String ipAddress = null;
        JSONParser parser = new JSONParser();
        int port = -1;
        if (args.length < 1) {

        }
        // Check given file is Simple Text File using Java
        else if (args.length == 1) try {
            Object obj = parser.parse(new FileReader(args[0]));
            JSONObject jsonObject = (JSONObject) obj;

            port =  Integer.parseInt((String) jsonObject.get("port"));
//                ipAddress = (String)jsonObject.get("ip address");

        } catch (ParseException | IOException e) {
            e.printStackTrace();
        }

        else if (args.length == 2)  {
            port = Integer.parseInt(args[0]);
//            ipAddress = args[1];
        }
        else
            System.out.println("Invalid arguments");

        Receiver receiver = new Receiver(port);
        new Thread(receiver).start();

    }
}
