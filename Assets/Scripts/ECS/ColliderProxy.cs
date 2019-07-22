//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.Entities;
//using UnityEngine;

//namespace Assets.Scripts.ECS
//{
//    public struct Collider1 : IComponentData
//    {

//    }

//    [DisallowMultipleComponent]
//    public class ColliderProxy : MonoBehaviour, IConvertGameObjectToEntity
//    {
//        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//        {
//            var data = new Collider1();
//            dstManager.AddComponentData(entity, data);
//        }

//        void OnTriggerEnter(Collider other)
//        {

//        }

//    }


//}
