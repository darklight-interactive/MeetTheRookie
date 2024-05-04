using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MenuButton : Button
{
    [UxmlAttribute]
    public string key;

    public MenuButton()
    {
        this.schedule.Execute(() =>
        {
            if (!string.IsNullOrEmpty(key))
            {
                //LocalizationManager.Instance.GetLocalizedValue(key);
            }
        });
    }
}