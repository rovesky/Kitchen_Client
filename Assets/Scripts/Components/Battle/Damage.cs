using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct Damage : IComponentData
    {
        public int Value;
    }
}