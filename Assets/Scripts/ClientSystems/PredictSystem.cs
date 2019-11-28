﻿using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
       

    [DisableAutoCreation]
    public class PredictSystem :ComponentSystem
    {
        private PredictUpdateSystemGroup predictUpdateSystemGroup;
        private InputSystem inputSystem;

        private TickStateDenseBuffer<CharacterPredictedState> commandBuffer = new TickStateDenseBuffer<CharacterPredictedState>(128);

        protected override void OnCreate()
        {    
            predictUpdateSystemGroup = World.GetOrCreateSystem<PredictUpdateSystemGroup>();

            inputSystem = World.GetOrCreateSystem<InputSystem>();
        }

        protected override void OnUpdate()
        {
            var serverTick = GetSingleton<ServerSnapshot>().tick;
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
                    PredictionUpdate(worldTime.Tick);
                }

                // PREDICT CURRENT TICK. Update current tick using duration of current tick
                worldTime.GameTick = predictTime;
                SetSingleton(worldTime);
                //     m_PlayerModule.RetrieveCommand(gameWorld.Tick);
                // Dont update systems with close to zero time. 
                if (worldTime.TickDuration > 0.008f)
                {
                    PredictionUpdate(predictTime.Tick);
                }
            }
        }

        private void PredictionUpdate(uint tick)
        {
            predictUpdateSystemGroup.Update();

            //保存预测数据
         //   var localPlayerQ = GetEntityQuery(typeof(LocalPlayer));
         //   if (localPlayerQ.CalculateEntityCount() != 1)
         //       return;

         //   var entity = localPlayerQ.GetSingletonEntity();
         //   var predictData = EntityManager.GetComponentData<EntityPredictData>(entity);

         ////   FSLog.Info($"<{tick}>PredictionUpdate:[{predictData.Position.x},{predictData.Position.y},{predictData.Position.z}]");

         //   var lastBufferTick = commandBuffer.LastTick();
         //   if (tick != lastBufferTick && tick != lastBufferTick + 1 && lastBufferTick!= -1)
         //   {
         //       commandBuffer.Clear();
         //       FSLog.Warning(string.Format("Trying to store tick:{0} but last predictData tick is:{1}. Clearing buffer", tick, lastBufferTick));
         //   }
      
         //   if (tick == lastBufferTick)
         //       commandBuffer.Set(ref predictData, (int)tick);
         //   else
         //       commandBuffer.Add(ref predictData, (int)tick);          
        }

        private void PredictionRollback()
        {
            Entities.ForEach((Entity entity, ref CharacterPredictedState predicData, ref EntityPredictDataSnapshot snapshotData) =>
            {
                predicData.Position = snapshotData.position;
                predicData.Rotation = snapshotData.rotation;
                predicData.PickupedEntity = snapshotData.pickupEntity;

            });

            //var localPlayerQ = GetEntityQuery(typeof(LocalPlayer));
            //if (localPlayerQ.CalculateEntityCount() != 1)
            //    return;

            //var snapshot = GetSingleton<SnapshotFromServer>();
            //var entity = localPlayerQ.GetSingletonEntity();
            //EntityManager.SetComponentData(entity, snapshot.predictData);


            //EntityPredictData predictData = default;
            //if (commandBuffer.TryGetValue((int)snapshot.tick, ref predictData))
            //{
            //    var lastServerData = snapshot.predictData;

            //    if (!lastServerData.Equals(predictData))
            //    {
            //        FSLog.Warning($"<{snapshot.tick}>lastServerData:[{lastServerData.Position.x},{lastServerData.Position.y},{lastServerData.Position.z}]");
            //        FSLog.Warning($"<{snapshot.tick}>   predictData:[{predictData.Position.x},{predictData.Position.y},{predictData.Position.z}]");
            //    }
            //}

          //  commandBuffer.Clear();
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
