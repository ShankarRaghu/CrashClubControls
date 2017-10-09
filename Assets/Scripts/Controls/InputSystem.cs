using System;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    #region Input Enums
    public enum eKeyState
    {
        Up,
        Down,
        Hold,
    }
    #endregion

    #region Registers
    /// <summary>
    /// Class holds info on which keys to listen to, the key state to listen and the callback to fire
    /// </summary>
    public class InputRegister
    {
        //Keys
        public KeyCode Keycode;
        public eKeyState Keystate;

        /// <summary>
        /// Params: KeyCode and eKeyState
        /// </summary>
        public Action<KeyCode, eKeyState> OnKeyPressed;

        public InputRegister(KeyCode a_KeyCode, eKeyState a_KeyState, Action<KeyCode, eKeyState> a_Callback)
        {
            Keycode = a_KeyCode;
            Keystate = a_KeyState;
            OnKeyPressed = a_Callback;
        }
        //

        //Touch
        public Rect TouchRect;
        public int TouchPhase;

        /// <summary>
        /// Params: Touch Rect, Touch Position and TouchPhase
        /// </summary>
        public Action<Rect, Vector2, int> OnTouch;

        public InputRegister(Rect a_TouchRect, Action<Rect, Vector2, int> a_Callback)
        {
            TouchRect = a_TouchRect;
            TouchPhase = -1;
            OnTouch = a_Callback;
        }

        public InputRegister(Rect a_TouchRect, TouchPhase a_TouchPhase, Action<Rect, Vector2, int> a_Callback)
        {
            TouchRect = a_TouchRect;
            TouchPhase = (int)a_TouchPhase;
            OnTouch = a_Callback;
        }
        //

        //Drag
        public Action<float, Vector3, Vector3> OnDrag;

        public Rect DragRect;
        public Vector3 CachedPos;

        public InputRegister(Rect a_DragRect, Action<float, Vector3, Vector3> a_Callback)
        {
            DragRect = a_DragRect;
            OnDrag = a_Callback;
        }
        //

        ~InputRegister()
        {
            OnKeyPressed = null;
            OnTouch = null;
            OnDrag = null;
        }
    }
    #endregion


    private static InputSystem m_Instance;
    public static InputSystem Instance
    {
        get
        {
            if (m_Instance == null)
            {
                GameObject obj = new GameObject("InputSystem");
                m_Instance = obj.AddComponent<InputSystem>();
            }

            return m_Instance;
        }
    }

    private List<InputRegister> KeyListenerList;
    private List<InputRegister> TouchListenerList;
    private List<InputRegister> DragListenerList;

    private Vector3 m_PreviousMousePositionForTouchPhase;
    private float m_TouchPhaseMouseBuffer = 0.1f;


    private void OnEnable()
    {

    }

    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
        DontDestroyOnLoad(gameObject);

        KeyListenerList = new List<InputRegister>();
        TouchListenerList = new List<InputRegister>();
        DragListenerList = new List<InputRegister>();
    }

    private void Start()
    {
        //Debug
        //AddKeyListener(KeyCode.A, eKeyState.Down, OnAPressed);
        //AddKeyListener(KeyCode.W, eKeyState.Hold, OnWPressed);
        //AddKeyListener(KeyCode.D, eKeyState.Up, OnDPressed);
        //AddTouchListener(new Rect(0, 0, Screen.width / 4, Screen.height), TouchPhase.Moved, OnFullScreenTouch);
        //AddFullScreenDragListener(OnDrag);
        //-----
    }

    private void Update ()
    {
        if (KeyListenerList != null)
        {
            //Iterate through all items in the register list
            for (int i = 0; i < KeyListenerList.Count; i++)
            {
                //Check if the key is pressed in the required state. If yes, fire the action 
                KeyCode key = KeyListenerList[i].Keycode;
                switch (KeyListenerList[i].Keystate)
                {
                    case eKeyState.Up:
                        if (Input.GetKeyUp(key))
                        {
                            if (KeyListenerList[i].OnKeyPressed != null)
                            {
                                KeyListenerList[i].OnKeyPressed(key, KeyListenerList[i].Keystate);
                            }
                        }
                        break;

                    case eKeyState.Down:
                        if (Input.GetKeyDown(key))
                        {
                            if (KeyListenerList[i].OnKeyPressed != null)
                            {
                                KeyListenerList[i].OnKeyPressed(key, KeyListenerList[i].Keystate);
                            }
                        }
                        break;

                    case eKeyState.Hold:
                        if (Input.GetKey(key))
                        {
                            if (KeyListenerList[i].OnKeyPressed != null)
                            {
                                KeyListenerList[i].OnKeyPressed(key, KeyListenerList[i].Keystate);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            //-----
        }
        
        if (TouchListenerList != null)
        {
            //Iterate through all items in the register list
            for (int i = 0; i < TouchListenerList.Count; i++)
            {
                //Check if it's a touch device or Mouse/Keyboard device
                if (!IsTouchDevice())
                {
                    //See if we need to check for a certain touch phase
                    if (TouchListenerList[i].TouchPhase == -1)
                    {
                        //Check if the mouse click was within the desired rect
                        if (Input.GetMouseButton(0))
                        {
                            if (TouchListenerList[i].TouchRect.Contains(Input.mousePosition))
                            {
                                if (TouchListenerList[i].OnTouch != null)
                                {
                                    TouchListenerList[i].OnTouch(TouchListenerList[i].TouchRect, Input.mousePosition, TouchListenerList[i].TouchPhase);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Convert touch phase to relevant mouse input function
                        TouchPhase touchPhase = (TouchPhase)TouchListenerList[i].TouchPhase;
                        switch (touchPhase)
                        {
                            case TouchPhase.Began:
                                if (Input.GetMouseButtonDown(0))
                                {
                                    if (TouchListenerList[i].TouchRect.Contains(Input.mousePosition))
                                    {
                                        if (TouchListenerList[i].OnTouch != null)
                                        {
                                            TouchListenerList[i].OnTouch(TouchListenerList[i].TouchRect, Input.mousePosition, TouchListenerList[i].TouchPhase);
                                        }
                                    }
                                }
                                break;

                            case TouchPhase.Canceled:
                            case TouchPhase.Ended:
                                if (Input.GetMouseButtonUp(0))
                                {
                                    if (TouchListenerList[i].TouchRect.Contains(Input.mousePosition))
                                    {
                                        if (TouchListenerList[i].OnTouch != null)
                                        {
                                            TouchListenerList[i].OnTouch(TouchListenerList[i].TouchRect, Input.mousePosition, TouchListenerList[i].TouchPhase);
                                        }
                                    }
                                }
                                break;

                            case TouchPhase.Moved:
                                if (Input.GetMouseButton(0))
                                {
                                    if (TouchListenerList[i].TouchRect.Contains(Input.mousePosition))
                                    {
                                        float distance = Vector3.Distance(Input.mousePosition, m_PreviousMousePositionForTouchPhase);
                                        if (distance > m_TouchPhaseMouseBuffer)
                                        {
                                            if (TouchListenerList[i].OnTouch != null)
                                            {
                                                TouchListenerList[i].OnTouch(TouchListenerList[i].TouchRect, Input.mousePosition, TouchListenerList[i].TouchPhase);
                                            }
                                        }
                                    }
                                }

                                m_PreviousMousePositionForTouchPhase = Input.mousePosition;
                                break;

                            case TouchPhase.Stationary:
                                if (Input.GetMouseButton(0))
                                {
                                    if (TouchListenerList[i].TouchRect.Contains(Input.mousePosition))
                                    {
                                        float distance = Vector3.Distance(Input.mousePosition, m_PreviousMousePositionForTouchPhase);
                                        if (distance <= m_TouchPhaseMouseBuffer)
                                        {
                                            if (TouchListenerList[i].OnTouch != null)
                                            {
                                                TouchListenerList[i].OnTouch(TouchListenerList[i].TouchRect, Input.mousePosition, TouchListenerList[i].TouchPhase);
                                            }
                                        }
                                    }
                                }

                                m_PreviousMousePositionForTouchPhase = Input.mousePosition;
                                break;
                        }
                    }
                }
                else
                {
                    //If it's a touch device check if the touch position was within the desired rect
                    if (Input.touchCount > 0)
                    {
                        //See if we need to check for a certain touch phase
                        if (TouchListenerList[i].TouchPhase == -1)
                        {
                            if (TouchListenerList[i].TouchRect.Contains(Input.touches[0].position))
                            {
                                if (TouchListenerList[i].OnTouch != null)
                                {
                                    TouchListenerList[i].OnTouch(TouchListenerList[i].TouchRect, Input.touches[0].position, TouchListenerList[i].TouchPhase);
                                }
                            }
                        }
                        else
                        {
                            if ((TouchPhase)TouchListenerList[i].TouchPhase == Input.touches[0].phase)
                            {
                                if (TouchListenerList[i].TouchRect.Contains(Input.touches[0].position))
                                {
                                    if (TouchListenerList[i].OnTouch != null)
                                    {
                                        TouchListenerList[i].OnTouch(TouchListenerList[i].TouchRect, Input.touches[0].position, TouchListenerList[i].TouchPhase);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if (DragListenerList != null)
        {
            //Iterate through all items in the register list
            for (int i = 0; i < DragListenerList.Count; i++)
            {
                //Check if it's a touch device or Mouse/Keyboard device
                if (!IsTouchDevice())
                {
                    //Check if the mouse click was within the desired rect
                    if (Input.GetMouseButton(0))
                    {
                        if (DragListenerList[i].DragRect.Contains(Input.mousePosition))
                        {
                            if ((DragListenerList[i].CachedPos - Input.mousePosition) != Vector3.zero)
                            {
                                float diff = Vector3.Distance(DragListenerList[i].CachedPos, Input.mousePosition);
                                if (DragListenerList[i].OnDrag != null)
                                {
                                    DragListenerList[i].OnDrag(diff, DragListenerList[i].CachedPos, Input.mousePosition);
                                }
                            }
                        }
                    }

                    DragListenerList[i].CachedPos = Input.mousePosition;
                }
                else
                {
                    //If it's a touch device check if the touch position was within the desired rect
                    if (Input.touchCount > 0)
                    {
                        //See if we need to check for a certain touch phase
                        if (DragListenerList[i].DragRect.Contains(Input.touches[0].position))
                        {
                            if (Input.touches[0].phase == TouchPhase.Moved)
                            {
                                float diff = Vector3.Distance(DragListenerList[i].CachedPos, Input.mousePosition);
                                if (diff > 0)
                                {
                                    if (DragListenerList[i].OnDrag != null)
                                    {
                                        DragListenerList[i].OnDrag(diff, DragListenerList[i].CachedPos, Input.mousePosition);
                                    }
                                }
                            }
                        }

                        DragListenerList[i].CachedPos = Input.touches[0].position;
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
    }

    private void OnDestroy()
    {
        RemoveAllKeyListeners();
        RemoveAllTouchListeners();
    }

    #region Key Event Register functions
    /// <summary>
    /// Add a listener for a key press
    /// </summary>
    /// <param name="a_KeyCode">Key to listen for</param>
    /// <param name="a_KeyState">Key state to listen for</param>
    /// <param name="a_Callback">Callback to fire</param>
    public void AddKeyListener(KeyCode a_KeyCode, eKeyState a_KeyState, Action<KeyCode, eKeyState> a_Callback)
    {
        if (KeyListenerList != null)
        {
            InputRegister reg = null;
            reg = KeyListenerList.Find(r => r.Keycode == a_KeyCode);
            if (reg == null)
                KeyListenerList.Add(new InputRegister(a_KeyCode, a_KeyState, a_Callback));
        }
    }

    /// <summary>
    /// Remove the listener for a key
    /// </summary>
    /// <param name="a_KeyCode">Key to no longer listen for</param>
    public void RemoveKeyListener(KeyCode a_KeyCode)
    {
        if (KeyListenerList != null)
        {
            InputRegister reg = null;
            reg = KeyListenerList.Find(r => r.Keycode == a_KeyCode);
            if (reg != null)
            {
                reg = null;
            }
            KeyListenerList.RemoveAll(r => r.Keycode == a_KeyCode);
        }
    }

    /// <summary>
    /// Remove all key listeners
    /// </summary>
    public void RemoveAllKeyListeners()
    {
        if (KeyListenerList != null && KeyListenerList.Count > 0)
        {
            for (int i = 0; i < KeyListenerList.Count; i++)
            {
                KeyListenerList[i] = null;
            }
            KeyListenerList.Clear();
        }
    }
    #endregion

    #region Touch Event Register Functions
    /// <summary>
    /// Add a listener for any touch event on the whole screen. Only registers the first touch
    /// </summary>
    /// <param name="a_Callback">Callback to fire</param>
    public void AddFullScreenTouchListener(Action<Rect, Vector2, int> a_Callback)
    {
        if (TouchListenerList != null)
        {
            Rect touchRect = new Rect(0f, 0f, Screen.width, Screen.height);
            InputRegister reg = null;
            reg = TouchListenerList.Find(r => r.TouchRect == touchRect);
            if (reg == null)
                TouchListenerList.Add(new InputRegister(touchRect, a_Callback));
        }
    }

    /// <summary>
    /// Add a listener for any touch event within a certain section of the screen. 
    /// Only registers the first touch
    /// </summary>
    /// <param name="a_Rect">Rectangle to check touch within</param>
    /// <param name="a_Callback">Callback to fire</param>
    public void AddTouchListener(Rect a_Rect, Action<Rect, Vector2, int> a_Callback)
    {
        if (TouchListenerList != null)
        {
            Rect touchRect = a_Rect;
            InputRegister reg = null;
            reg = TouchListenerList.Find(r => r.TouchRect == touchRect);
            if (reg == null)
                TouchListenerList.Add(new InputRegister(touchRect, a_Callback));
        }
    }

    /// <summary>
    /// Add a listener for any touch event within a certain section of the screen at a certain touch phase.
    /// Only registers the first touch
    /// </summary>
    /// <param name="a_Rect">Rectangle to check touch within</param>
    /// <param name="a_TouchPhase">Touch Phase to monitor</param>
    /// <param name="a_Callback">Callback to fire</param>
    public void AddTouchListener(Rect a_Rect, TouchPhase a_TouchPhase, Action<Rect, Vector2, int> a_Callback)
    {
        if (TouchListenerList != null)
        {
            Rect touchRect = a_Rect;
            TouchPhase touchPhase = a_TouchPhase;
            InputRegister reg = null;
            reg = TouchListenerList.Find(r => r.TouchRect == touchRect && r.TouchPhase == (int)touchPhase);
            if (reg == null)
                TouchListenerList.Add(new InputRegister(touchRect, touchPhase, a_Callback));
        }
    }

    /// <summary>
    /// Remove the listener for the full screen Touch region
    /// </summary>
    public void RemoveFullScreenTouchListener()
    {
        if (TouchListenerList != null)
        {
            Rect touchRect = new Rect(0f, 0f, Screen.width, Screen.height);
            InputRegister reg = null;
            reg = TouchListenerList.Find(r => r.TouchRect == touchRect);
            if (reg != null)
            {
                reg = null;
            }
            TouchListenerList.RemoveAll(r => r.TouchRect == touchRect);
        }
    }

    /// <summary>
    /// Remove the listener for a Touch region
    /// </summary>
    /// <param name="a_KeyCode">Touch region to no longer listen for</param>
    public void RemoveTouchListener(Rect a_TouchRegion)
    {
        if (TouchListenerList != null)
        {
            Rect touchRect = a_TouchRegion;
            InputRegister reg = null;
            reg = TouchListenerList.Find(r => r.TouchRect == touchRect);
            if (reg != null)
            {
                reg = null;
            }
            TouchListenerList.RemoveAll(r => r.TouchRect == touchRect);
        }
    }

    /// <summary>
    /// Remove the listener for a Touch region and a Touch Phase
    /// </summary>
    /// <param name="a_TouchRegion">Touch region to no longer listen for</param>
    /// <param name="a_TouchPhase">Touch Phase of the registered region</param>
    public void RemoveTouchListener(Rect a_TouchRegion, TouchPhase a_TouchPhase)
    {
        if (TouchListenerList != null)
        {
            Rect touchRect = a_TouchRegion;
            TouchPhase touchPhase = a_TouchPhase;
            InputRegister reg = null;
            reg = TouchListenerList.Find(r => r.TouchRect == touchRect && r.TouchPhase == (int)touchPhase);
            if (reg != null)
            {
                reg = null;
            }
            TouchListenerList.RemoveAll(r => r.TouchRect == touchRect && r.TouchPhase == (int)touchPhase);
        }
    }

    /// <summary>
    /// Remove all Touch listeners
    /// </summary>
    public void RemoveAllTouchListeners()
    {
        if (TouchListenerList != null && TouchListenerList.Count > 0)
        {
            for (int i = 0; i < TouchListenerList.Count; i++)
            {
                TouchListenerList[i] = null;
            }
            TouchListenerList.Clear();
        }
    }
    #endregion

    #region Drag Event Register Functions
    public void AddFullScreenDragListener(Action<float, Vector3, Vector3> a_Callback)
    {
        if (DragListenerList != null)
        {
            Rect dragRect = new Rect(0f, 0f, Screen.width, Screen.height);
            InputRegister reg = null;
            reg = DragListenerList.Find(r => r.DragRect == dragRect);
            if (reg == null)
                DragListenerList.Add(new InputRegister(dragRect, a_Callback));
        }
    }

    public void AddDragListener(Rect a_Rect, Action<float, Vector3, Vector3> a_Callback)
    {
        if (DragListenerList != null)
        {
            Rect dragRect = a_Rect;
            InputRegister reg = null;
            reg = DragListenerList.Find(r => r.DragRect == dragRect);
            if (reg == null)
                DragListenerList.Add(new InputRegister(dragRect, a_Callback));
        }
    }

    public void RemoveFullScreenDragListener()
    {
        if (DragListenerList != null)
        {
            Rect dragRect = new Rect(0f, 0f, Screen.width, Screen.height);
            InputRegister reg = null;
            reg = DragListenerList.Find(r => r.DragRect == dragRect);
            if (reg != null)
            {
                reg = null;
            }
            DragListenerList.RemoveAll(r => r.DragRect == dragRect);
        }
    }

    public void RemoveDragListener(Rect a_DragRegion)
    {
        if (DragListenerList != null)
        {
            Rect dragRect = a_DragRegion;
            InputRegister reg = null;
            reg = DragListenerList.Find(r => r.DragRect == dragRect);
            if (reg != null)
            {
                reg = null;
            }
            DragListenerList.RemoveAll(r => r.DragRect == dragRect);
        }
    }

    public void RemoveAllDragListeners()
    {
        if (DragListenerList != null && DragListenerList.Count > 0)
        {
            for (int i = 0; i < DragListenerList.Count; i++)
            {
                DragListenerList[i] = null;
            }
            DragListenerList.Clear();
        }
    }
    #endregion

    /// <summary>
    /// Checks if the device uses touch inputs or mouse and keyboard
    /// </summary>
    /// <returns></returns>
    public bool IsTouchDevice()
    {
#if UNITY_EDITOR
        return false;
#elif UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE || UNITY_WP8 || UNITY_AMAZON
        return true;
#else
        return false;
#endif
    }

    #region Debug
    //Debug
    private void OnAPressed(KeyCode a_KeyCode, eKeyState a_KeyState)
    {
        Debug.Log("A Down");
    }

    private void OnWPressed(KeyCode a_KeyCode, eKeyState a_KeyState)
    {
        Debug.Log("W Hold");
    }

    private void OnDPressed(KeyCode a_KeyCode, eKeyState a_KeyState)
    {
        Debug.Log("D Up");
    }

    private void OnFullScreenTouch(Rect a_TouchRect, Vector2 a_TouchPosition, int a_TouchPhase)
    {
        Debug.Log("Touch");
    }

    private void OnDrag(float a_Diff, Vector3 a_PreviousPosition, Vector3 a_CurrentPosition)
    {
        Debug.Log("Drag: " + a_Diff);
    }
    //-----
    #endregion
}
