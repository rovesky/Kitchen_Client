using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class DrawLine : MonoBehaviour
    {

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var pos = transform.position;
            pos.y = 0.1f;
            Gizmos.DrawLine(pos, pos + (Vector3)math.forward(transform.rotation)* 3);

        }


    }
}
