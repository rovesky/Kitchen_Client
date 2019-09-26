using System;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct MoveMouse : IComponentData
    {
        public float Speed;
       
    }     

}