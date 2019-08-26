﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct MoveRotation : IComponentData
    {
    //    public bool IsInited;
        public float Speed;      
    }

}