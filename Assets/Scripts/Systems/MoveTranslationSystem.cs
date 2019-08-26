using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public class MoveTranslationSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            float dt = Time.fixedDeltaTime;

            Entities.ForEach(
                (ref Translation position, ref MoveTranslation move) =>
                {
                    var value = position.Value;
                    if (move.Direction == Direction.Up)
                    {
                        value.z += move.Speed * Time.deltaTime;
                    }
                    else if (move.Direction == Direction.Down)
                    {
                        value.z -= move.Speed * Time.deltaTime;
                    }
                    else if (move.Direction == Direction.Left)
                    {
                        value.x -= move.Speed * Time.deltaTime;
                    }
                    else if (move.Direction == Direction.Right)
                    {
                        value.x += move.Speed * Time.deltaTime;
                    }

                    position = new Translation()
                    {
                        Value = value
                    };

                }
            );
        }
    }

}
