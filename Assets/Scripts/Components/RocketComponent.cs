using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Assets.Scripts.ECS
{
    public enum RocketType
    {
        Player,
        Enemy
    }

    [Serializable]
    public struct Rocket : IComponentData
    {
        public RocketType Type;
    }
}
