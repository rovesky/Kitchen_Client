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
    public class MoveSinSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {            
            Entities.WithAllReadOnly<MoveSin>().ForEach((ref Translation translation) =>
            {
                // 左右移动
                float rx = Mathf.Sin(Time.time) * Time.deltaTime;
                translation = new Translation()
                {
                    Value = new float3(translation.Value.x + rx,
                                       translation.Value.y,
                                       translation.Value.z)
                };

            });
        }
    }

}
