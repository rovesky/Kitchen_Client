using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct LifeTime : IComponentData
    {
        public float Value;
    }

    [DisallowMultipleComponent]
    public class LifeTimeProxy : ComponentDataProxy<LifeTime> { }

}
