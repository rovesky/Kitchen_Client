using Unity.Entities;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    //public class ConversionRocektSystem : GameObjectConversionSystem
    //{
    //    protected override void OnUpdate()
    //    {
    //        Entities.ForEach((FireBehaviour behaviour) => {
    //        //    behaviour.DeclareReferencedPrefabs(this.)
    //            behaviour.Convert(GetPrimaryEntity(behaviour), DstEntityManager, this); });

    //    }
    //}

    public class SpawnPlayerSystem : ComponentSystem
    {
      //  private GameObjectConversionSystem gameObjectConversionSystem;

        protected override void OnCreate()
        {
        //    gameObjectConversionSystem = World.GetOrCreateSystem<GameObjectConversionSystem>();
        }

        protected override void OnUpdate()
        {
            Debug.Log($"SpawnPlayerSystem OnUpdate!");
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
                   PostUpdateCommands.AddComponent(e, new UpdateHealthUI());
                 

                   //var prefab = (GameObject)Resources.Load("Prefabs/Rocket");
                   //var rocket = GetPrimaryEntity(prefab);

                   //Debug.Log($"prefab:{prefab},rocket:{rocket}");
                   //PostUpdateCommands.AddComponent(e, new FireRocket()
                   //{
                   //    rocket = rocket,
                   //    minRocketTimer = 0.1f,
                   //    rocketTimer = 0,
                   //});

                   //  PostUpdateCommands.AddComponent(e, new PhysicsBody());
               }
           );
        }
    }
}
