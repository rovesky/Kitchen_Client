using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [DisableAutoCreation]
    public class ExlosionSystem : ComponentSystem
    {

        private GameObject prefab;
        private Entity prefabEntity;

        protected override void OnCreate()
        {
            prefab = Resources.Load("Prefabs/Explosion") as GameObject;
            prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, World.Active);
        }

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Explosion>().ForEach((Entity entity, ref Health health,ref Translation gunTransform) =>
            {

                if (health.Value <= 0)
                {
                    if (health.Value < 0)
                        Debug.LogWarning($"ExlosionSystem :{health.Value},{prefabEntity}");
              //      var e = PostUpdateCommands.Instantiate(prefabEntity);

                //    Translation position = new Translation() { Value = gunTransform.Value };
               //     PostUpdateCommands.SetComponent(e, position);

                    Object.Instantiate(prefab, gunTransform.Value, Quaternion.identity);
                }
            });
        }
    }
}


