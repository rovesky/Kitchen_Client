using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    public class SpawnEnemySystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.ForEach(
               (ref LocalToWorld gunTransform, ref Rotation gunRotation, ref SpawnEntity spawn) =>
               {
                   if (spawn.entity == null)
                       return;

                   spawn.spawnTimer -= Time.deltaTime;
                   if (spawn.spawnTimer > 0)
                       return;

                   spawn.spawnTimer = UnityEngine.Random.Range(spawn.spawnIntervalMin, spawn.spawnIntervalMax);

                   var e = PostUpdateCommands.Instantiate(spawn.entity);

                   Translation position = new Translation() { Value = gunTransform.Position };
                   Rotation rotation = new Rotation() { Value = Quaternion.identity };

                   PostUpdateCommands.SetComponent(e, position);
                   PostUpdateCommands.SetComponent(e, rotation);

               }
           );
        }
    }
}
