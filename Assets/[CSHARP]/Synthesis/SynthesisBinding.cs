using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SynthesisBindings/SynthesisBinding")]
public class SynthesisBinding : ScriptableObject
{
    public string value;
    public virtual void setValue(string v) {
        value = v;
    }

    public virtual object Clone() {
        return MemberwiseClone();
    }
}