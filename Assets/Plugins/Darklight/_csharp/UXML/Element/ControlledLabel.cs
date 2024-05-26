using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UXML.Element
{
    #region ---- [[ CONTROLLED LABEL ]] ----
    [UxmlElement]
    public partial class ControlledLabel : Label
    {
        public class ControlledLabelFactory : UxmlFactory<ControlledLabel> { }

        [UxmlAttribute]
        public float fontSize
        {
            get { return style.fontSize.value.value; }
            set { style.fontSize = value; }
        }
    }
    #endregion
}
