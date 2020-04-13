using System;
using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnGameSystem : ComponentSystem
    {
        private bool isSpawned;
        public const ushort TotalTime = 300;
        protected override void OnCreate()
        {
         
        }

        protected override void OnUpdate()
        {
            if (isSpawned)
                return;

            isSpawned = true;

            var e = EntityManager.CreateEntity(typeof(ReplicatedEntityData),typeof(Countdown),typeof(Score));
            EntityManager.SetComponentData(e,new ReplicatedEntityData()
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            EntityManager.SetComponentData(e,new Countdown()
            {
                Value = TotalTime,
                EndTime = DateTime.Now.AddSeconds(TotalTime).Ticks
            });

            EntityManager.SetComponentData(e,new Score()
            {
                Value = 0
            });
        }
    }
}