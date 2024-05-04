using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Scene;
using Darklight.UnityExt.UXML;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;



[UxmlElement]
public partial class SceneChangeButton : Button
{
    public new class UxmlFactory : UxmlFactory<SceneChangeButton> { }

    [UxmlAttribute("Scene")]
    public SceneAsset scene;

    public SceneChangeButton()
    {
        this.clickable.clicked += () =>
        {
            UniversalSceneManager.Instance.GoToScene(scene.name);
        };
    }

}

