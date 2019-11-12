﻿using System;
using FootStone.ECS;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace Assets.Scripts.ECS
{


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

            //float remainTime = (float)(nextTickTime - worldTime.frameTime);

            //int rate = worldTime.gameTick.TickRate;
            //if (remainTime > 0.75f * worldTime.gameTick.TickInterval)
            //    rate -= 2;
            //else if (remainTime < 0.25f * worldTime.gameTick.TickInterval)
            //    rate += 2;

            //Application.targetFrameRate = rate;
        }

        private void PrevierTickUpdate()
        {
            var worldTime = GetSingleton<WorldTime>();
            worldTime.gameTick = gameTime;
  
            inputSystem.StoreCommand(worldTime.Tick);
      
            spawnSystemGroup.Update();
     
            inputSystem.RetrieveCommand(worldTime.Tick);         
            predictUpdateSystem.Update();

            presentationSystemGroup.Update();

            despawnSystemGroup.Update();

            SetSingleton(worldTime);
        }
    }
}
