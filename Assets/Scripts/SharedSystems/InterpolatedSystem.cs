using FootStone.ECS;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class InterpolatedSystem : FSComponentSystem
    {

        private Dictionary<int, TickStateSparseBuffer<EntityPredictData>> interpolateDataMap = 
            new Dictionary<int, TickStateSparseBuffer<EntityPredictData>>();


        public void AddData(int serverTick,int id,ref EntityPredictData data)
        {           
            if (!interpolateDataMap.ContainsKey(id))         
                interpolateDataMap.Add(id, new TickStateSparseBuffer<EntityPredictData>(64));         

            var buffer = interpolateDataMap[id];
            buffer.Add(serverTick, data);

            FSLog.Info($"AddData serverTick:{serverTick},id:{id}");
        }

        protected override void OnCreate()
        {          
           
        }



        protected override void OnUpdate()
        {
        //    FSLog.Info("InterpolatedSystem Update1");
            var worldTime = GetSingleton<WorldTime>();
            Entities.WithAllReadOnly<EntityInterpolate>().ForEach((Entity entity,ref Player player, ref EntityPredictData predictData) =>
            {
              //  FSLog.Info($"InterpolatedSystem Update2:{player.id}");
                if (!interpolateDataMap.ContainsKey(player.id))
                    return;
             
                var buffer = interpolateDataMap[player.id];
                if (buffer.Count <= 0)
                    return;

              //  FSLog.Info("InterpolatedSystem Update");

                int lowIndex = 0, highIndex = 0;
                float interpVal = 0;
                var interpValid = buffer.GetStates((int)worldTime.Tick, worldTime.TickDurationAsFraction, ref lowIndex, ref highIndex, ref interpVal);

                if (interpValid)
                {
                    var prevState = buffer[lowIndex];
                    var nextState = buffer[highIndex];
                    predictData.Interpolate(ref prevState, ref nextState, interpVal);
                }
                else
                {
                    predictData = buffer.Last();
                }
            });

        }
    }
}
