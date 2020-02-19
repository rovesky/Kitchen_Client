using FootStone.ECS;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    //   [UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
    public class PreviewClientSimulationSystemGroup : NoSortComponentSystemGroup
    {
        private GameTick gameTime = GameTick.DefaultGameTick;
        private double nextTickTime;

        private SpawnPreviewClientSystem spawnSystemGroup;
        private PredictUpdateSystemGroup predictUpdateSystem;
        private PredictPresentationSystemGroup predictPresentationSystemGroup;
        private DespawnClientSystemGroup despawnSystemGroup;
        private InputSystem inputSystem;
        private SpawnPlatesSystem spawnPlatesSystem;
        private UpdateReplicatedOwnerFlag updateReplicatedOwnerFlag;

        protected override void OnCreate()
        {
            FSLog.Info("PreviewClientSimulationSystemGroup OnCreate");
            Application.targetFrameRate = 30;
            UnityEngine.Time.fixedDeltaTime = gameTime.TickInterval;
            ConfigVar.Init();
            GameWorld.Active = new GameWorld();

            //  World.DestroySystem(World.GetExistingSystem<ExportPhysicsWorld>());

            inputSystem = World.GetOrCreateSystem<InputSystem>();
            m_systemsToUpdate.Add(inputSystem);

            spawnSystemGroup = World.GetOrCreateSystem<SpawnPreviewClientSystem>();
            m_systemsToUpdate.Add(spawnSystemGroup);

            spawnPlatesSystem = World.GetOrCreateSystem<SpawnPlatesSystem>();
            m_systemsToUpdate.Add(spawnPlatesSystem);

            updateReplicatedOwnerFlag = World.GetOrCreateSystem<UpdateReplicatedOwnerFlag>();
            m_systemsToUpdate.Add(updateReplicatedOwnerFlag);

            predictUpdateSystem = World.GetOrCreateSystem<PredictUpdateSystemGroup>();
            m_systemsToUpdate.Add(predictUpdateSystem);

            predictPresentationSystemGroup = World.GetOrCreateSystem<PredictPresentationSystemGroup>();
            m_systemsToUpdate.Add(predictPresentationSystemGroup);

            despawnSystemGroup = World.GetOrCreateSystem<DespawnClientSystemGroup>();
            m_systemsToUpdate.Add(despawnSystemGroup);

            updateReplicatedOwnerFlag.SetLocalPlayerId(0);
        }

        protected override void OnUpdate()
        {
            var worldTime = GetSingleton<WorldTime>();
            inputSystem.SampleInput(worldTime.Tick);

            bool commandWasConsumed = false;
            while (worldTime.FrameTime > nextTickTime)
            {
                gameTime.Tick++;
                gameTime.TickDuration = gameTime.TickInterval;

                commandWasConsumed = true;
                PreviewTickUpdate();
                nextTickTime += worldTime.GameTick.TickInterval;
            }

            if (commandWasConsumed)
                inputSystem.ResetInput();
        }

        private void PreviewTickUpdate()
        {
            var worldTime = GetSingleton<WorldTime>();
            worldTime.GameTick = gameTime;

            inputSystem.StoreCommand(worldTime.Tick);

            spawnSystemGroup.Update();

            spawnPlatesSystem.Update();

            updateReplicatedOwnerFlag.Update();

           // for (var i = 0; i < 1; ++i)
            {
                inputSystem.RetrieveCommand(worldTime.Tick);

                predictUpdateSystem.Update();
            }


            predictPresentationSystemGroup.Update();

            despawnSystemGroup.Update();

            SetSingleton(worldTime);
        }
    }
}
