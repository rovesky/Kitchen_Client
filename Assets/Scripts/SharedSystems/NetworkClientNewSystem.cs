using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class NetworkClientNewSystem : ComponentSystem, INetworkClientCallbacks
    {
        private NetworkClient network;
        private WorldTimeSystem worldTimeSystem;
        private SpawnEntitiesClientSystem spawnEntitiesClientSystem;

        public bool IsConnected => network.isConnected;

        protected override void OnCreate()
        {
            base.OnCreate();          

            worldTimeSystem = World.GetOrCreateSystem<WorldTimeSystem>();
            spawnEntitiesClientSystem = World.GetOrCreateSystem<SpawnEntitiesClientSystem>();

            var snapshotEntity = EntityManager.CreateEntity(typeof(SnapshotFromServer));
            SetSingleton(new SnapshotFromServer()
            {
                tick = 0,
                time = 0,
                rtt = 0,
                lastAcknowlegdedCommandTime = 0,
            });

            network = new NetworkClient();
        }

        protected override void OnDestroy()
        {
            network.Shutdown();
        }    


        protected override void OnUpdate()
        {
            if(network.connectionState == NetworkClient.ConnectionState.Disconnected)
            {
                network.Connect("127.0.0.1");
            }

            network.Update(this, spawnEntitiesClientSystem);
         //   GameManager.Instance.UpdateRtt(network.rtt);
            var snapshotFromServer = GetSingleton<SnapshotFromServer>();        
            snapshotFromServer.tick = (uint)network.serverTime;
            snapshotFromServer.time = network.timeSinceSnapshot;
            snapshotFromServer.rtt = network.rtt;
            snapshotFromServer.lastAcknowlegdedCommandTime = network.lastAcknowlegdedCommandTime;   
            SetSingleton(snapshotFromServer);
        }


        public void QueueCommand(uint tick, NetworkClient.DataGenerator data )
        {
            network.QueueCommand((int)tick, data);
        }


        public void SendData()
        {
            network.SendData();
        }


        public void OnConnect(int clientId)
        {
           
        }

        public void OnEvent(int clientId, NetworkEvent info)
        {
          
        }

        public void OnDisconnect(int clientId)
        {
           
        }

        public void OnMapUpdate(ref NetworkReader data)
        {
       
        }

     
    }
}
