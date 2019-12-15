using FootStone.ECS;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    [UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
    public class PreviewClientSimulationSystemGroup : NoSortComponentSystemGroup
    {
        private GameTick gameTime = GameTick.DefaultGameTick;
        private double nextTickTime ;

        private SpawnPreviewClientSystem spawnSystemGroup; 
        private PredictUpdateSystemGroup predictUpdateSystem;
        private PredictPresentationSystemGroup latePresentationSystemGroup;
        private DespawnClientSystemGroup despawnSystemGroup;
        private InputSystem inputSystem;
        private SpawnPlatesSystem spawnPlatesSystem;
        private UpdateReplicatedOwnerFlag updateReplicatedOwnerFlag;

        protected override void OnCreate()
        {
            FSLog.Info("PreviewClientSimulationSystemGroup OnCreate");
            Application.targetFrameRate = 60;
            ConfigVar.Init();
            GameWorld.Active = new GameWorld();

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

            latePresentationSystemGroup = World.GetOrCreateSystem<PredictPresentationSystemGroup>();
            m_systemsToUpdate.Add(latePresentationSystemGroup);

            despawnSystemGroup = World.GetOrCreateSystem<DespawnClientSystemGroup>();
            m_systemsToUpdate.Add(despawnSystemGroup);
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
                PrevierTickUpdate();
                nextTickTime += worldTime.GameTick.TickInterval;
            }
            if (commandWasConsumed)
                inputSystem.ResetInput();      
        }

        private void PrevierTickUpdate()
        {
            var worldTime = GetSingleton<WorldTime>();
            worldTime.GameTick = gameTime;
  
            inputSystem.StoreCommand(worldTime.Tick);
      
            spawnSystemGroup.Update();

            spawnPlatesSystem.Update();

            updateReplicatedOwnerFlag.Update();

            inputSystem.RetrieveCommand(worldTime.Tick);     
            
            predictUpdateSystem.Update();

         //   throwSystem.Update();

            latePresentationSystemGroup.Update();

            despawnSystemGroup.Update();

            SetSingleton(worldTime);
        }
    }
}
