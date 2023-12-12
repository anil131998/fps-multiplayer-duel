using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class ButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public bool isPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }


}
