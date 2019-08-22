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
//    public class InitEnemyBehaviour : MonoBehaviour, IReceiveEntity
//    {
//        private Entity _entityInit = Entity.Null;
//        public void SetReceivedEntity(Entity entity)
//        {
//            _entityInit = entity;
//        }

//        void LateUpdate()
//        {
//            if (_entityInit != Entity.Null)
//            {
//                try
//                {
//                    var em = World.Active.EntityManager;


//                    Translation position = new Translation() { Value = gunTransform.Position };

//                    Debug.Log($"Spawn enemy on position:[{position.Value.x},{position.Value.y},{position.Value.z}]");
//                    Rotation rotation = new Rotation() { Value = Quaternion.identity };
//                    var e = _entityInit;
//                    em.SetComponentData(e, position);
//                    em.SetComponentData(e, rotation);

                  

//                    em.AddComponentData(e, new Enemy());
//                    em.AddComponentData(e, new Damage());
//                    em.AddComponentData(e, new Attack() { Power = 1 });
//                    em.AddComponentData(e, new MoveTranslation() { Speed = 1, Direction = Direction.Down });

//                    //    PostUpdateCommands.AddComponent(e, new KillOutofRender() { IsRenderEnable = true });
//                    em.AddComponentData(e, new EntityKiller() { TimeToDie = 500 });

//                    if (spawn.enemyType == EnemyType.Normal)
//                    {
//                        em.AddComponent(e, new Health() { Value = 100 });
//                        em.AddComponent(e, new MoveSin());

//                    }
//                    else if (spawn.enemyType == EnemyType.Super)
//                    {
//                        em.AddComponent(e, new Health() { Value = 500 });
//                        em.AddComponent(e, new FireRocket()
//                        {
//                            Rocket = rocket,
//                            FireCooldown = 2f,
//                            RocketTimer = 0,
//                        });
//                    }

//                }
//                catch
//                {
//                    // Dirty way to check for an Entity that no longer exists.
//                    _entityInit = Entity.Null;
//                }
//            }
//        }

//    }


//}
