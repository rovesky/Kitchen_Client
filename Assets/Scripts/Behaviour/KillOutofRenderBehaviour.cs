using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public class KillOutofRenderBehaviour : MonoBehaviour//, IConvertGameObjectToEntity
    {
        private Renderer m_renderer;
        private bool m_isActiv = false;
        private Entity entity;
        private EntityManager dstManager;

        private void OnEnable()
        {
            Debug.Log($"KillOutofRenderBehaviour OnEnable");
        }

        void Start()
        {
            Debug.Log($"KillOutofRenderBehaviour start");
            m_renderer = this.GetComponent<Renderer>(); // 获得模型渲染组件
            if (m_renderer == null)
            {
                var rs = this.GetComponentsInChildren<Renderer>();
                foreach (var render in rs)
                {
                    if (!render.name.StartsWith("col"))
                    {
                        m_renderer = render;
                        break;
                    }
                }
            }
        }

        void OnBecameVisible()  // 当模型进入屏幕
        {
            Debug.Log($"KillOutofRenderBehaviour OnBecameVisible");
            m_isActiv = true;
        }

        void Update()
        {
            Debug.Log($"KillOutofRenderBehaviour Update");
            if (m_isActiv && !this.m_renderer.isVisible)  // 如果移动到屏幕外
            {
                Debug.Log($"SetComponentData,KillOutofRender:{entity.Index}");
                dstManager.SetComponentData(entity, new KillOutofRender() { IsRenderEnable = false });
            }
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Debug.Log($"KillOutofRenderBehaviour Convert");
            this.entity = entity;
            this.dstManager = dstManager;

            dstManager.AddComponentData<KillOutofRender>(entity, new KillOutofRender()
            {
                IsRenderEnable = true,
            });
        }
    }
}
