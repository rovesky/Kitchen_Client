//using FootStone.ECS;
//using System.Collections.Generic;
//using Unity.Entities;
//using Unity.Transforms;
//using UnityEngine;

//namespace Assets.Scripts.ECS
//{

//    [DisableAutoCreation]
//    public class InterpolatedSystem : ComponentSystem
//    {
//        private Dictionary<int, TickStateSparseBuffer<CharacterInterpolatedState>> interpolateDataMap =
//            new Dictionary<int, TickStateSparseBuffer<CharacterInterpolatedState>>();

//        public void AddData(int serverTick, int id, ref CharacterInterpolatedState data)
//        {
//            if (!interpolateDataMap.ContainsKey(id))
//                interpolateDataMap.Add(id, new TickStateSparseBuffer<CharacterInterpolatedState>(64));

//            var buffer = interpolateDataMap[id];
//            buffer.Add(serverTick, data);
//            //  FSLog.Info($"AddData serverTick:{serverTick},id:{id}");
//        }

//        protected override void OnCreate()
//        {

//        }

//        protected override void OnUpdate()
//        {        
//            var worldTime = GetSingleton<WorldTime>();
//            Entities/*.WithAllReadOnly<EntityInterpolate>()*/.ForEach((Entity entity, ref EntityInterpolate interpolate, ref CharacterInterpolatedState predictData) =>
//            {
//              //  FSLog.Info($"InterpolatedSystem Update1:{interpolate.id}");
//                if (!interpolateDataMap.ContainsKey(interpolate.id))
//                    return;

//                var buffer = interpolateDataMap[interpolate.id];
//                if (buffer.Count <= 0)
//                    return;    

//                int lowIndex = 0, highIndex = 0;
//                float interpVal = 0;
//                var interpValid = buffer.GetStates((int)worldTime.Tick, worldTime.TickDurationAsFraction, ref lowIndex, ref highIndex, ref interpVal);

//            //    FSLog.Info($"InterpolatedSystem Update2:{interpolate.id}");
//                if (interpValid)
//                {
//                    var prevState = buffer[lowIndex];
//                    var nextState = buffer[highIndex];
//                    predictData.Interpolate(ref prevState, ref nextState, interpVal);
//                }
//                else
//                {
//                    predictData = buffer.Last();
//                }
//            });

//        }
//    }
//}
