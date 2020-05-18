using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Moving : MonoBehaviour
{
    private Rigidbody rb;
    private float h = 0;
    private float v = 0;
    public float speed = 5.0f;
    public static float cnt = 0;
    public GameObject TireBack;
    public GameObject TireFront;
    public GameObject Board;
    public GameObject BikeHead;
    private static byte[] result = new byte[4];
    private float num;
    private int receiveLength;
    private Socket clientSocket;
    private bool playing;
    private Client client;

    void Start()
    {
        playing = true;
        rb = GetComponent<Rigidbody>();
        client = new Client("101.200.154.111",1094,speed);
    }
    private void FixedUpdate()
    {
        if(client != null)
            speed = client.GetSpeed();
        Move();
        Turn();
    }

    private void Move()
    {
        v = 1.0f;
        //v = Input.GetAxis("Vertical");
        Vector3 movement = transform.forward * v * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
        TireBack.transform.Rotate(-200 * v * speed * Time.deltaTime,0 , 0, Space.Self);
        TireFront.transform.Rotate(-200 * v * speed * Time.deltaTime, 0, 0, Space.Self);
        Board.transform.Rotate(-40 * v * speed * Time.deltaTime, 0, 0, Space.Self);
    }

    private void Turn()
    {
        h = Input.GetAxis("Horizontal");
        float angle = BikeHead.transform.localEulerAngles.z;
        if (!((angle < 335 && angle > 334 && h < 0) || (angle > 25 && angle < 26 && h > 0)))
            BikeHead.transform.Rotate(0,0,h,Space.Self);
        if(h<=0.05)
        {
            float x = 0;
            if(angle > 0.5 && angle < 26)
            {
                x = -0.5f;
            }
            else if(angle > 334 && angle < 359.5)
            {
                x = 0.5f;
            }
            BikeHead.transform.Rotate(0,0,x,Space.Self);
        }
        float turn = h * speed * Time.deltaTime * 10;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
    private void OnDestroy()
    {
        Debug.Log("程序结束");
        client.endThread();
    }
}

public class Client 
{
    public TcpClient client;
    TcpListener listener;
    private Thread t_receive;
    private Thread t_send;
    private string ip;
    private int port;
    private float speed;
    private byte[] sendmessage;
    private bool canSend;
    private bool isend;
    public Client(string ip,int port,float speed)
    {
        isend = false;
        canSend = false;
        this.ip = ip;
        this.port = port;
        this.speed = speed;
        try
        {
            /*Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = IPAddress.Parse(ip);
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);
            Debug.Log(111);
            s.Bind(iPEndPoint);
            Debug.Log("Socket连接成功");*/
            Debug.Log("正在尝试与服务器建立连接...");
            client = new TcpClient();
            client.Connect(IPAddress.Parse(ip),port);
            Debug.Log("连接服务器成功");
            t_send = new Thread(sendout);
            t_send.Start();
            t_receive = new Thread(listen);
            t_receive.Start();
        }
        catch(Exception ex)
        {
            Debug.Log("连接服务器失败");
            String str = "";
            str += ex.Message + "\n";//异常消息
            str += ex.StackTrace + "\n";//提示出错位置，不会定位到方法内部去
            str += ex.ToString() + "\n";//将方法内部和外部所有出错的位置提示出来
            Debug.Log(str);
            
        }
        
        
    }
    void listen()
    {
        int receiveLength;
        float num;
        byte[] result = new byte[4];
        NetworkStream ns;
        while(true)
        {
            ns = client.GetStream();
            while (client.Available > 0 && !isend)
            {
                receiveLength = ns.Read(result,0,4);
                reverse(result);
                num = BitConverter.ToSingle(result, 0);
                Debug.Log("接收服务器消息：" + receiveLength + " / " + num);
                if (num - speed > 1)
                {
                    num = speed + 1;
                }
                if (speed - num > 1)
                {
                    num = speed - 1;
                }
                if(num >= 0 && num <= 100)
                    speed = num;
            }
            if (client.Available == 0) 
            {
                Thread.Sleep(500);
            }
        }
    }
    void sendout()
    {
        NetworkStream ns;
        while(true && !isend)
        {
            ns = client.GetStream();
            if (canSend)
            {
                ns.Write(sendmessage,0,sendmessage.Length);
                ns.Flush();
                canSend = false;
            }
            else 
            {
                Thread.Sleep(500);
            }
        }
    }
    void send(string Massage)
    {
        sendmessage = System.Text.Encoding.Default.GetBytes(Massage);
        canSend = true;
    }
    public float GetSpeed()
    {
        return speed;
    }
    public void endThread()
    {
        isend = true;
    }
    private void reverse(byte[] num)
    {
        byte temp;
        for(int i = 0; i < 2;i++)
        {
            temp = num[i];
            num[i] = num[3 - i];
            num[3 - i] = temp;
        }
    }
}