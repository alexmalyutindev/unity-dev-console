using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeveloperConsole
{
    public class DevConsoleView : IDisposable
    {
        private const string CommandInputFieldId = "CommandInputField";
        private Rect _windowRect = new(0, 0, 800, 600);
        private Vector2 _scroll;
        private string _input;

        private bool _enabled;
        private readonly GUISkin _skin;

        private bool _commandInputFocussed;
        private readonly List<LogEntry> _logs = new();

        private readonly DevConsole _devConsole;

        public DevConsoleView(DevConsole devConsole)
        {
            _devConsole = devConsole;
            _skin = Resources.Load<GUISkin>("console-skin");
            Application.logMessageReceived += OnLogCallback;

            // https://www.asciiart.eu/text-to-ascii-art
            // Font:
            // - StickLetters
            // - Big
            // - Standart
            var logo =
                @"   ___             _____                   __   " + '\n' +
                @"  / _ \___ _  __  / ___/__  ___  ___ ___  / /__ " + '\n' +
                @" / // / -_) |/ / / /__/ _ \/ _ \(_-</ _ \/ / -_)" + '\n' +
                @"/____/\__/|___/  \___/\___/_//_/___/\___/_/\__/ " + DevConsole.Version;
            AddLogEntry(logo, string.Empty, LogTypeInternal.Command);
        }

        public void OnGUI()
        {
            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.F12)
            {
                _enabled = !_enabled;
                _commandInputFocussed = false;
                return;
            }

            if (!_enabled) return;

            GUI.skin = _skin;
            var scale = 1.5f; // Screen.height / 720.0f;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            _windowRect = GUI.Window(0, _windowRect, Window, "Console");
        }

        public void Dispose()
        {
            Application.logMessageReceived -= OnLogCallback;
        }

        private void OnLogCallback(string message, string trace, LogType type)
        {
            AddLogEntry(message, trace, (LogTypeInternal) type);
        }

        private void AddLogEntry(string message, string trace, LogTypeInternal type)
        {
            _logs.Add(
                new LogEntry
                {
                    Message = message,
                    Stack = trace,
                    Type = type,
                    TimeStamp = DateTime.Now,
                    GuiContent = new GUIContent(message),
                }
            );
            _scroll.y += 100 * message.Split("\n").Length;
        }

        private void Window(int id)
        {
            if (Event.current.keyCode == KeyCode.Return && !string.IsNullOrWhiteSpace(_input))
            {
                var input = _input;
                _input = string.Empty;
                ProcessCommand(input);
            }

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Tab)
            {
                if (GUI.GetNameOfFocusedControl() == CommandInputFieldId)
                {
                    var similar = _devConsole.Commands
                        .Where(command => command.Name.StartsWith(_input))
                        .ToArray();

                    if (similar.Length == 1)
                    {
                        var matchedCommandName = similar[0].Name;
                        _input = matchedCommandName;
                        TextEditor stateObject = (TextEditor) GUIUtility.GetStateObject(
                            typeof(TextEditor),
                            GUIUtility.keyboardControl
                        );
                        stateObject.text = _input;
                        stateObject.MoveLineEnd();
                    }
                    else if (similar.Length > 1)
                    {
                        // TODO: Autocomplete dropdown
                    }
                }
            }

            var topPanelRect = new Rect(0, 1, _windowRect.width - 1, 18);
            using (new GUILayout.AreaScope(topPanelRect))
            {
                using var _ = new GUILayout.HorizontalScope();
                GUILayout.Space(125);
                TopPanel(topPanelRect);
            }

            GUILayout.Space(5);
            MainWindow();

            var dragZone = new Rect(0, 0, _windowRect.width, 30);
            GUI.DragWindow(dragZone);
        }

        private void TopPanel(Rect rect)
        {
            if (GUILayout.Button("clear")) _logs.Clear();
            if (GUILayout.Button("focus")) _scroll.y += 10000;
            if (GUILayout.Button("warning")) Debug.LogWarning("warning");
            if (GUILayout.Button("error")) Debug.LogError("error");
            if (GUILayout.Button("dropdown"))
            {
                // TODO: Dropdown!
            }

            GUILayout.FlexibleSpace();

            const string xSymbol = "\u00d7";
            if (GUILayout.Button(xSymbol, GUILayout.Width(rect.height)))
            {
                _enabled = false;
            }
        }

        private void MainWindow()
        {
            using (var scrollViewScope = new GUILayout.ScrollViewScope(
                       _scroll,
                       false,
                       true,
                       _skin.verticalScrollbar,
                       _skin.verticalScrollbar
                   ))
            {
                _scroll = scrollViewScope.scrollPosition;
                foreach (var log in _logs)
                {
                    GUI.color = log.Type switch
                    {
                        LogTypeInternal.Error or LogTypeInternal.Exception or LogTypeInternal.Assert =>
                            new Color(1.0f, 0.02f, 0.0f, 1.0f),
                        LogTypeInternal.Warning => new Color(1.0f, 0.4f, 0.0f, 1.0f),
                        LogTypeInternal.Command => new Color(0.3f, 1.0f, 0.3f, 1.0f),
                        _ => Color.white,
                    };
                    GUILayout.Label(log.GuiContent);
                }

                GUI.color = Color.white;
            }

            GUI.SetNextControlName(CommandInputFieldId);
            _input = GUILayout.TextField(_input);

            if (!_commandInputFocussed)
            {
                GUI.FocusControl(CommandInputFieldId);
                _commandInputFocussed = true;
            }
        }

        private void ProcessCommand(string input)
        {
            AddLogEntry($">{input}", string.Empty, LogTypeInternal.Command);
            _devConsole.Execute(input);
        }

        private struct LogEntry
        {
            public LogTypeInternal Type;
            public string Message;
            public string Stack;
            public GUIContent GuiContent;
            public DateTime TimeStamp;
        }

        public enum LogTypeInternal
        {
            Error = LogType.Error,
            Assert = LogType.Assert,
            Warning = LogType.Warning,
            Log = LogType.Log,
            Exception = LogType.Exception,
            Command,
        }
    }
}
