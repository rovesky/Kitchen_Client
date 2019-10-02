﻿using FootStone.Kcp;
using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class NetworkClientSystem : ComponentSystem, INetworkCallbacks
    {
        private KcpClient kcpClient;
        private int conId = -1;
        private List<int> connections = new List<int>();

        private PlayerCommand recvBuffer = default;


        public void OnConnect(KcpConnection connection)
        {
            FSLog.Info($"client connection created:{connection.Id}");
            conId = connection.Id;
            connections.Add(connection.Id);

         

            connection.Recv += (inSequence, buffer) =>
            {
                //  var recMsg = Encoding.UTF8.GetString(buffer);
                FSLog.Debug($"[{inSequence}] client recv data");
                recvBuffer.FromData(buffer);
                recvBuffer.renderTick = (int)inSequence;
                recvBuffer.isBack = true;

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
          
          //  FSLog.Info($"OnCreate {conId}");
        }

        protected override void OnDestroy()
        {
            FSLog.Info($"OnDestroy {conId}");
            connections.Clear();
            if (kcpClient != null)
            {
                kcpClient.Shutdown();
            }
        }
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref SpawnPlayer spawn) =>
            {
              //  FSLog.Info($"EntityManager.AddBuffer<PlayerId>(entity");
                if (!spawn.spawned)
                {
                    EntityManager.AddBuffer<PlayerId>(entity);
                    var buffer = EntityManager.GetBuffer<PlayerId>(entity);
                    buffer.Add(new PlayerId() { playerId = 0 });
                    spawn.spawned = true;
                }
            });

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
              //  FSLog.Info($"kcpClient.Update()");
                kcpClient.Update();
               
                Entities.WithAllReadOnly<Player>().ForEach((Entity entity, ref PlayerCommand command) =>
                {
                  //  FSLog.Info($"WithAllReadOnly<Player>()");
                    if (connection.IsConnected)
                    {
                        byte[] data = command.ToData();
                        kcpClient.SendData(conId, data, data.Length);

                        if (recvBuffer.isBack)
                        {
                            PostUpdateCommands.SetComponent(entity, recvBuffer);
                            recvBuffer = default;
                        }

                        GameManager.Instance.UpdateRtt(connection.RTT);
                    }

                });
            }
        }
    }
}
