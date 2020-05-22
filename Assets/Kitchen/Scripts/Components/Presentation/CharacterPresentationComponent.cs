using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
  
  
    public class CharacterPresentation : IComponentData
    {
        public GameObject CharacterObject;
        public GameObject KnifeObject1;
        public GameObject KnifeObject2;
    }

}