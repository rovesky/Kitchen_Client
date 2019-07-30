using System;
using System.Threading;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct Health : IComponentData
    {
        public int Value;
    }
}