using FootStone.ECS;
using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{

    public struct EntityInterpolate :IComponentData
    {
        public int id;
     //   public TickStateSparseBufferStruct<EntityPredictData> buffer;
    }
}