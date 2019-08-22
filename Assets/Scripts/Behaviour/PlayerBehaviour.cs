using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    public class PlayerBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent(entity, typeof(Player));
            //dstManager.AddComponentData(entity, new Player()
            //{

            //});
        }
    }

    //[DisallowMultipleComponent]
    //public class PlayerBehaviour : ComponentDataProxy<Player> { }
}
