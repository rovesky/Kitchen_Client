using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [UpdateInGroup(typeof(TransformSystemGroup))]
    public class MoveForwardSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {

            Entities.ForEach((ref LocalToWorld lw, ref Translation position, ref MoveForward move) =>
            {
                position = new Translation()
                {
                    Value = position.Value - lw.Forward * move.Speed * Time.deltaTime
                };
            });

        }
    }
}
