using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class OnEnableInvoke : MonoBehaviour
{
    public UnityEvent onEnable, onDisable;
    public bool runInEditor;
    private void OnEnable()
    {
        if(runInEditor || Application.isPlaying)
            onEnable.Invoke();
    }
    private void OnDisable()
    {
        if(runInEditor || Application.isPlaying)
            onDisable.Invoke();
    }
}