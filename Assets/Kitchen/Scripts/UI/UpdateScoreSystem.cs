﻿using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateScoreSystem : SystemBase
    {

        private int frameCount;
        private float passedTime;

        protected override void OnUpdate()
        {
            Entities
                .ForEach((Entity entity,
                    in Score score) =>
                {
                    UIManager.Instance.UpdateScore(score.Value);
                }).Run();
        }
    }
}