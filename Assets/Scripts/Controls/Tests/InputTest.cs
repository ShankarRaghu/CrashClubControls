using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    public GameObject Object;


    private void Start()
    {
        InputSystem.Instance.AddFullScreenDragListener(OnDrag);
    }

    private void OnDrag(float a_Speed, Vector3 aPrevPosition, Vector3 a_CurrentPosition)
    {
        Object.transform.Rotate(new Vector3(0f, 0f, a_Speed));
    }

    private void OnDestroy()
    {
        InputSystem.Instance.RemoveFullScreenDragListener();
    }
}
