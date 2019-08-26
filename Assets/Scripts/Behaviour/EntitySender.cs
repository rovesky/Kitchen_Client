using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public interface IReceiveEntity
    {
        void SetReceivedEntity(Entity entity);
    }

    public struct SentEntity : IComponentData
    {
    }

    public class EntitySender : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject[] EntityReceivers;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new SentEntity() { });
            foreach (var entityReceiver in EntityReceivers)
            {
                var potentialReceivers = entityReceiver.GetComponents<MonoBehaviour>();
                foreach (var potentialReceiver in potentialReceivers)
                {
                    if (potentialReceiver is IReceiveEntity receiver)
                    {
                        receiver.SetReceivedEntity(entity);
                    }
                }
            }
        }
    }

}