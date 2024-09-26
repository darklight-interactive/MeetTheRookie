using System;
using Darklight.UnityExt.Editor;
using UnityEngine;

public partial class MTRInteractable
{
    [Serializable]
    public new class InternalData : InternalData<MTRInteractable>
    {
        [SerializeField, ShowOnly] string _name;
        [SerializeField, ShowOnly] string _key;
        [SerializeField, ShowOnly] string _layer;
        [SerializeField, ShowOnly] Type _type;
        [SerializeField] Sprite _sprite;

        public override string Name => _name;
        public override string Key => _key;
        public override string Layer => _layer;
        public Type Type => _type;
        public Sprite Sprite => _sprite;

        public InternalData(MTRInteractable interactable) : base(interactable) { }

        public override void LoadData(MTRInteractable interactable)
        {
            _name = DEFAULT_NAME;
            _key = interactable._interactionStitch;
            _layer = DEFAULT_LAYER;
            _type = DetermineType(interactable);

            // << SET THE INITIAL SPRITE >> ------------------------------------
            // If the data contains a sprite, assign it to the sprite renderer
            if (_sprite != null)
                interactable.spriteRenderer.sprite = _sprite;
            // Else if the sprite renderer has a sprite, assign it to the data
            else if (interactable.spriteRenderer.sprite != null)
                _sprite = interactable.spriteRenderer.sprite;
        }

        Type DetermineType(MTRInteractable interactable)
        {
            return Type.INTERACTABLE;
        }
    }
}