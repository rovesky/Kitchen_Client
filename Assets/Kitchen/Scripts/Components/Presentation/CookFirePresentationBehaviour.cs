using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class CookFirePresentationBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
     //   public GameObject PresentationObject;


        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            
               
            dstManager.AddComponentData(entity, new CookFirePresentation
            {
                Object = null
            });
        }
    }
}
