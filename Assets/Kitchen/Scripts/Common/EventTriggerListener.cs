using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;
using System;

namespace UnityEngine.EventSystems
{
    public class EventTriggerListener : MonoBehaviour,
                                        IPointerEnterHandler,
                                        IPointerExitHandler,
                                        IPointerDownHandler,
                                        IPointerUpHandler,
                                        IPointerClickHandler,
                                        IBeginDragHandler,
                                        IInitializePotentialDragHandler,
                                        IDragHandler,
                                        IEndDragHandler,
                                        IDropHandler,
                                        IScrollHandler,
                                        IUpdateSelectedHandler,
                                        ISelectHandler,
                                        IDeselectHandler,
                                        IMoveHandler,
                                        ISubmitHandler,
                                        ICancelHandler
    {
        public delegate void BaseDelegate(BaseEventData eventData);
        public delegate void PointDelegate(PointerEventData eventData);
        public delegate void AxisDelegate(AxisEventData eventData);
        public delegate void BoolDelegate(bool isPressed);
        public delegate void VoidDelegate(GameObject go);

        public VoidDelegate onEnable;
        public PointDelegate onBeginDrag;
        public BaseDelegate onCancel;
        public BaseDelegate onDeselect;
        public PointDelegate onDrag;
        public PointDelegate onDrop;
        public PointDelegate onEndDrag;
        public PointDelegate onInitializePotentialDrag;
        public AxisDelegate onMove;
        public PointDelegate onPointerClick;
        public PointDelegate onPointerDown;
        public PointDelegate onPointerEnter;
        public PointDelegate onPointerExit;
        public PointDelegate onPointerUp;
        public PointDelegate onScroll;
        public BaseDelegate onSelect;
        public BaseDelegate onSubmit;
        public BaseDelegate onUpdateSelected;
        public BoolDelegate onPress;

        static public EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null) listener = go.AddComponent<EventTriggerListener>();
            return listener;
        }

        public void OnEnable() { if (onEnable != null) onEnable(gameObject); }
        public void OnBeginDrag(PointerEventData eventData) { if (onBeginDrag != null) onBeginDrag(eventData); }
        public void OnCancel(BaseEventData eventData) { if (onCancel != null) onCancel(eventData); }
        public void OnDeselect(BaseEventData eventData) { if (onDeselect != null) onDeselect(eventData); }
        public void OnDrag(PointerEventData eventData) { if (onDrag != null) onDrag(eventData); }
        public void OnDrop(PointerEventData eventData) { if (onDrop != null) onDrop(eventData); }
        public void OnEndDrag(PointerEventData eventData) { if (onEndDrag != null) onEndDrag(eventData); }
        public void OnInitializePotentialDrag(PointerEventData eventData) { if (onInitializePotentialDrag != null) onInitializePotentialDrag(eventData); }
        public void OnMove(AxisEventData eventData) { if (onMove != null) onMove(eventData); }
        public void OnPointerClick(PointerEventData eventData) { if (onPointerClick != null) onPointerClick(eventData); }
        public void OnScroll(PointerEventData eventData) { if (onScroll != null) onScroll(eventData); }
        public void OnSelect(BaseEventData eventData) { if (onSelect != null) onSelect(eventData); }
        public void OnSubmit(BaseEventData eventData) { if (onSubmit != null) onSubmit(eventData); }
        public void OnUpdateSelected(BaseEventData eventData) { if (onUpdateSelected != null) onUpdateSelected(eventData); }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (onPress != null)
                onPress(true);

            if (onPointerDown != null)
                onPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (onPress != null)
                onPress(false);

            if (onPointerUp != null)
                onPointerUp(eventData);
        }


        public void OnPointerEnter(PointerEventData eventData) { if (onPointerEnter != null) onPointerEnter(eventData); }
        public void OnPointerExit(PointerEventData eventData) { if (onPointerExit != null) onPointerExit(eventData); }
    }
}