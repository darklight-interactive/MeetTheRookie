using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class PauseButton : Button
{
    public static BindingId keyPropertyKey = nameof(key);

    [UxmlAttribute]
    public string key;

    public PauseButton()
    {
        this.schedule.Execute(() =>
        {
            if (!string.IsNullOrEmpty(key))
            {
                this.text = $"Inky > {key}";
                //LocalizationManager.Instance.GetLocalizedValue(key);
            }
        });
    }
}