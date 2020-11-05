using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldEventReceiver : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action<PointerEventData> OnPointerDownAction;

    public Action<PointerEventData> OnPointerUpAction;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownAction?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpAction?.Invoke(eventData);
    }
}
