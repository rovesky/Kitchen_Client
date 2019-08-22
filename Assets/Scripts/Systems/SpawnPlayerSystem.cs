using Unity.Entities;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    
    public class SpawnPlayerSystem : ComponentSystem
    {
        private Entity rocket;

        protected override void OnCreate()
        {
            var rocketPrefab = Resources.Load("Prefabs/Rocket") as GameObject;
            rocket = GameObjectConversionUtility.ConvertGameObjectHierarchy(rocketPrefab, World.Active);
        }

        protected override void OnUpdate()
        {

            Entities.ForEach(
               (Entity entity,ref LocalToWorld gunTransform, ref Rotation gunRotation, ref SpawnPlayer spawn) =>
               {
                   //创建Player
                   var e = PostUpdateCommands.Instantiate(spawn.entity);
                   Translation position = new Translation() { Value = gunTransform.Position };
                   Rotation rotation = new Rotation() { Value = gunRotation.Value };

                   PostUpdateCommands.SetComponent(e, position);
                   PostUpdateCommands.SetComponent(e, rotation);
                   PostUpdateCommands.AddComponent(e, new Player());
                   PostUpdateCommands.AddComponent(e, new Attack() { Power = 10000 });
                   PostUpdateCommands.AddComponent(e, new Damage());
                   PostUpdateCommands.AddComponent(e, new Health() { Value = 3 });
                   PostUpdateCommands.AddComponent(e, new Score() { ScoreValue = 0, MaxScoreValue = 0 });
                   PostUpdateCommands.AddComponent(e, new UpdateHealthUI());

                   PostUpdateCommands.AddComponent(e, new FireRocket()
                   {
                       Rocket = rocket,
                       FireCooldown = 0.1f,
                       RocketTimer = 0,
                   });
                   PostUpdateCommands.AddComponent(e, new MoveMouse()
                   {
                       Speed = 5,
                       InputMask = 1 << LayerMask.NameToLayer("plane")
                   });

                   //移除SpawnPlayer
                   PostUpdateCommands.RemoveComponent(entity,typeof(SpawnPlayer));
               });
        }
    }
}
