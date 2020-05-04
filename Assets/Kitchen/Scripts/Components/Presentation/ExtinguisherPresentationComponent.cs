using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class ExtinguisherPresentation : IComponentData
    {
        public GameObject Smog;
        public float3 SmogPos;

    }
   
}