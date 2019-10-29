using FootStone.Kcp;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class NetworkClientSystem : ComponentSystem, FootStone.Kcp.INetworkCallbacks
    {
        private KcpClient kcpClient;
        private int conId = -1;
        private uint sessionId = 0;
        private List<int> connections = new List<int>();

  
        private WorldTimeSystem worldTimeSystem;

        public unsafe void OnConnect(KcpConnection connection)
        {
            FSLog.Info($"client connection created:{connection.Id}");
            conId = connection.Id;
            sessionId = connection.SessionId;
            connections.Add(connection.Id);

            connection.Recv += (inSequence, buffer) =>
            {
                //  FSLog.Info($"[{inSequence}] client recv data:{buffer.Length}");            
                var snapShot = GetSingleton<SnapshotFromServer>();
              
                snapShot.length = buffer.Length;
                snapShot.time = worldTimeSystem.GetCurrentTime();
                using (UnmanagedMemoryStream tempUMS = new UnmanagedMemoryStream((byte*)snapShot.data,
                    buffer.Length, buffer.Length, FileAccess.ReadWrite))
                {
                    tempUMS.Write(buffer, 0, buffer.Length);
                }             
                SetSingleton(snapShot);            
            };
        }

        public void OnDisconnect(int connectionId)
        {
            FSLog.Info($"client connection destroyed:{connectionId}");
            connections.Remove(connectionId);
            conId = -1;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            kcpClient = new KcpClient(this);

            FSLog.Info($"NetworkClientSystem OnCreate");
          //  playerCommandQuery = GetEntityQuery(ComponentType.ReadWrite<UserCommand>());
          //  snapShotQuery = GetEntityQuery(ComponentType.ReadWrite<SnapshotFromServer>());

            worldTimeSystem = World.GetOrCreateSystem<WorldTimeSystem>();
        }

        protected override void OnDestroy()
        {
            FSLog.Info($"OnDestroy {conId}");
            foreach (var id in connections.ToArray())
            {
                kcpClient.Disconnect(id);
            }

            connections.Clear();
            kcpClient.Shutdown();            
        }

        public bool IsSession(uint sessionId)
        {
            return this.sessionId == sessionId;
        }


        protected override void OnUpdate()
        {          
             //  FSLog.Info($"Update {conId}");
            if (connections.Count == 0 && conId < 0)
            {
                FSLog.Info($"client connection to 192.168.0.128:1001");
                try
                {
                    conId = kcpClient.Connect("127.0.0.1", 1001);
                }
                catch(Exception e)
                {
                    FSLog.Error(e);
                }
            }
        
            var connection = kcpClient.GetConnection(conId);
            if (connection != null)
            {
                //    FSLog.Info($"kcpClient.Update()");
                kcpClient.Update();
                GameManager.Instance.UpdateRtt(connection.RTT);
            }
        }

        public double getRTT()
        {
            var connection = kcpClient.GetConnection(conId);
            if (connection != null)
            {
                return connection.RTT;                        
            }
            return 0;
        }

        public void SendCommand(byte[] data)
        {
            var connection = kcpClient.GetConnection(conId);
            if (connection != null && connection.IsConnected)
            {
                kcpClient.SendData(connection.Id, data, data.Length);
            }
        }

        public bool IsConnected()
        {
            var connection = kcpClient.GetConnection(conId);
            if (connection != null && connection.IsConnected)
                return true;
            return false;
        }
    }
}
