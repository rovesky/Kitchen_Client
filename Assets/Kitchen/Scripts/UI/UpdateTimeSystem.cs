using System;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateTimeSystem : ComponentSystem
    {

        private int frameCount;
        private float passedTime;

        protected override void OnUpdate()
        {
            UIManager.Instance.UpdateTime((ushort)DateTime.Now.Second);
        }
    }
}