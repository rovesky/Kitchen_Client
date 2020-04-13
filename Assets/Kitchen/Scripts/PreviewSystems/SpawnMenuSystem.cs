using System;
using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnMenuSystem : ComponentSystem
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

            CreateMenu(1, 5);
            CreateMenu(2, 6,1,7);
        }

        private void CreateMenu(ushort productId,ushort material1,ushort material2 = 0
            ,ushort material3 = 0,ushort material4 = 0)
        {
            var e = EntityManager.CreateEntity(typeof(ReplicatedEntityData),typeof(Menu));
            EntityManager.SetComponentData(e,new ReplicatedEntityData()
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            EntityManager.SetComponentData(e,new Menu()
            {
                ProductId = productId,
                MaterialId1 = material1,
                MaterialId2 = material2,
                MaterialId3 = material3,
                MaterialId4 = material4,
            });
            EntityManager.AddComponentData(e,new NewEntity());
        }
    }
}