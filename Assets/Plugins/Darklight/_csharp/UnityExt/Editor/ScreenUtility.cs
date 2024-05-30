using UnityEngine;

namespace Darklight.UnityExt.Editor
{
    public static class ScreenUtility
    {
        public static Vector2 GameViewSize => GetScreenSize();
        public static float ScreenAspectRatio => GetScreenAspectRatio();

        /// <summary>
        /// Retrieves the size of the main game view.
        /// </summary>
        /// <returns>A Vector2 representing the size of the main game view.</returns>
        public static Vector2 GetScreenSize()
        {
#if UNITY_EDITOR
            return GetGameViewSizeInEditor();
#else
            return new Vector2(Screen.width, Screen.height);
#endif
        }

        private static Vector2 GetGameViewSizeInEditor()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod(
                "GetSizeOfMainGameView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }

        /// <summary>
        /// Retrieves the aspect ratio value of the screen.
        /// </summary>
        /// <returns>The aspect ratio value of the screen.</returns>
        public static float GetScreenAspectRatio()
        {
            Vector2 gameViewSize = GetScreenSize();
            return gameViewSize.x / gameViewSize.y;
        }
    }
}
