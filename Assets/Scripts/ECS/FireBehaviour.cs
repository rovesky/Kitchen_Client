using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct FireRocket : IComponentData
    {
        public Entity rocket;    
        public float  minRocketTimer;
        public float rocketTimer;
    }


    public class FireBehaviour : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        public GameObject bullet;
        public float minRocketTimer;

        // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
        public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
        {
            gameObjects.Add(bullet);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {     
            dstManager.AddComponentData(
                entity,
                new FireRocket()
                {
                    rocket = conversionSystem.GetPrimaryEntity(bullet), 
                    minRocketTimer = minRocketTimer,
                    rocketTimer = 0,
                });
        }
    }

    public class PlayerFireSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Player>().ForEach(
                (ref LocalToWorld gunTransform, ref Rotation gunRotation, ref FireRocket fire) =>
                {
                    if (fire.rocket == null)
                        return;

                    fire.rocketTimer -= Time.deltaTime;
                    if (fire.rocketTimer > 0)
                        return;

                    fire.rocketTimer = fire.minRocketTimer;

                    if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
                    {
                        var e = PostUpdateCommands.Instantiate(fire.rocket);

                        Translation position = new Translation() { Value = gunTransform.Position };
                        Rotation rotation = new Rotation() { Value = gunRotation.Value };

                        PostUpdateCommands.SetComponent(e, position);
                        PostUpdateCommands.SetComponent(e, rotation);
                    }
                }
            );
        }
    }


    public class EnemyFireSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {

            Entities.WithAllReadOnly<Enemy>().ForEach(
                (ref LocalToWorld gunTransform, ref Rotation gunRotation, ref FireRocket fire) =>
                {
                    if (fire.rocket == null)
                        return;

                    fire.rocketTimer -= Time.deltaTime;
                    if (fire.rocketTimer > 0)
                        return;

                    fire.rocketTimer = fire.minRocketTimer;

                    var e = PostUpdateCommands.Instantiate(fire.rocket);

                    Translation position = new Translation() { Value = gunTransform.Position };
                    Rotation rotation = new Rotation() { Value = gunRotation.Value };

                    PostUpdateCommands.SetComponent(e, position);
                    PostUpdateCommands.SetComponent(e, rotation);

                }
            );
        }
    }
}