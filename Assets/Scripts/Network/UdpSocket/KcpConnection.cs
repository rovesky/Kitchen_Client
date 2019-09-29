using System;
using System.Buffers;
using System.IO;
using Unity.Collections;
using Unity.Networking.Transport;

namespace FootStone.Kcp
{
    /// <summary>
    /// Kcp连接
    /// </summary>
    public class KcpConnection : System.Net.Sockets.Kcp.IKcpCallback
    {
        //公共属性
        public Action<uint, byte[]> Recv;
        public int Id => connection.InternalId;
        public double RTT => statistics.CaculateRtt();
        public bool IsConnected => connection.GetState(m_Socket) == NetworkConnection.State.Connected;

        //私有属性
        private NetworkConnection connection;
        private System.Net.Sockets.Kcp.Kcp kcp;
        private BasicNetworkDriver<IPv4UDPSocket> m_Socket;
        private NetStatistics statistics = new NetStatistics();

        //发送和接收序号
        private uint outSequence = 0;
        private uint outSequenceAck = 0;
        private uint inSequence = 0;

        private string category = "";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="socket"></param>
        /// <param name="flag"></param>
        public KcpConnection(NetworkConnection connection, BasicNetworkDriver<IPv4UDPSocket> socket,uint sessionId, string category)
        {
            this.m_Socket = socket;
            this.category = category;
            this.connection = connection;

            //设置KCP参数           
            kcp = new System.Net.Sockets.Kcp.Kcp(sessionId, this);
            kcp.NoDelay(1, 0, 2, 1);//fast
            kcp.WndSize(64, 64);
            kcp.SetMtu(NetworkConfig.MTU);
        }

        public void SetConnection(NetworkConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// socket 发送数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="avalidLength"></param>
        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            using (var sendStream = new DataStreamWriter(avalidLength, Allocator.Persistent))
            {
                var data = buffer.Memory.Slice(0, avalidLength).ToArray();
                sendStream.Write(data, avalidLength);

                FSLog.Debug($"[{outSequence}]m_Socket.Send ");

                m_Socket.Send(connection, sendStream);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            m_Socket.Disconnect(connection);
        }

        /// <summary>
        /// Rent
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public IMemoryOwner<byte> RentBuffer(int length)
        {
            return null;
        }

        /// <summary>
        /// Kcp更新
        /// </summary>
        public void KcpUpdate()
        {
            kcp.Update(DateTime.UtcNow);
        }

        /// <summary>
        /// Kcp发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sendSize"></param>
        public void KcpSend(byte[] data, int sendSize)
        {
            if (connection.GetState(m_Socket) != NetworkConnection.State.Connected)
                return;

            MemoryStream memStream = new MemoryStream(sendSize + 8);
            BinaryWriter writer = new BinaryWriter(memStream);

            writer.Write(outSequence);
            writer.Write(inSequence);
            writer.Write(data, 0, sendSize);

            FSLog.Debug($"[{outSequence}][{category}] send msg!");

            kcp.Send(memStream.ToArray());
            KcpUpdate();

            statistics.Send(outSequence, DateTime.Now.Ticks);

            outSequence++;
        }

        /// <summary>
        /// Kcp输入数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataSize"></param>
        public void KcpInput(byte[] data, int dataSize)
        {
            var seq = inSequence == 0 ? 0 : inSequence + 1;
            FSLog.Debug($"[{seq}][{category}] send msg!");

            Memory<byte> mem = new Memory<byte>(data, 0, dataSize);
            kcp.Input(mem.Span);
            KcpRecv();
        }

        private void RecvData(byte[] data)
        {
            var memStream = new MemoryStream(data);
            var reader = new BinaryReader(memStream);

            inSequence = reader.ReadUInt32();
            outSequenceAck = reader.ReadUInt32();
            Recv(inSequence, reader.ReadBytes(data.Length - 8));

            statistics.Recv(outSequenceAck, DateTime.Now.Ticks);
        }

        private void KcpRecv()
        {
            int len;
            while ((len = kcp.PeekSize()) > 0)
            {
                var buffer = new byte[len];
                if (kcp.Recv(buffer) >= 0)
                {
                    RecvData(buffer);
                }
            }
        }
    }
}
