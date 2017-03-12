using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System;

public class NetworkManager : Singleton<NetworkManager>
{
    public bool isConnected = false;
    public bool isServer = false;
    public bool isInit = false;
    public const int bufferSize = 1024;

    public string host = "127.0.0.1";
    public int port = 8000;

    private int hostId;
    private int connectionId;
    private ConnectionConfig config;
    private HostTopology topology;
    private byte channelId;

    public Transform avatarPrefab;
    public Transform ballPrefab;
    Transform avatar;

    public InputField hostText;
    public InputField portText;

    public void Init(bool isServer)
    {
        byte error;
        host = hostText.text;
        port = Convert.ToInt32(portText.text.ToString());
        this.isServer = isServer;
        NetworkTransport.Init();
        config = new ConnectionConfig();
        config.AddChannel(QosType.Reliable);
        if (isServer)
        {
            topology = new HostTopology(config, 10);
            hostId = NetworkTransport.AddHost(topology, port);
            Debug.Log("Server started on port" + port + " with id of " + hostId);
        }
        else
        {
            topology = new HostTopology(config, 1);
            hostId = NetworkTransport.AddHost(topology, 0);
            connectionId = NetworkTransport.Connect(hostId, host, port, 0, out error);
            NetworkError networkError = (NetworkError)error;
            if (networkError != NetworkError.Ok)
            {
                Debug.LogError(string.Format("Unable to connect to {0}:{1}, Error: {2}", host, port, networkError));
            }
            else
            {
                Debug.Log(string.Format("Connected to {0}:{1} with hostId: {2}, connectionId: {3}, channelId: {4},", host, port, hostId, connectionId, channelId));

            }
        }
        isInit = true;
    }

    void Update()
    {
        if (!isInit)
            return;
        int recHostId;
        int recConnectionId;
        int recChannelId;
        byte[] recBuffer = new byte[NetworkManager.bufferSize];
        int dataSize;
        byte error;
        NetworkEventType networkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, NetworkManager.bufferSize, out dataSize, out error);

        NetworkError networkError = (NetworkError)error;
        if (networkError != NetworkError.Ok)
        {
            Debug.LogError(string.Format("Error recieving event: {0} with recHostId: {1}, recConnectionId: {2}, recChannelId: {3}", networkError, recHostId, recConnectionId, recChannelId));
        }

        switch (networkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log(string.Format("incoming connection event received with connectionId: {0}, recHostId: {1}, recChannelId: {2}", recConnectionId, recHostId, recChannelId));
                avatar = (Transform)Instantiate(avatarPrefab);
                if (isServer)
                    connectionId = recConnectionId;
                isConnected = true;
                break;
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                OnRecvGameMsg(recBuffer, dataSize);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("remote client " + recConnectionId + " disconnected");
                break;
        }
    }


    public void OnRecvGameMsg(byte[] data, int length)
    {
        object o;
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream s = new MemoryStream(data))
        {
            try
            {
                o = formatter.Deserialize(s);
                s.Close();
            }
            catch
            {
                Debug.Log("deserialize error!!!!!");
                return;
            }
        }
        Messages msg = o as Messages;
        if (msg == null)
        {
            return;
        }
        else
        {
            if (msg.type == Messages.Type.Position)
            {
                PlayerState state = msg.content as PlayerState;
                avatar.position = state.position.V3;
                avatar.rotation = state.rotation.Q;
            }

            if (msg.type == Messages.Type.Fire)
            {
                Fire state = msg.content as Fire;
                Transform ball = (Transform)Instantiate(ballPrefab, state.position.V3, state.rotation.Q);
            }

        }
    }

    public void SendGameMsg(Messages msg)
    {
        if (!isConnected || !isInit)
            return;
        BinaryFormatter formatter = new BinaryFormatter();
        byte[] data = new byte[NetworkManager.bufferSize];
        using (MemoryStream s = new MemoryStream())
        {
            formatter.Serialize(s, msg);
            data = s.ToArray();
        }
        byte error;
        NetworkTransport.Send(hostId, connectionId, channelId, data, NetworkManager.bufferSize, out error);
    }

}
