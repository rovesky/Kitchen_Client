using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenerCompelet : MonoBehaviour
{
    public void TweenerCmplete()
    {
        Messenger.Broadcast(MessengerEventDef.TWEENER_COMPLETE);
    }

}
