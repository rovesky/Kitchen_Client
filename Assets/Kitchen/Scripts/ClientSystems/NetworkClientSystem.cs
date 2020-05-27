using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class NetworkClientSystem : ComponentSystem, INetworkClientCallbacks
    {
        private NetworkClient network;
        private ReplicateEntitySystemGroup replicateEntitySystemGroup;

        public bool IsConnected => network.isConnected;


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
     
        protected override void OnCreate()
        {
            base.OnCreate();

            replicateEntitySystemGroup = World.GetOrCreateSystem<ReplicateEntitySystemGroup>();

            EntityManager.CreateEntity(typeof(ServerSnapshot));
            SetSingleton(new ServerSnapshot
            {
                ServerTick = 0,
                TimeSinceSnapshot = 0,
                Rtt = 0,
                LastAcknowledgedTick = 0
            });

            network = new NetworkClient(GameWorld.Active);
        }

        protected override void OnDestroy()
        {
            network.Shutdown();
        }


        protected override void OnUpdate()
        {
            if (network.connectionState == NetworkClient.ConnectionState.Disconnected)
            {
                var ip = "58.247.94.202:7923";
               // var ip = "220.229.229.91";
                FSLog.Info($"network.Connect:{ip}");
                network.Connect(ip);
            }
        

            network.Update(this, replicateEntitySystemGroup);

            var snapshotFromServer = GetSingleton<ServerSnapshot>();
            snapshotFromServer.ServerTick = (uint) network.serverTime;
            snapshotFromServer.TimeSinceSnapshot = network.timeSinceSnapshot;
            snapshotFromServer.Rtt = network.rtt;
            snapshotFromServer.LastAcknowledgedTick = network.lastAcknowlegdedCommandTime;
            SetSingleton(snapshotFromServer);
        }

        public void QueueCommand(uint tick, NetworkClient.DataGenerator data)
        {
            network.QueueCommand((int) tick, data);
        }


        public void SendData()
        {
            network.SendData();
        }
    }
}