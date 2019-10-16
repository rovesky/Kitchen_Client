using FootStone.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public class NoSortComponentSystemGroup : ComponentSystemGroup
    {
        public override void SortSystemUpdateList()
        {

        }
    }

    [UnityEngine.ExecuteAlways]
    public class SpawnSystemGroup : NoSortComponentSystemGroup
    {
     
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnEntitiesClientSystem>());
        }
    }

    [UnityEngine.ExecuteAlways]
    public class DespawnSystemGroup : NoSortComponentSystemGroup
    {
     
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<DespawnSystem>());
        }
    }


    [UnityEngine.ExecuteAlways]
    public class PresentationSystemGroup : NoSortComponentSystemGroup
    {      

        protected override void OnCreate()
        {            
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ExlosionSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateHealthUISystem>());

        }
    }

    //[UnityEngine.ExecuteAlways]
    //public class MoveSystemGroup : NoSortComponentSystemGroup
    //{

    //    protected override void OnCreate()
    //    {
    //        m_systemsToUpdate.Add(World.GetOrCreateSystemE<MoveForwardSystem>());
    //        m_systemsToUpdate.Add(World.GetOrCreateSystemE<MoveSinSystem>());
    //        m_systemsToUpdate.Add(World.GetOrCreateSystemE<MovePositionSystem>());
    //        m_systemsToUpdate.Add(World.GetOrCreateSystemE<MoveTargetSystem>());
    //        m_systemsToUpdate.Add(World.GetOrCreateSystemE<MoveTranslationSystem>());
    //    }
    //}


    [UnityEngine.ExecuteAlways]
    public class PredictSystemGroup : NoSortComponentSystemGroup
    {

        protected override void OnCreate()
        { 
            m_systemsToUpdate.Add(World.GetOrCreateSystemE<MovePositionSystem>()); 
        }
    }

    [UnityEngine.ExecuteAlways]
    public class PredictClientSimulationSystemGroup : ComponentSystemGroup
    {
        private GameTick predictedTime = new GameTick(30);
        private GameTick renderTime = new GameTick(30);
        private double lastFrameTime = 0;
        private float frameTimeScale = 1.0f;
        private GameWorld gameWorld;
        private EntityQuery snapShotQuery;
        private uint serverTick;

        protected override void OnCreate()
        {
            FSLog.Info("PredictClientSimulationSystemGroup OnCreate");
            GameWorld.Active = new GameWorld();
            gameWorld = GameWorld.Active;

            snapShotQuery = GetEntityQuery(ComponentType.ReadWrite<SnapshotFromServer>());


            m_systemsToUpdate.Add(World.GetOrCreateSystem<InputSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<NetworkClientSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ReadSnapshotSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnSystemGroup>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<PredictSystemGroup>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<PresentationSystemGroup>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<DespawnSystemGroup>());            
        }

        public override void SortSystemUpdateList()
        {

        }

        private void HandleTime()
        {
            if (snapShotQuery.CalculateEntityCount() == 0)
                return;
            serverTick = snapShotQuery.GetSingleton<SnapshotFromServer>().tick;


            float frameDuration = lastFrameTime != 0 ? (float)(Game.frameTime - lastFrameTime) : 0;
            lastFrameTime = Game.frameTime;
            uint prevTick = predictedTime.Tick;

            // Increment time
            var deltaPredictedTime = frameDuration * frameTimeScale;
            predictedTime.AddDuration(deltaPredictedTime);

            // Adjust time to be synchronized with server
            uint preferredBufferedCommandCount = 2;
            
            uint preferredTick = serverTick + (uint)((120 / 1000.0f) * gameWorld.GameTick.TickRate) + preferredBufferedCommandCount;

            FSLog.Info($"serverTick:{serverTick},predictTick:{predictedTime.Tick}," +
                $"renderTick:{renderTime.Tick},preferredTick:{preferredTick}");

            bool resetTime = false;
            if (!resetTime && predictedTime.Tick < preferredTick - 3)
            {
                FSLog.Info($"Client predictTime hard catchup ... ");
                resetTime = true;
            }

            if (!resetTime && predictedTime.Tick > preferredTick + 6)
            {
                FSLog.Info($"Client predictTime hard slowdown ... ");
                resetTime = true;
            }

            frameTimeScale = 1.0f;
            if (resetTime)
            {
                FSLog.Info(string.Format("CATCHUP ({0} -> {1})",predictedTime.Tick, preferredTick));

               // m_NetworkStatistics.notifyHardCatchup = true;
              //  m_GameWorld.nextTickTime = Game.frameTime;
             //   predictedTime.Tick = preferredTick;
                predictedTime.SetTick(preferredTick, 0);

            }
            else
            {
                //int bufferedCommands = m_NetworkClient.lastAcknowlegdedCommandTime - serverTick;
                //if (bufferedCommands < preferredBufferedCommandCount)
                //    frameTimeScale = 1.01f;

                //if (bufferedCommands > preferredBufferedCommandCount)
                //    frameTimeScale = 0.99f;
            }

            // Increment interpolation time
            renderTime.AddDuration(frameDuration * frameTimeScale);

            // Force interp time to not exeede server time
            if (renderTime.Tick >= serverTick)
            {
                FSLog.Info($"Client renderTime hard slowdown ... ");
                renderTime.SetTick(serverTick, 0);
            }

            // hard catchup
            if (renderTime.Tick < serverTick - 10)
            {
                FSLog.Info($"Client renderTime hard catchup ... ");
                renderTime.SetTick(serverTick - 8, 0);
            }

            // Throttle up to catch up
            if (renderTime.Tick < serverTick - 1)
            {
               renderTime.AddDuration(frameDuration * 0.01f);
            }

            // If predicted time has entered a new tick the stored commands should be sent to server 
            if (predictedTime.Tick > prevTick)
            {
                var oldestCommandToSend = (int)Mathf.Max(prevTick, predictedTime.Tick - NetworkConfig.commandClientBufferSize);
                for (int tick = oldestCommandToSend; tick < predictedTime.Tick; tick++)
                {
                  //  m_PlayerModule.StoreCommand(tick);
               //     m_PlayerModule.SendCommand(tick);
                }

              //  m_PlayerModule.ResetInput(userInputEnabled);
              //  m_PlayerModule.StoreCommand(m_PredictedTime.tick);
            }

            // Store command
          //  m_PlayerModule.StoreCommand(m_PredictedTime.tick);
        }

        protected override void OnUpdate()
        {
            HandleTime();

            gameWorld.GameTick = renderTime; 

            World.GetExistingSystem<InputSystem>().Update();
            World.GetExistingSystem<NetworkClientSystem>().Update();
            World.GetExistingSystem<ReadSnapshotSystem>().Update();
            World.GetExistingSystem<SpawnSystemGroup>().Update();

            gameWorld.GameTick = predictedTime;

            if (IsPredictionAllowed())
            {
                // ROLLBACK. All predicted entities (with the ServerEntity component) are rolled back to last server state 
                gameWorld.GameTick.SetTick(serverTick, predictedTime.TickInterval);
                PredictionRollback();


                // PREDICT PREVIOUS TICKS. Replay every tick *after* the last tick we have from server up to the last stored command we have
                for (var tick = serverTick + 1; tick <predictedTime.Tick; tick++)
                {
                    gameWorld.GameTick.SetTick(tick, predictedTime.TickInterval);
                  //  m_PlayerModule.RetrieveCommand(gameWorld.Tick);
                    PredictionUpdate();
                }

                // PREDICT CURRENT TICK. Update current tick using duration of current tick
                gameWorld.GameTick = predictedTime;
           //     m_PlayerModule.RetrieveCommand(gameWorld.Tick);
                // Dont update systems with close to zero time. 
                if (gameWorld.TickDuration > 0.008f)
                {
                    PredictionUpdate();
                }            
            }

            World.GetExistingSystem<PresentationSystemGroup>().Update();

            gameWorld.GameTick = renderTime;
            World.GetExistingSystem<DespawnSystemGroup>().Update();
        }

        private void PredictionUpdate()
        {
        //    World.GetExistingSystem<MovePositionSystem>().Update();
        }

        private void PredictionRollback()
        {
          
        }

        private bool IsPredictionAllowed()
        {
            return true;
        }
    }

 
}
