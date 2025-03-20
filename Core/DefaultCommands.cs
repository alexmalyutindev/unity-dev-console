using System.Collections;
using UnityEngine;

namespace DeveloperConsole
{
    [RequireComponent(typeof(DevConsoleRenderer))]
    public class DefaultCommands : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return null;
            var devConsoleRenderer = GetComponent<DevConsoleRenderer>();
            var console = devConsoleRenderer.DevConsole;
            console.Register(FPS);
            console.Register<string>(Echo);
            console.Register(Frog);
        }

        [DevCmd(name: "fps", help: "prints current frame fps")]
        private static void FPS()
        {
            Debug.Log($"fps: {1.0f / Time.deltaTime:F2}; time: {Time.deltaTime * 1000.0f}ms");
        }


        [DevCmd(name: "Echo", help: "write argument to the debug console")]
        private static void Echo(string message) => Debug.Log(message);

        [DevCmd(name: "frog", help: "frog")]
        private static void Frog()
        {
            Debug.Log(
                @"              _   " + "\n" +
                @"  __   ___.--'_`. " + "\n" +
                @" ( _`.'. -   'o` )" + "\n" +
                @" _\.'_'      _.-' " + "\n" +
                @"( \`. )    //\`   " + "\n" +
                @" \_`-'`---'\\__,  " + "\n" +
                @"  \`        `-\   " + "\n" +
                @"   `"
            );
        }
    }
}
