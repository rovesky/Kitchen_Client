//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.Entities;
//using Unity.Transforms;
//using UnityEngine;

//namespace Assets.Scripts.ECS
//{

//    public class InitPlayerSystem : ComponentSystem
//    {
//        private GameObject rocketPrefab;
//        //  private GameObject playerPrefab;
//        private Entity rocket;

//        protected override void OnCreate()
//        {
//            rocketPrefab = Resources.Load("Prefabs/Rocket") as GameObject;

//            //            rocket = GameObjectConversionUtility.ConvertGameObjectHierarchy(rocketPrefab, World.Active);

//        }

//        protected override void OnUpdate()
//        {
//            Entities.WithAllReadOnly<Player>().ForEach((Entity e) =>
//            {
//                if (!EntityManager.HasComponent<Translation>(e))
//                {
//                    return;
//                }

//                if (EntityManager.HasComponent<Attack>(e))
//                {
//                    return;
//                }
//                //Translation position = new Translation() { Value = gunTransform.Position };
//                //Rotation rotation = new Rotation() { Value = gunRotation.Value };
//                Translation position = new Translation();
//                position.Value.x = 0;
//                position.Value.y = 0;
//                position.Value.z = -2;

//                //    LocalToWorld localToWorld = new LocalToWorld();


//                PostUpdateCommands.SetComponent(e, position);
//                // PostUpdateCommands.SetComponent(e, rotation);
//                //    PostUpdateCommands.AddComponent(e, new Player());
//                PostUpdateCommands.AddComponent(e, new Attack() { Power = 10000 });
//                PostUpdateCommands.AddComponent(e, new Damage());
//                PostUpdateCommands.AddComponent(e, new Health() { Value = 3 });
//                PostUpdateCommands.AddComponent(e, new Score() { ScoreValue = 0, MaxScoreValue = 0 });
//                PostUpdateCommands.AddComponent(e, new UpdateHealthUI());

//                //PostUpdateCommands.AddComponent(e, new FireRocket()
//                //{
//                //    Rocket = rocket,
//                //    FireCooldown = 0.1f,
//                //    RocketTimer = 0,
//                //});
//                PostUpdateCommands.AddComponent(e, new MoveMouse()
//                {
//                    Speed = 5,
//                    InputMask = 1 << LayerMask.NameToLayer("plane")
//                });

//            });
//        }
//    }
//}
