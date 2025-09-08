using System.Collections;
using System.Collections.Generic;

using LostOasis;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Visuals;

[DefaultExecutionOrder(-1)]
public class ScreenManager : Singleton<ScreenManager>
{
    public string activeScreenID = null;
    private string prevActiveID = null;
    public ScreenControl mainScreenControl, gameplayControl;
    [HideInInspector] public ScreenControl activeBox;
    public ScreenBools screenBool;

    private static Dictionary<string, ScreenControl> controls;
    private static Dictionary<string, PageTransition> transitions;

    public static bool FreezePagesEscape { get; set; }

    private void Awake()
    {
        controls = new();
        transitions = new Dictionary<string, PageTransition>();
        var list = GameObject.FindObjectsByType<ScreenControl>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach(var l in list)
        {
            if (string.IsNullOrEmpty(l.label)) continue;
            controls.Add(l.label.MegaTrim(), l);
        }

        var tList = GameObject.FindObjectsByType<PageTransition>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach(var t in tList)
        {
            transitions.Add(t.label, t);
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

    public static PageTransition GetTransition(string name)
    {
        return transitions[name];
    }

    public static void Transition(string name, string page)
    {
        GetTransition(name).SetActive(page);
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
