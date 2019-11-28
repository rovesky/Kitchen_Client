using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnPlatesSystem : ComponentSystem
    {
        private Entity platePrefab;
        private bool isSpawned = false;

        protected override void OnCreate()
        {
            base.OnCreate();

           platePrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
           Resources.Load("Plate") as GameObject, World.Active);

        }

        protected override void OnUpdate()
        {
            if (isSpawned)
                return;

            isSpawned = true;

            var e = EntityManager.Instantiate(platePrefab);
            Translation position = new Translation() { Value = { x = -4, y = 1, z = -1 } };
            Rotation rotation = new Rotation() { Value = Quaternion.identity };

            EntityManager.SetComponentData(e, position);

            EntityManager.AddComponentData(e, new Plate());

            EntityManager.AddComponentData(e, new ItemInterpolatedState()
            {
                position = position.Value,
                rotation = rotation.Value

            });

        }      
    }
}