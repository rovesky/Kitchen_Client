using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class UIObject : IComponentData
    {
        public GameObject Icon;
        public GameObject Progress;
        public GameObject Info;
    }
}