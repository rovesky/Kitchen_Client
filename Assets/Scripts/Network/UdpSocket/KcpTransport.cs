using System;
using System.Collections.Generic;
using System.Net;
using Unity.Networking.Transport;
using EventType = Unity.Networking.Transport.NetworkEvent.Type;
using UdpNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

namespace FootStone.Kcp
{
    /// <summary>
    /// 网络参数配置
    /// </summary>
    public static class NetworkConfig
    {
        //MTU值
        public const int MTU = 1380;
        //数据包单帧大小，应该小于MTU值
        public const int packageFragmentSize = MTU - 120;
        //单个数据包最大帧数
        public const int maxFragments = 16;
        //数据包最大值
        public const int maxPackageSize = maxFragments * packageFragmentSize;

        //连接超时时间
        public const int connectTimeout = 5000;
        //超时重试次数
        public const int maxConnectAttempts = 3;
        //断开连接超时时间
        public const int disconnectTimeout = 30000;
    }
   

    /// <summary>
    /// 网络传输事件
    /// </summary>
    public struct TransportEvent
    {
        public enum Type
        {
            Data,
            Connect,
            Disconnect
        }
        public Type type;
        public int connectionId;
        public byte[] data;
        public int dataSize;
    }

    /// <summary>
    /// 网络传输接口
    /// </summary>
    public interface INetworkTransport
    {
        int Connect(string ip, int port);

        void Disconnect(int connectionId);

        void SendData(int connectionId, byte[] data, int sendSize);

        void Update();

        void Shutdown();
    }

    /// <summary>
    /// 网络事件回调接口
    /// </summary>
    public interface INetworkCallbacks
    {
        void OnConnect(KcpConnection connection);
        void OnDisconnect(int connectionId);
    }

    /// <summary>
    /// Kcp客户端
    /// </summary>
    public class KcpClient : KcpTransport
    {
        public KcpClient(INetworkCallbacks networkCallbacks)
            : base(networkCallbacks, 0,16)
        {
        }
    }

    /// <summary>
    /// Kcp服务端
    /// </summary>
    public class KcpServer : KcpTransport
    {
        public KcpServer(INetworkCallbacks networkCallbacks, int port = 0, int maxConnections = 16)
            :base(networkCallbacks,port,maxConnections)
        {
        }        
    }

    /// <summary>
    /// KCP传输
    /// </summary>
    public abstract class KcpTransport : INetworkTransport
    {
        public KcpTransport(INetworkCallbacks networkCallbacks, int port, int maxConnections)
        {
            this.networkCallbacks = networkCallbacks;

            m_IdToConnection = new List<KcpConnection>(maxConnections);
            for (int i = 0; i < maxConnections; ++i)
            {
                m_IdToConnection.Add(null);
            }

            m_Socket = new UdpNetworkDriver(new NetworkDataStreamParameter { size = 10 * NetworkConfig.maxPackageSize },
                new NetworkConfigParameter {
                    disconnectTimeout = NetworkConfig.disconnectTimeout,
                    connectTimeout = NetworkConfig.connectTimeout,
                    maxConnectAttempts = NetworkConfig.maxConnectAttempts});
            m_Socket.Bind(new IPEndPoint(IPAddress.Any, port));

            if (port != 0)
                m_Socket.Listen();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public int Connect(string ip, int port)
        {
            var connection = m_Socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
         
            m_IdToConnection[connection.InternalId] = new KcpConnection(
                connection, 
                m_Socket,
                m_Socket.GetConnectionRecvToken(connection),
                "client");
            return connection.InternalId;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="connectionId"></param>
        public void Disconnect(int connectionId)
        {
            m_IdToConnection[connectionId].Disconnect();           
            m_IdToConnection[connectionId] = null;
            networkCallbacks.OnDisconnect(connectionId);
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public KcpConnection GetConnection(int connectionId)
        {
            if (connectionId < 0 || connectionId >= m_IdToConnection.Count)
                return null;

            return m_IdToConnection[connectionId];
        }


        /// <summary>
        /// 主循环
        /// </summary>
        public void Update()
        {
            try
            {
                //socket update
                m_Socket.ScheduleUpdate().Complete();

                //kcp update
                foreach (var c in m_IdToConnection)
                {
                    if (c != null)
                    {
                        c.KcpUpdate();
                    }
                }

                //处理网络事件
                TransportEvent e = new TransportEvent();
                while (NextEvent(ref e))
                {
                    var connection = m_IdToConnection[e.connectionId];
                    if (connection == null)
                        continue;

                    if (e.type == TransportEvent.Type.Connect)
                    {
                        networkCallbacks.OnConnect(connection);
                    }
                    else if (e.type == TransportEvent.Type.Disconnect)
                    {
                     //   FSLog.Info($"TransportEvent.Type.Disconnect:{e.connectionId}");
                        m_IdToConnection[e.connectionId] = null;
                        networkCallbacks.OnDisconnect(e.connectionId);

                    }
                    else if (e.type == TransportEvent.Type.Data)
                    {
                        connection.KcpInput(e.data, e.dataSize);
                    }
                }
            }
            catch (Exception e)
            {
                FSLog.Error(e);
            }
        }

        /// <summary>
        /// 网络事件
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool NextEvent(ref TransportEvent e)
        {
            NetworkConnection connection;

            connection = m_Socket.Accept();
            if (connection.IsCreated)
            {
                e.type = TransportEvent.Type.Connect;
                e.connectionId = connection.InternalId;
                m_IdToConnection[connection.InternalId] = new KcpConnection(
                    connection, 
                    m_Socket,
                    m_Socket.GetConnectionSendToken(connection),
                    "server");
                return true;
            }

            DataStreamReader reader;
            var context = default(DataStreamReader.Context);
            var ev = m_Socket.PopEvent(out connection, out reader);

            if (ev == EventType.Empty)
                return false;

            int size = 0;
            if (reader.IsCreated)
            {
                // GameDebug.Assert(m_Buffer.Length >= reader.Length);
                reader.ReadBytesIntoArray(ref context, ref m_Buffer, reader.Length);
                size = reader.Length;
            }

            switch (ev)
            {
                case EventType.Data:
                    e.type = TransportEvent.Type.Data;
                    e.data = m_Buffer;
                    e.dataSize = size;
                    e.connectionId = connection.InternalId;
                    break;
                case EventType.Connect:
                    e.type = TransportEvent.Type.Connect;
                    e.connectionId = connection.InternalId;                 
                    m_IdToConnection[connection.InternalId].SetConnection(connection);
                    break;
                case EventType.Disconnect:
                 //   FSLog.Info($"EventType.Disconnect:{connection.InternalId}");
                    e.type = TransportEvent.Type.Disconnect;
                    e.connectionId = connection.InternalId;                  
                    break;
                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="data"></param>
        /// <param name="sendSize"></param>
        public void SendData(int connectionId, byte[] data, int sendSize)
        {
            if (m_IdToConnection[connectionId] == null)
                return;

            m_IdToConnection[connectionId].KcpSend(data, sendSize);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
            m_Socket.Dispose();
            m_IdToConnection.Clear();
        }

        private byte[] m_Buffer = new byte[1024 * 8];
        private BasicNetworkDriver<IPv4UDPSocket> m_Socket;
        private INetworkCallbacks networkCallbacks;
        private List<KcpConnection> m_IdToConnection;
    }
}