using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct KillOutofRender : IComponentData
    {
        public bool IsRenderEnable;
    }
}
