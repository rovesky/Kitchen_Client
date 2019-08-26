using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SpawnEnemySystem : ComponentSystem
    {

        private Entity rocket;
        private GameObject enemy1Prefab;
        private GameObject enemy2Prefab;

        protected override void OnCreate()
        {
            var prefab = Resources.Load("Prefabs/EnemyRocket") as GameObject;       
            rocket = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, World.Active);

            enemy1Prefab = Resources.Load("Prefabs/Enemy1") as GameObject;
            enemy2Prefab = Resources.Load("Prefabs/Enemy3") as GameObject;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach(
               (ref LocalToWorld gunTransform, ref Rotation gunRotation, ref SpawnEnemy spawn) =>
               {
                   //if (spawn.entity == null)
                   //    return;

                   spawn.spawnTimer -= Time.deltaTime;
                   if (spawn.spawnTimer > 0)
                       return;

                   spawn.spawnTimer = Random.Range(spawn.spawnIntervalMin, spawn.spawnIntervalMax);
                
                   var enemyPrefab = spawn.enemyType == EnemyType.Normal? enemy1Prefab:enemy2Prefab;
                   var go = Object.Instantiate(enemyPrefab);
                   var e = go.GetComponent<EntityTracker>().EntityToTrack;
                  // var e = PostUpdateCommands.Instantiate(spawn.entity);

                   Translation position = new Translation() { Value = gunTransform.Position };
                   Rotation rotation = new Rotation() { Value = Quaternion.identity };

            //       Debug.Log($"Spawn enemy on position:[{position.Value.x},{position.Value.y},{position.Value.z}]");


                   PostUpdateCommands.SetComponent(e, position);
                   PostUpdateCommands.SetComponent(e, rotation);

                
                   PostUpdateCommands.AddComponent(e, new Enemy());
                   PostUpdateCommands.AddComponent(e, new Damage());
                   PostUpdateCommands.AddComponent(e, new Attack() { Power = 1 });
                   PostUpdateCommands.AddComponent(e, new Explosion());
                   //   PostUpdateCommands.AddComponent(e, new KillOutofRender() { IsVisible = true });

                   if (spawn.enemyType == EnemyType.Normal)
                   {
                       PostUpdateCommands.AddComponent(e, new Health() { Value = 100 });
                       PostUpdateCommands.AddComponent(e, new MoveSin());
                       PostUpdateCommands.AddComponent(e, new MoveTranslation() { Speed = 1f, Direction = Direction.Down });

                   }
                   else if (spawn.enemyType == EnemyType.Super)
                   {
                       PostUpdateCommands.AddComponent(e, new MoveTranslation() { Speed = 0.5f, Direction = Direction.Down });

                       PostUpdateCommands.AddComponent(e, new Health() { Value = 500 });
                       PostUpdateCommands.AddComponent(e, new FireRocket()
                       {
                           Rocket = rocket,
                           FireCooldown = 2f,
                           RocketTimer = 0,
                       });
                   }
               });
        }
    }
}
