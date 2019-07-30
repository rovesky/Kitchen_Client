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
    public class MoveSinSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            // Debug.Log("MoveSpeedSystem OnUpdate");
            // Entities.ForEach processes each set of ComponentData on the main thread. This is not the recommended
            // method for best performance. However, we start with it here to demonstrate the clearer separation
            // between ComponentSystem Update (logic) and ComponentData (data).
            // There is no update logic on the individual ComponentData.
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
