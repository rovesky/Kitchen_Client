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
    public class NetworkClientSystem : ComponentSystem, INetworkCallbacks
    {
        private KcpClient kcpClient;
        private int conId = -1;
        private List<int> connections = new List<int>();

        private EntityQuery playerCommandQuery;
        private EntityQuery snapShotQuery;

        public unsafe void OnConnect(KcpConnection connection)
        {
            FSLog.Info($"client connection created:{connection.Id}");
            conId = connection.Id;
            connections.Add(connection.Id);

            connection.Recv += (inSequence, buffer) =>
            {
                //FSLog.Info($"[{inSequence}] client recv data:{buffer.Length}");

                if (snapShotQuery.CalculateEntityCount() > 0)
                {
                    var snapShot = snapShotQuery.GetSingleton<SnapshotFromServer>();
                    snapShot.length = buffer.Length;

                    using (UnmanagedMemoryStream tempUMS = new UnmanagedMemoryStream((byte*)snapShot.data,
                        buffer.Length, buffer.Length, FileAccess.ReadWrite))
                    {
                        tempUMS.Write(buffer, 0, buffer.Length);                      
                    }

                    snapShotQuery.SetSingleton(snapShot);
                }
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
            playerCommandQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerCommand>());
            snapShotQuery = GetEntityQuery(ComponentType.ReadWrite<SnapshotFromServer>());
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


        protected override void OnUpdate()
        {          
             //  FSLog.Info($"Update {conId}");
            if (connections.Count == 0 && conId < 0)
            {
                FSLog.Info($"client connection to 192.168.0.128:1001");
                try
                {
                    conId = kcpClient.Connect("192.168.0.128", 1001);
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

                if (connection.IsConnected)
                {
                    var userCommand = playerCommandQuery.GetSingleton<PlayerCommand>();
                    byte[] data = userCommand.ToData();
                    kcpClient.SendData(connection.Id, data, data.Length);    

                    GameManager.Instance.UpdateRtt(connection.RTT);
                }          
            }
        }
    }
}
