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
            Entities.WithAllReadOnly<Explosion>().ForEach((Entity entity, ref Despawn despawn,ref Translation gunTransform) =>
            {
                Object.Instantiate(prefab, gunTransform.Value, Quaternion.identity);
            });
        }
    }
}


