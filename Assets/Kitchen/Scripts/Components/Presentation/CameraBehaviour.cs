//using Unity.Entities;
//using UnityEngine;

//namespace FootStone.Kitchen
//{
//    public class CameraBehaviour : MonoBehaviour, IConvertGameObjectToEntity
//    {
//        public GameObject Camera;
      
//        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
//            GameObjectConversionSystem conversionSystem)
//        {

//            dstManager.AddComponentData(entity, new Cam()
//            {
//                Full = conversionSystem.GetPrimaryEntity(Full),
//                Empty = conversionSystem.GetPrimaryEntity(Empty),
//                Steam = null
//            });

//        }
//    }
//}
