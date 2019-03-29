using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using System.IO;

/*
public class LidgrenTest : MonoBehaviour {

    public NetServer server;
    public NetClient client;
    public int port = 14242;

    private static ulong s_length = 0;
    private static ulong s_received = 0;
    private static FileStream s_writeStream;
    private float s_timeStarted;

    public string fileName = @"D:\\worldConfig.xml";

    // Use this for initialization
    void Start () {
        NetPeerConfiguration configClient = new NetPeerConfiguration("DW");
        NetPeerConfiguration configServer = new NetPeerConfiguration("DW");
        configServer.Port = port;

        InitializeServer(configServer);
        InitializeClient(configClient);
    }

    // Update is called once per frame
    void Update()
    {
        //HandleClient();
        HandleServer();
    }

    private void OnDestroy()
    {
        server.Shutdown("time at the bar");
        client.Shutdown("no more drinks for you");
    }

    //-------------- CLIENT --------------//
    void InitializeClient(NetPeerConfiguration config)
    {
        client = new NetClient(config);
        client.Start();
        client.Connect("localhost", port);

        Debug.Log("Client status: " + client.Status.ToString());
    }

    void HandleClient()
    {
        NetIncomingMessage inc;
        while ((inc = client.ReadMessage()) != null) {
            switch (inc.MessageType) {
                case NetIncomingMessageType.Data:
                    int chunkLen = inc.LengthBytes;
                    if (s_length == 0) {
                        s_length = inc.ReadUInt64();
                        string filename = inc.ReadString();
                        s_writeStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                        s_timeStarted = Time.time;
                        break;
                    }

                    byte[] all = inc.ReadBytes(inc.LengthBytes);
                    s_received += (ulong)all.Length;
                    s_writeStream.Write(all, 0, all.Length);

   
                    if (s_received >= s_length) {
                        float passed = Time.time - s_timeStarted;
                        double psec = (double)passed / 1000.0;
                        double bps = (double)s_received / psec;
                        Debug.Log("Done at " + NetUtility.ToHumanReadable((long)bps) + " per second");

                        Debug.Log("FileName: " + s_writeStream.Name);

                        s_writeStream.Flush();
                        s_writeStream.Close();
                        s_writeStream.Dispose();

                        //client.Disconnect("Everything received, bye!");
                    }
                    break;
            }
            client.Recycle(inc);
        }
    }


    //-------------- SERVER --------------//
    void InitializeServer(NetPeerConfiguration config)
    {
        server = new NetServer(config);
        server.Start();

        Debug.Log("Server status: " + server.Status.ToString());
    }

	void HandleServer()
    {
        NetIncomingMessage inc;
        while ((inc = server.ReadMessage()) != null) {
            switch (inc.MessageType) {
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    Debug.Log(inc.ReadString());
                    break;
                case NetIncomingMessageType.StatusChanged:
                    NetConnectionStatus status = (NetConnectionStatus)inc.ReadByte();
                    switch (status) {
                        case NetConnectionStatus.Connected:
                            // start streaming to this client
                            inc.SenderConnection.Tag = new StreamingClient(inc.SenderConnection, @"D:\\worldConfig.xml");
                            Debug.Log("Starting streaming to " + inc.SenderConnection);
                            break;
                        default:
                            Debug.Log(inc.SenderConnection + ": " + status + " (" + inc.ReadString() + ")");
                            break;
                    }
                    break;
            }
            server.Recycle(inc);
        }

        // stream to all connections
        foreach (NetConnection conn in server.Connections) {
            StreamingClient client = conn.Tag as StreamingClient;
            if (client != null)
                client.Heartbeat();
        }

        //System.Threading.Thread.Sleep(0);
    }
}
*/