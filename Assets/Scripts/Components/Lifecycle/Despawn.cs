using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct Despawn : IComponentData
    {
        public int Frame;
    }
}