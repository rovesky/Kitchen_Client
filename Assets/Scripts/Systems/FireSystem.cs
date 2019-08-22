using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public class PlayerFireSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Player>().ForEach(
                (ref Translation gunTransform, ref Rotation gunRotation, ref FireRocket fire) =>
                {
                    if (fire.Rocket == null)
                        return;

                    fire.RocketTimer -= Time.deltaTime;
                    if (fire.RocketTimer > 0)
                        return;

                    fire.RocketTimer = fire.FireCooldown;

                    if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
                    {
                        var e = PostUpdateCommands.Instantiate(fire.Rocket);

                        Translation position = new Translation() { Value = gunTransform.Value };
                        Rotation rotation = new Rotation() { Value = gunRotation.Value };

                        PostUpdateCommands.SetComponent(e, position);
                        PostUpdateCommands.SetComponent(e, rotation);

                        PostUpdateCommands.AddComponent(e, new TriggerDestroy());
                        PostUpdateCommands.AddComponent(e, new Attack() { Power = 20 });
                        PostUpdateCommands.AddComponent(e, new MoveTranslation() { Speed = 5,Direction = Direction.Up });
                        PostUpdateCommands.AddComponent(e, new EntityKiller() { TimeToDie = 50 });

                    }
                }
            );
        }
    }


    public class EnemyFireSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {

            Entities.WithAllReadOnly<Enemy>().ForEach(
                (ref LocalToWorld gunTransform, ref Rotation gunRotation, ref FireRocket fire) =>
                {
                    if (fire.Rocket == null)
                        return;

                    fire.RocketTimer -= Time.deltaTime;
                    if (fire.RocketTimer > 0)
                        return;

                    fire.RocketTimer = fire.FireCooldown;

                    var e = PostUpdateCommands.Instantiate(fire.Rocket);

                    Translation position = new Translation() { Value = gunTransform.Position };
                    Rotation rotation = new Rotation() { Value = gunRotation.Value };

                    PostUpdateCommands.SetComponent(e, position);
                    PostUpdateCommands.SetComponent(e, rotation);

                    PostUpdateCommands.AddComponent(e, new TriggerDestroy());
                    PostUpdateCommands.AddComponent(e, new Attack() { Power = 1 });
                    PostUpdateCommands.AddComponent(e, new MoveTarget() { Speed = 3});
                    PostUpdateCommands.AddComponent(e, new EntityKiller() { TimeToDie = 100 });
                }
            );
        }
    }
}
