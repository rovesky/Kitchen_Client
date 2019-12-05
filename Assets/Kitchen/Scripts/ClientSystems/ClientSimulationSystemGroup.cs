using FootStone.ECS;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SetRenderTimeSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var worldTime = GetSingleton<WorldTime>();
            worldTime.GameTick = GetSingleton<ClientTickTime>().Render;
            SetSingleton(worldTime);
        }
    }


    [DisableAutoCreation]
    public class SetPredictTimeSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var worldTime = GetSingleton<WorldTime>();
            worldTime.GameTick = GetSingleton<ClientTickTime>().Predict;
            SetSingleton(worldTime);
        }
    }


    //[DisableAutoCreation]
    [UpdateAfter(typeof(ExportPhysicsWorld))]
    [UpdateBefore(typeof(EndFramePhysicsSystem))]
    public class ClientSimulationSystemGroup : NoSortComponentSystemGroup
    {
        private DespawnClientSystemGroup despawnSystemGroup;
        private HandleTimeSystem handleTimeSystem;
        private NetworkClientSystem networkSystem;
        private PredictSystem predictSystem;
        private PresentationSystemGroup presentationSystemGroup;
        private ReplicateEntitySystemGroup replicateEntitySystemGroup;
        private SetPredictTimeSystem setPredictTimeSystem;
        private SetRenderTimeSystem setRenderTimeSystem;
        private SpawnClientSystemGroup spawnSystemGroup;

        protected override void OnCreate()
        {
            FSLog.Info("KitchenClientSimulationSystemGroup OnCreate");
            Application.targetFrameRate = 30;
            ConfigVar.Init();
            GameWorld.Active = new GameWorld();


            networkSystem = World.GetOrCreateSystem<NetworkClientSystem>();
            m_systemsToUpdate.Add(networkSystem);

            handleTimeSystem = World.GetOrCreateSystem<HandleTimeSystem>();
            m_systemsToUpdate.Add(handleTimeSystem);

            replicateEntitySystemGroup = World.GetOrCreateSystem<ReplicateEntitySystemGroup>();
            m_systemsToUpdate.Add(replicateEntitySystemGroup);

            setRenderTimeSystem = World.GetOrCreateSystem<SetRenderTimeSystem>();
            m_systemsToUpdate.Add(setRenderTimeSystem);

            spawnSystemGroup = World.GetOrCreateSystem<SpawnClientSystemGroup>();
            m_systemsToUpdate.Add(spawnSystemGroup);

            setPredictTimeSystem = World.GetOrCreateSystem<SetPredictTimeSystem>();
            m_systemsToUpdate.Add(setPredictTimeSystem);

            predictSystem = World.GetOrCreateSystem<PredictSystem>();
            m_systemsToUpdate.Add(predictSystem);

            presentationSystemGroup = World.GetOrCreateSystem<PresentationSystemGroup>();
            m_systemsToUpdate.Add(presentationSystemGroup);

            despawnSystemGroup = World.GetOrCreateSystem<DespawnClientSystemGroup>();
            m_systemsToUpdate.Add(despawnSystemGroup);
        }


        protected override void OnUpdate()
        {
            networkSystem.Update();

            if (!networkSystem.IsConnected)
                return;

            handleTimeSystem.Update();

            setRenderTimeSystem.Update();

            replicateEntitySystemGroup.Update();

            spawnSystemGroup.Update();

            setPredictTimeSystem.Update();

            predictSystem.Update();

            presentationSystemGroup.Update();

            setRenderTimeSystem.Update();

            despawnSystemGroup.Update();

            networkSystem.SendData();
        }
    }
}