using UnityEngine;

namespace F
{
    public static class ExitGame
    {
        public static void QuitGame()
        { 
            #if UNITY_EDITOR

            UnityEditor.EditorApplication.isPlaying = false;
            #else

            Application.Quit();
            #endif
        }
    }
}
