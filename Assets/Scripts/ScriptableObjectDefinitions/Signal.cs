using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Signal", menuName = "Signal/new Signal")]
public class Signal : ScriptableObject
{
    private Action m_action;

    // instance
    public void Post(GameObject sender = null)
    {
        m_action?.Invoke();

#if UNITY_EDITOR
        Debug.Log($"Post {this.name}{(sender != null ? $" from sender: {sender.gameObject.name}" : "")}");
#endif
    }

    // reference
    public void Register(Action _action)
    {
        m_action += _action;
    }
    public void UnRegister(Action _action)
    {
        m_action -= _action;
    }

    // statics
    public static void Register(Signal[] _signals, Action _action)
    {
        for (int i = 0; i < _signals.Length; i++)
        {
            _signals[i].Register(_action);
        }
    }
    public static void UnRegister(Signal[] _signals, Action _action)
    {
        for (int i = 0; i < _signals.Length; i++)
        {
            _signals[i].UnRegister(_action);
        }
    }


}
