using System;
using UnityEngine;

public class MecAnimEventTrigger : MonoBehaviour {

    public event Action<string> OnCustomEventTrigger;
    
    public void CustomEvent(string eventName)
    {
        OnCustomEventTrigger(eventName);
    }
}
