using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    public struct MoveComponent : IComponentData
    {
        public int Speed;
        public Direction Direction;    }


    public class MoveBehaviour : MonoBehaviour,  IConvertGameObjectToEntity
    {
        public int Speed;

        public Direction Direction;


        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<MoveComponent>(
                entity,
                new MoveComponent()
                {
                    Speed = Speed,
                    Direction = Direction,
                });
        }
    }


    #region System
    // update before physics gets going so that we dont have hazzard warnings
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class MoveSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            float dt = Time.fixedDeltaTime;

            Entities.ForEach(
                (ref Translation position,   ref MoveComponent move) =>
                {
                    var value = position.Value;
                    if (move.Direction == Direction.Up)
                    {
                        value.z += move.Speed * Time.deltaTime;
                    }
                    else if(move.Direction == Direction.Down)
                    {
                        value.z -= move.Speed * Time.deltaTime;
                    }
                    else if (move.Direction == Direction.Left)
                    {
                        value.x -= move.Speed * Time.deltaTime;
                    }
                    else if (move.Direction == Direction.Right)
                    {
                        value.x += move.Speed * Time.deltaTime;
                    }

                    position = new Translation()
                    {
                        Value = value
                    };

                }
            );
        }
    }
    #endregion

}