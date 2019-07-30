using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct EntityKiller : IComponentData
    {
        public int TimeToDie;
    }
   
}