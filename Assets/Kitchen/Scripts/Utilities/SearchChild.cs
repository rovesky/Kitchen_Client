using System.Collections.Generic;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class SearchChild : MonoBehaviour
    {
        static Dictionary<string, Transform> buffer = new Dictionary<string, Transform>();

        private static Transform FindChild(Transform FatherTrans, string childName)
        {
            if (childName == "")
                return null;

            Transform child = FatherTrans.Find(childName);
            if (child != null)
                return child;

            Transform go = null;
            for (int i = 0; i < FatherTrans.childCount; i++)
            {
                child = FatherTrans.GetChild(i);
                go = FindChild(child, childName);
                if (go != null)
                    return go;
            }

            return null;
        }


        public static Transform FindChildFast(Transform FatherTrans, string childName)
        {
            if (buffer.ContainsKey(childName))
                return buffer[childName];
            else
            {
                Transform child = FindChild(FatherTrans, childName);
                buffer.Add(childName, child);
                return child;
            }
        }

    }
}
