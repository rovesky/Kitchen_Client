using FootStone.ECS;
using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{

    public struct EntityInterpolate :IComponentData
    {
        public int id;
     //   public TickStateSparseBufferStruct<EntityPredictData> buffer;
    }
}