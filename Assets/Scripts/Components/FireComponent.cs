using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct FireRocket : IComponentData
    {
        public Entity Rocket;    

        public float  FireCooldown;

        public float RocketTimer;
    }    
   
}