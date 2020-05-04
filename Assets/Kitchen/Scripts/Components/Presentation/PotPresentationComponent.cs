using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class PotPresentation : IComponentData
    {
        public Entity Full;
        public Entity Empty;
        public GameObject Steam;
    }
}