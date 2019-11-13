using System;
using FootStone.ECS;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    [UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
    public class PreviewClientSimulationSystemGroup : NoSortComponentSystemGroup
    {
        private GameTick gameTime = GameTick.defaultGameTick;
        private double nextTickTime = 0;

        private SpawnPreviewClientSystem spawnSystemGroup; 
        private PredictUpdateSystemGroup predictUpdateSystem;
        private PresentationSystemGroup presentationSystemGroup;
        private DespawnSystemGroup despawnSystemGroup;
        private InputSystem inputSystem;
        private SpawnPlatesSystem spawnPlatesSystem;

        protected override void OnCreate()
        {
            FSLog.Info("PreviewClientSimulationSystemGroup OnCreate");
       //     Application.targetFrameRate = 30;
            ConfigVar.Init();
            GameWorld.Active = new GameWorld();

            inputSystem = World.GetOrCreateSystem<InputSystem>();
            m_systemsToUpdate.Add(inputSystem);

            spawnSystemGroup = World.GetOrCreateSystem<SpawnPreviewClientSystem>();
            m_systemsToUpdate.Add(spawnSystemGroup);

            spawnPlatesSystem = World.GetOrCreateSystem<SpawnPlatesSystem>();
            m_systemsToUpdate.Add(spawnPlatesSystem);

            

            predictUpdateSystem = World.GetOrCreateSystem<PredictUpdateSystemGroup>();
            m_systemsToUpdate.Add(predictUpdateSystem);

            presentationSystemGroup = World.GetOrCreateSystem<PresentationSystemGroup>();
            m_systemsToUpdate.Add(presentationSystemGroup);

            despawnSystemGroup = World.GetOrCreateSystem<DespawnSystemGroup>();
            m_systemsToUpdate.Add(despawnSystemGroup);
        }

        protected override void OnUpdate()
        {          
            var worldTime = GetSingleton<WorldTime>();
            inputSystem.SampleInput(worldTime.Tick);

            bool commandWasConsumed = false;
            while (worldTime.frameTime > nextTickTime)
            {
                gameTime.Tick++;
                gameTime.TickDuration = gameTime.TickInterval;
        
                commandWasConsumed = true;
                PrevierTickUpdate();
                nextTickTime += worldTime.gameTick.TickInterval;
            }
            if (commandWasConsumed)
                inputSystem.ResetInput();      
        }

        private void PrevierTickUpdate()
        {
            var worldTime = GetSingleton<WorldTime>();
            worldTime.gameTick = gameTime;
  
            inputSystem.StoreCommand(worldTime.Tick);
      
            spawnSystemGroup.Update();
            spawnPlatesSystem.Update();


            inputSystem.RetrieveCommand(worldTime.Tick);     
            
            predictUpdateSystem.Update();

            presentationSystemGroup.Update();

            despawnSystemGroup.Update();

            SetSingleton(worldTime);
        }
    }
}
