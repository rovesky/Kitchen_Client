using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct SpawnPlayer : IComponentData
    {
        public Entity entity;
    }

}