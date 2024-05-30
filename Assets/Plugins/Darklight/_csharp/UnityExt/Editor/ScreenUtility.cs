using UnityEngine;

namespace Darklight.UnityExt.Editor
{
    public static class ScreenUtility
    {
        private static Vector2Int _screenSize;

        public static Vector2 ScreenSize
        {
            get
            {
                return GetMainGameViewSize();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            UpdateScreenSize();
        }

        private static void UpdateScreenSize()
        {
            Display mainDisplay = Display.main;
            _screenSize = new Vector2Int(mainDisplay.systemWidth, mainDisplay.systemHeight);
        }

        public static Vector2 GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod(
                "GetSizeOfMainGameView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }
    }
}
