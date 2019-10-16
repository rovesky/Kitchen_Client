using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{
    
    [Serializable]
    public struct PlayerPredictData : IComponentData
    {
       public  float3 pos;
    }

}