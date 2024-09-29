using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;

public partial class BaseInteractable
{

    [Serializable]
    public new class InternalData : InternalData<BaseInteractable>
    {
        [SerializeField, ShowOnly] string _name;
        [SerializeField, ShowOnly] string _key;
        [SerializeField, ShowOnly] string _layer;
        [SerializeField, ShowOnly] Type _type;

        public override string Name => _name;
        public override string Key => _key;
        public override string Layer => _layer;
        public Type Type => _type;

        public InternalData(BaseInteractable interactable, string name, string key)
            : base(interactable) { }

        public override void LoadData(BaseInteractable interactable)
        {
            _name = DEFAULT_NAME;
            _key = DEFAULT_KEY;
            _layer = DEFAULT_LAYER;
            _type = DetermineType(interactable);
        }

        Type DetermineType(BaseInteractable interactable)
        {
            return Type.INTERACTABLE;
        }
    }
}