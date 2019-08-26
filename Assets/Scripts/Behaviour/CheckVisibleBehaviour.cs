using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public class CheckVisibleBehaviour : MonoBehaviour, IReceiveEntity
    {
        private Renderer _renderer;
        private bool isActive = false;
        public Entity entity = Entity.Null;

        private void OnEnable()
        {
         //  Debug.Log($"CheckVisibleBehaviour OnEnable");
        }

        void Start()
        {
            _renderer = this.GetComponent<MeshRenderer>(); // 获得模型渲染组件
           // Debug.Log($"CheckVisibleBehaviour start begin{_renderer}");
            if (_renderer == null)
            {
                var rs = this.GetComponentsInChildren<MeshRenderer>();
                foreach (var render in rs)
                {
                    if (!render.name.StartsWith("col")){
                        _renderer = render;
                        break;
                    }
                }
            }
           // Debug.Log($"CheckVisibleBehaviour start end{_renderer}");
        }

        void OnBecameVisible()  // 当模型进入屏幕
        {
        //    Debug.Log($"CheckVisibleBehaviour OnBecameVisible");
            isActive = true;
        }

        public bool InVisible()
        {
            return isActive && !this._renderer.isVisible && entity != Entity.Null;
        }
       

        public void SetReceivedEntity(Entity entity)
        {
           // Debug.Log($"CheckVisibleBehaviour SetReceivedEntity：{entity}");
            this.entity = entity;
        }
    }
}
