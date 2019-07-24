using Unity.Physics.Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;
using System.Collections.Generic;
using System;
using Unity.Physics;

namespace Assets.Scripts.ECS
{
    public struct FireRocket : IComponentData
    {
        public Entity rocket;
    }


    public class FireBehaviour : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        public GameObject bullet;


        // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
        public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
        {
            gameObjects.Add(bullet);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<FireRocket>(
                entity,
                new FireRocket()
                {
                    rocket = conversionSystem.GetPrimaryEntity(bullet),
                });
        }
    }


    #region System
    // update before physics gets going so that we dont have hazzard warnings
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class FireSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            float dt = Time.fixedDeltaTime;

            Entities.ForEach(
                (ref LocalToWorld gunTransform, ref Rotation gunRotation, ref FireRocket gun) =>
                {

                    if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
                    {

                        if (gun.rocket != null)
                        {
                            var e = PostUpdateCommands.Instantiate(gun.rocket);

                            Translation position = new Translation() { Value = gunTransform.Position };
                            Rotation rotation = new Rotation() { Value = gunRotation.Value };

                            PostUpdateCommands.SetComponent(e, position);
                            PostUpdateCommands.SetComponent(e, rotation);

                        }
                    }

                }
            );
        }
    }
    #endregion
}