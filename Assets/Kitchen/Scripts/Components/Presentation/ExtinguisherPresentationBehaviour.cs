using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class ExtinguisherPresentationBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject Slot;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            if (Slot == null)
                return;

            var slotEntity = conversionSystem.GetPrimaryEntity(Slot);

            dstManager.AddComponentData(entity, new ExtinguisherPresentation()
            {
               Smog = null,
               SmogPos = dstManager.GetComponentData<Translation>(slotEntity).Value,
            });

        }
    }
}
