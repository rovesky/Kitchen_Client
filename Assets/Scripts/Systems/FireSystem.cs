using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public class PlayerFireSystem : ComponentSystem
    {
     
        private GameObject rocketPrefab;

        protected override void OnCreate()
        {
            rocketPrefab = Resources.Load("Prefabs/Rocket") as GameObject;
        }

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Player>().ForEach(
                (ref Translation gunTransform, ref Rotation gunRotation, ref FireRocket fire) =>
                {
                    //if (fire.Rocket == null)
                    //    return;

                    fire.RocketTimer -= Time.deltaTime;
                    if (fire.RocketTimer > 0)
                        return;

                    fire.RocketTimer = fire.FireCooldown;

                    if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
                    {
                        var go = Object.Instantiate(rocketPrefab);
                        var e = go.GetComponent<EntityTracker>().EntityToTrack;
                      //  var e = PostUpdateCommands.Instantiate(fire.Rocket);

                        Translation position = new Translation() { Value = gunTransform.Value };
                        Rotation rotation = new Rotation() { Value = gunRotation.Value };

                        PostUpdateCommands.SetComponent(e, position);
                        PostUpdateCommands.SetComponent(e, rotation);

                        PostUpdateCommands.AddComponent(e, new TriggerDestroy());

                        PostUpdateCommands.AddComponent(e, new Rocket() {Type = RocketType.Player} );
                        PostUpdateCommands.AddComponent(e, new Attack() { Power = 20 });
                        PostUpdateCommands.AddComponent(e, new MoveTranslation() { Speed = 5,Direction = Direction.Up });
                        PostUpdateCommands.AddComponent(e, new KillOutofRender() { IsVisible = true });
                    }
                }
            );
        }
    }


    public class EnemyFireSystem : ComponentSystem
    {
        private GameObject rocketPrefab;

        protected override void OnCreate()
        {
            rocketPrefab = Resources.Load("Prefabs/Rocket1") as GameObject;
        }

        protected override void OnUpdate()
        {

            Entities.WithAllReadOnly<Enemy>().ForEach(
                (ref LocalToWorld gunTransform, ref Rotation gunRotation, ref FireRocket fire) =>
                {
                   // if (fire.Rocket == null)
                      //  return;

                    fire.RocketTimer -= Time.deltaTime;
                    if (fire.RocketTimer > 0)
                        return;

                    fire.RocketTimer = fire.FireCooldown;

                    Debug.Log("Enemy Fire");

                    var go = Object.Instantiate(rocketPrefab);
                    var e = go.GetComponent<EntityTracker>().EntityToTrack;
                   // var e = PostUpdateCommands.Instantiate(fire.Rocket);

                    Translation position = new Translation() { Value = gunTransform.Position };
                    Rotation rotation = new Rotation() { Value = gunRotation.Value };

                    PostUpdateCommands.SetComponent(e, position);
                    PostUpdateCommands.SetComponent(e, rotation);

                    PostUpdateCommands.AddComponent(e, new Rocket(){Type = RocketType.Enemy});
                    PostUpdateCommands.AddComponent(e, new TriggerDestroy());
                    PostUpdateCommands.AddComponent(e, new Attack() { Power = 1 });
                    PostUpdateCommands.AddComponent(e, new MoveTarget() { Speed = 3});
                  //  PostUpdateCommands.AddComponent(e, new EntityKiller() { TimeToDie = 100 });
                }
            );
        }
    }
}
