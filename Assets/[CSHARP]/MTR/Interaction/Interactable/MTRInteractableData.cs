using System;
using Darklight.UnityExt.Editor;
using UnityEngine;

public partial class MTRInteractable
{
    [Serializable]
    public new class InternalData : InternalData<MTRInteractable>
    {
        [SerializeField, ShowOnly] string _name;
        [SerializeField, ShowOnly] string _scene;
        [SerializeField, ShowOnly] string _key;
        [SerializeField, ShowOnly] string _layer;
        [SerializeField, ShowOnly] Type _type = Type.BASE;
        [SerializeField] Sprite _sprite;

        public override string Name => _name;
        public override string Key => _key;
        public override string Layer => _layer;

        public string SceneKnot => _scene;
        public Type Type => _type;
        public Sprite Sprite => _sprite;

        public InternalData(MTRInteractable interactable) : base(interactable)
        {
            LoadData(interactable);
        }

        /// <summary>
        /// Set the name of the interactable
        /// </summary>
        public void SetName(string name) => _name = name;

        /// <summary>
        /// Set the key value of the interactable. <br/>
        /// This is the value that will be used to identify the interactable in the story. <br/>
        /// If the interactable is a character, the key will be the character's name. <br/>
        /// Otherwise, the key will be the interactable's corresponding stitch.
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(string key) => _key = key;

        public override void LoadData(MTRInteractable interactable)
        {
            _scene = interactable._sceneKnot;
            _key = interactable._interactionStitch;

            _name = _key.Replace($"{_scene}.", "");
            _type = interactable.TypeKey;

            if (interactable is MTRPlayerInteractor)
            {
                _key = (interactable as MTRPlayerInteractor).SpeakerTag.ToString();
                _layer = InteractionSystem.Settings.PlayerLayer;

                interactable.gameObject.name = $"PLAYER_Lupe";
            }
            else if (interactable is MTRCharacterInteractable)
            {
                _key = (interactable as MTRCharacterInteractable).SpeakerTag.ToString();
                _layer = InteractionSystem.Settings.NPCLayer;

                string name = _key.Replace("Speaker.", "");
                interactable.gameObject.name = $"CHARACTER_{name}";
            }
            else if (interactable is MTRInteractable)
            {
                _key = interactable._interactionStitch;
                _layer = InteractionSystem.Settings.InteractableLayer;

                interactable.gameObject.name = $"INTRCT_{_key}";
            }
            else
            {
                _layer = DEFAULT_LAYER;
            }

            _type = interactable.TypeKey;
            interactable.gameObject.layer = LayerMask.NameToLayer(_layer);

            // << SET THE INITIAL SPRITE >> ------------------------------------
            // If the data contains a sprite, assign it to the sprite renderer
            if (_sprite != null)
                interactable.spriteRenderer.sprite = _sprite;
            // Else if the sprite renderer has a sprite, assign it to the data
            else if (interactable.spriteRenderer.sprite != null)
                _sprite = interactable.spriteRenderer.sprite;
        }
    }
}