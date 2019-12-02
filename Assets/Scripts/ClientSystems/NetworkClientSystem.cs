﻿using FootStone.ECS;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class NetworkClientSystem : ComponentSystem, INetworkClientCallbacks
    {
        private NetworkClient network;
     //   private WorldTimeSystem worldTimeSystem;
     //   private SpawnEntitiesClientSystem spawnEntitiesClientSystem;
        private ReplicateEntitySystemGroup replicateEntitySystemGroup;

        public bool IsConnected => network.isConnected;
      //  public ReplicatedEntityClient EntityClient { get; } = new ReplicatedEntityClient(World.Active);

        protected override void OnCreate()
        {
            base.OnCreate();          

         //   worldTimeSystem = World.GetOrCreateSystem<WorldTimeSystem>();
        //    spawnEntitiesClientSystem = World.GetOrCreateSystem<SpawnEntitiesClientSystem>();
            replicateEntitySystemGroup = World.GetOrCreateSystem<ReplicateEntitySystemGroup>();

            var snapshotEntity = EntityManager.CreateEntity(typeof(ServerSnapshot));
            SetSingleton(new ServerSnapshot()
            {
                Tick = 0,
                Time = 0,
                Rtt = 0,
                LastAcknowlegdedCommandTime = 0,
            });

            network = new NetworkClient(GameWorld.Active);
        }

        protected override void OnDestroy()
        {
            network.Shutdown();
        }    


        protected override void OnUpdate()
        {
            if(network.connectionState == NetworkClient.ConnectionState.Disconnected)
            {
                network.Connect("58.247.94.202");
               // network.Connect("211.75.33.162");             
                //  network.Connect("192.168.0.128");
            }

            network.Update(this, replicateEntitySystemGroup);
        
            var snapshotFromServer = GetSingleton<ServerSnapshot>();        
            snapshotFromServer.Tick = (uint)network.serverTime;
            snapshotFromServer.Time = network.timeSinceSnapshot;
            snapshotFromServer.Rtt = network.rtt;
            snapshotFromServer.LastAcknowlegdedCommandTime = network.lastAcknowlegdedCommandTime;   
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
            FSLog.Info($"OnConnect:{network.clientId},clientId:{clientId}");
        }

        public void OnEvent(int clientId, NetworkEvent info)
        {
          
        }

        public void OnDisconnect(int clientId)
        {
            var localPlayer = GetSingleton<LocalPlayer>();
            localPlayer.PlayerId = -1;
            SetSingleton(localPlayer);
        }

        public void OnMapUpdate(ref NetworkReader data)
        {
           // FSLog.Error($"OnConnect:{network.clientId}");
            var localPlayer = GetSingleton<LocalPlayer>();
            localPlayer.PlayerId = network.clientId;
            SetSingleton(localPlayer);
            replicateEntitySystemGroup.SetLocalPlayerId(network.clientId);
        }     
    }
}
