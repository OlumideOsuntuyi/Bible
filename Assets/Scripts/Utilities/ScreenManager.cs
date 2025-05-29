using System.Collections;
using System.Collections.Generic;

using LostOasis;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class ScreenManager : Singleton<ScreenManager>
{
    public string activeScreenID = null;
    private string prevActiveID = null;
    public ScreenControl mainScreenControl, gameplayControl;
    [HideInInspector] public ScreenControl activeBox;
    public ScreenBools screenBool;

    private static Dictionary<string, ScreenControl> controls;
    private static Dictionary<string, ScreenControl> transitions;

    public static bool FreezePagesEscape { get; set; }

    private void Awake()
    {
        controls = new();
        var list = GameObject.FindObjectsByType<ScreenControl>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach(var l in list)
        {
            if (string.IsNullOrEmpty(l.label)) continue;
            controls.Add(l.label.MegaTrim(), l);
        }
    }

    public static ScreenControl Get(string name)
    {
        return controls[name.MegaTrim()];
    }

    public static void Set(string name, string page)
    {
        Get(name).SetActiveBox(page);
    }
    public static void Set(string name, int page)
    {
        Get(name).SetActiveBox(page);
    }

    private void Update()
    {
        if(prevActiveID != activeScreenID)
        {
            prevActiveID = activeScreenID;
        }

        if (!FreezePagesEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
    }
    public void SetWorldCam(bool activate)
    {
        screenBool.canMoveWorldCamera = activate;
    }
    public void SwitchMainScreen(bool activate)
    {
        SetWorldCam(activate);
    }
    public void SwitchIfBarIsZero(ScreenControl box)
    {
        SwitchMainScreen(box.active_box == 0 && mainScreenControl.active_box == 0);
    }
    public void Return()
    {
        if(activeBox != null)
        {
            activeBox.SetActiveBox(0);
            if(activeBox == activeBox.parent)
            {
                activeBox.SetActiveBox(0);
            }
            else
            {
                if (activeBox.changeParentIndex)
                {
                    activeBox.parent.active_box = activeBox.parentIndex;
                }
                activeBox = activeBox.parent;
            }
            //SwitchMainScreen(mainScreenControl.active_box is 0 && overlayControl.active_box is 0);
        }
    }
    [System.Serializable]
    public class ErrorUI
    {
        public TMP_Text errorMessage;
        public GameObject gameObject;
        public int index;
    }
    [System.Serializable]
    public struct ScreenBools
    {
        public bool canMoveWorldCamera;
    }
}
public enum ScreenState { }
