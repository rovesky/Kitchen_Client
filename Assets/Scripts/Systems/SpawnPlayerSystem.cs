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
            var prefab = Resources.Load("Prefabs/Rocket") as GameObject;
            rocket = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, World.Active);
            //  rocket = GameObjectConversionUtility.g(prefab, World.Active);

           // ConvertToEntity.InjectOriginalComponents(World.Active, EntityManager, prefab.transform);
            if (EntityManager.HasComponent<Transform>(rocket))
            {
                Debug.Log($"create player: {rocket},has Transform!");
            }

            //var components = prefab.GetComponents<Component>();

            //for (var i = 0; i != components.Length; i++)
            //{
            //    var com = components[i];
            //    Debug.Log($"rocket components:{com.GetType().Name}");
            //}
        }

        protected override void OnUpdate()
        {

            Entities.ForEach(
               (ref LocalToWorld gunTransform, ref Rotation gunRotation, ref SpawnPlayer spawn) =>
               {
                   if (spawn.isSpawned)
                       return;

                   spawn.isSpawned = true;

                   var e = PostUpdateCommands.Instantiate(spawn.entity);

                   Translation position = new Translation() { Value = gunTransform.Position };
                   Rotation rotation = new Rotation() { Value = gunRotation.Value };

                   PostUpdateCommands.SetComponent(e, position);
                   PostUpdateCommands.SetComponent(e, rotation);
                   PostUpdateCommands.AddComponent(e, new Player());
                   PostUpdateCommands.AddComponent(e, new Attack() { Power = 10000 });
                   PostUpdateCommands.AddComponent(e, new Damage());
                   PostUpdateCommands.AddComponent(e, new Health() { Value = 3 });
                   PostUpdateCommands.AddComponent(e, new Score() { ScoreValue = 0,MaxScoreValue = 0});
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
                       InputMask = 1<<LayerMask.NameToLayer("plane")
                   });
               });
        }
    }
}
