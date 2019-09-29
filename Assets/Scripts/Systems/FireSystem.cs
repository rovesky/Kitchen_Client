using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [DisableAutoCreation]
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
                (ref Translation gunTransform,ref PlayerCommand command, ref Rotation gunRotation, ref FireRocket fire) =>
                {
              
                    fire.RocketTimer -= Time.deltaTime;
                    if (fire.RocketTimer > 0)
                        return;

                    fire.RocketTimer = fire.FireCooldown;

                    if (command.isBack && command.buttons.IsSet(PlayerCommand.Button.PrimaryFire))
                    {
                        var go = Object.Instantiate(rocketPrefab);
                        var e = go.GetComponent<EntityTracker>().EntityToTrack;
                        //  var e = PostUpdateCommands.Instantiate(fire.Rocket);

                        Translation position = new Translation() {Value = gunTransform.Value};
                        Rotation rotation = new Rotation() {Value = gunRotation.Value};

                        PostUpdateCommands.SetComponent(e, position);
                        PostUpdateCommands.SetComponent(e, rotation);

                        PostUpdateCommands.AddComponent(e, new Rocket() { Type = RocketType.Player });
                        PostUpdateCommands.AddComponent(e, new Health(){Value = 1});
                        PostUpdateCommands.AddComponent(e, new Damage() );
                        PostUpdateCommands.AddComponent(e, new Attack() {Power = 20});
                        PostUpdateCommands.AddComponent(e, new MoveTranslation() {Speed = 6, Direction = Direction.Up});
              
                    }
                }
            );
        }
    }

    [DisableAutoCreation]
    public class EnemyFireSystem : ComponentSystem
    {
        private GameObject rocketPrefab;
        private EntityQuery playerGroup;

        protected override void OnCreate()
        {
            rocketPrefab = Resources.Load("Prefabs/Rocket1") as GameObject;
            playerGroup = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(Player),
                }
            });
        }

        protected override void OnUpdate()
        {
         
            var playerEntities = playerGroup.ToEntityArray(Allocator.Persistent);
            if (playerEntities.Length > 0)
            {
                Entities.WithAllReadOnly<Enemy>().ForEach(
                    (ref LocalToWorld gunTransform, ref Rotation gunRotation, ref FireRocket fire) =>
                    {
                        fire.RocketTimer -= Time.deltaTime;
                        if (fire.RocketTimer > 0)
                            return;

                        fire.RocketTimer = fire.FireCooldown;
          
                        var go = Object.Instantiate(rocketPrefab);
                        var e = go.GetComponent<EntityTracker>().EntityToTrack;
                        // var e = PostUpdateCommands.Instantiate(fire.Rocket);

                        Translation position = new Translation() {Value = gunTransform.Position};

                        var targetPos = EntityManager.GetComponentData<Translation>(playerEntities[0]);
                        Rotation rotation = new Rotation()
                            {Value = Quaternion.LookRotation(-(targetPos.Value - position.Value))};

                        PostUpdateCommands.SetComponent(e, position);
                        PostUpdateCommands.SetComponent(e, rotation);

                      
                        PostUpdateCommands.AddComponent(e, new Rocket() {Type = RocketType.Enemy});
                  
                        PostUpdateCommands.AddComponent(e, new Attack() {Power = 1});
                        PostUpdateCommands.AddComponent(e, new Health() { Value = 1 });
                        PostUpdateCommands.AddComponent(e, new Damage());

                        PostUpdateCommands.AddComponent(e, new MoveForward() { Speed = 3});
                     //   PostUpdateCommands.AddComponent(e, new KillOutofRender() {IsVisible = true});
                    }
                );
            }

            playerEntities.Dispose();
        }
    }
}
