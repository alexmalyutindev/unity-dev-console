using UnityEngine;

namespace DeveloperConsole
{
    [ExecuteAlways]
    public class DevConsoleRenderer : MonoBehaviour
    {
        public DevConsole DevConsole => _devConsole;
        
        private DevConsoleView _view;
        private DevConsole _devConsole;

        private void Start()
        {
            _devConsole = new DevConsole();
            _view = new DevConsoleView(_devConsole);
        }

        private void OnGUI()
        {
            _view?.OnGUI();
        }

        private void OnDestroy()
        {
            _view?.Dispose();
        }
    }
}
