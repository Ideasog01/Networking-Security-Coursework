using UnityEngine;
using UnityEngine.Events;

public class PlayerActions : MonoBehaviour
{
    private UnityAction _primaryAction;
    private UnityAction _ability1Action;
    private UnityAction _ability2Action;

    public UnityAction PrimaryAction
    {
        get { return _primaryAction; }
        set { _primaryAction = value; }
    }

    public UnityAction Ability1Action
    {
        get { return _ability1Action; }
        set { _ability1Action = value; }
    }

    public UnityAction Ability2Action
    {
        get { return _ability2Action; }
        set { _ability2Action = value; }
    }

    public void PrimaryActionTrigger()
    {
        if(_primaryAction != null)
        {
            _primaryAction.Invoke();
        }
    }

    public void Ability1ActionTrigger()
    {
        if(_ability1Action != null)
        {
            _ability1Action.Invoke();
        }
    }

    public void Ability2ActionTrigger()
    {
        if(_ability2Action != null)
        {
            _ability2Action.Invoke();
        }
    }
}
