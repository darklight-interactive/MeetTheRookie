using UnityEngine;

[CreateAssetMenu(menuName = "SynthesisBindings/SynthesisImage")]
public class SynthesisImage : SynthesisBinding {
    public Texture2D texture;
    public override void setValue(string v) {
        this.texture = (Texture2D)Resources.Load("Synthesis/" + v);
    }
}