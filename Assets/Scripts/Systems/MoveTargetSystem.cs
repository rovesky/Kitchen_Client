﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class MoveTargetSystem : ComponentSystem
    {
        public EntityQuery PlayerGroup;

        protected override void OnCreate()
        {
            PlayerGroup = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                   typeof(Player),
                }
            });
        }

        protected override void OnUpdate()
        {
            var PlayerEntities = PlayerGroup.ToEntityArray(Allocator.Persistent);

            if (PlayerEntities.Length == 0)
            {
                PlayerEntities.Dispose();
                return;
            }

            Entities.ForEach((ref Translation position, ref Rotation rotation, ref MoveTarget move) =>
            {

                var target = PlayerEntities[0];

                var targetPos = EntityManager.GetComponentData<Translation>(target);
                var targetRotation = EntityManager.GetComponentData<Rotation>(target);

                Vector3 value = Vector3.MoveTowards(position.Value, targetPos.Value, move.Speed * Time.deltaTime);

                position = new Translation()
                {
                    Value = value
                };

                Vector3 relativePos = position.Value - targetPos.Value;

                rotation = new Rotation()
                {
                    Value = Quaternion.LookRotation(relativePos)
                };
            });
            PlayerEntities.Dispose();
        }
    }
}
