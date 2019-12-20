using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

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
               Resources.Load("Apple") as GameObject,
               GameObjectConversionSettings.FromWorld(World,
                   World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));


        }

        protected override void OnUpdate()
        {
            if (isSpawned)
                return;

            isSpawned = true;

            var query = GetEntityQuery(typeof(TriggerData));
            var entities = query.ToEntityArray(Allocator.TempJob);


            //生成Plate
            for (var i = 0; i < 1; ++i)
            {
                var entity = entities[i * 2];
                var slot = EntityManager.GetComponentData<SlotPredictedState>(entity);
                var triggerData = EntityManager.GetComponentData<TriggerData>(entity);

                var e = EntityManager.Instantiate(platePrefab);
               
                var position = new Translation {Value = triggerData.SlotPos};
                var rotation = new Rotation {Value = Quaternion.identity};

                EntityManager.SetComponentData(e, position);
                EntityManager.SetComponentData(e, rotation);

                EntityManager.AddComponentData(e, new ReplicatedEntityData()
                {
                    Id = 0,
                    PredictingPlayerId = 0
                });

                EntityManager.AddComponentData(e, new Plate());

                slot.FilledInEntity = e;
                EntityManager.SetComponentData(entity, slot);
            
                EntityManager.AddComponentData(e, new ItemInterpolatedState
                {
                    Position = position.Value,
                    Rotation = Quaternion.identity,
                    Owner = Entity.Null
                });

                EntityManager.AddComponentData(e, new EntityPredictedState()
                {
                    Transform = new RigidTransform()
                    {
                        pos = position.Value,
                        rot = rotation.Value
                    }
                });

                EntityManager.AddComponentData(e, new ItemPredictedState
                {
                    Owner = Entity.Null
                });

                EntityManager.AddComponentData(e, new TriggerSetting()
                {
                    Distance = 0.1f
                });

                EntityManager.AddComponentData(e, new TriggerPredictedState()
                {
                    TriggeredEntity = Entity.Null
                });

                EntityManager.RemoveComponent<PhysicsVelocity>(e);

            }

            entities.Dispose();

        }      
    }
}