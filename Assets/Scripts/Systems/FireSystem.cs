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
                (ref LocalToWorld gunTransform, ref Rotation gunRotation, ref FireRocket fire) =>
                {
                    if (fire.rocket == null)
                        return;

                    fire.rocketTimer -= Time.deltaTime;
                    if (fire.rocketTimer > 0)
                        return;

                    fire.rocketTimer = fire.minRocketTimer;

                    if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
                    {
                        var e = PostUpdateCommands.Instantiate(fire.rocket);

                        Translation position = new Translation() { Value = gunTransform.Position };
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
                    if (fire.rocket == null)
                        return;

                    fire.rocketTimer -= Time.deltaTime;
                    if (fire.rocketTimer > 0)
                        return;

                    fire.rocketTimer = fire.minRocketTimer;

                    var e = PostUpdateCommands.Instantiate(fire.rocket);

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
