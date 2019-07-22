using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct MoveSpeed : IComponentData
    {
        public float SpeedPerSecond;
    }

    [DisallowMultipleComponent]
    public class MoveSpeedProxy : ComponentDataProxy<MoveSpeed> { }
}
