﻿using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [ExecuteAlways]
    [DisableAutoCreation]
    public class PredictUpdateSystemGroup : NoSortComponentSystemGroup
    {

        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystemE<MovePositionSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveSinSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveTargetSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveForwardSystem>());
            //   m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveTranslationSystem>());
        }
    }

    [ExecuteAlways]
    [DisableAutoCreation]
    public class PredictSystem : FSComponentSystem
    {
        private PredictUpdateSystemGroup predictUpdateSystemGroup;
        private InputSystem inputSystem;

        protected override void OnCreate()
        {    
            predictUpdateSystemGroup = World.GetOrCreateSystem<PredictUpdateSystemGroup>();

            inputSystem = World.GetOrCreateSystem<InputSystem>();
        }

        protected override void OnUpdate()
        {
            var serverTick = GetSingleton<SnapshotFromServer>().tick;
            var clientTick = GetSingleton<ClientTickTime>();
            var worldTime = GetSingleton<WorldTime>();
            
            var predictTime = clientTick.predict;
            var renderTime = clientTick.render;

            if (IsPredictionAllowed(predictTime,serverTick))
            {
                // ROLLBACK. All predicted entities (with the ServerEntity component) are rolled back to last server state 
                worldTime.SetTick(serverTick, predictTime.TickInterval);
                SetSingleton(worldTime);

                PredictionRollback();

                // PREDICT PREVIOUS TICKS. Replay every tick *after* the last tick we have from server up to the last stored command we have
                for (var tick = serverTick + 1; tick < predictTime.Tick; tick++)
                {
                    worldTime.SetTick(tick, predictTime.TickInterval);
                    SetSingleton(worldTime);

                    inputSystem.RetrieveCommand(worldTime.Tick);
                    PredictionUpdate();
                }

                // PREDICT CURRENT TICK. Update current tick using duration of current tick
                worldTime.SetTick(predictTime);
                SetSingleton(worldTime);
                //     m_PlayerModule.RetrieveCommand(gameWorld.Tick);
                // Dont update systems with close to zero time. 
                if (worldTime.TickDuration > 0.008f)
                {
                    PredictionUpdate();
                }
            }
        }

        private void PredictionUpdate()
        {
            predictUpdateSystemGroup.Update();
        }

        private void PredictionRollback()
        {

        }

        private bool IsPredictionAllowed(GameTick predictTime,uint serverTick)
        {
            if (predictTime.Tick <= serverTick)
            {
                FSLog.Warning("No predict! Predict time not ahead of server tick! ");
                return false;
            }

            if (!inputSystem.HasCommands(serverTick + 1, predictTime.Tick))
            {
                FSLog.Warning("No predict! No commands available. " );
                return false;
            }

            return true;
        }
    }
}
