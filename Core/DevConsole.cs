using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace DeveloperConsole
{
    public class DevConsole
    {
        public const string Version = "v0.0.1";
        public IReadOnlyList<Command> Commands => _commands.AsReadOnly();
        private readonly List<Command> _commands = new();
        // TODO: Make dependencies registry!
        private readonly Dictionary<Type, object> _dependencies = new();

        public DevConsole()
        {
            _dependencies.Add(typeof(DevConsole), this);
            Register<DevConsole, string>(Help);
        }

        private void Register(Delegate command)
        {
            var cmdAttribute = command.Method.GetCustomAttribute<DevCmdAttribute>();
            if (cmdAttribute == null)
            {
                throw new Exception("Can't register command: command should have `DevCmdAttribute`!");
            }

            _commands.Add(new Command(command, _dependencies));
        }

        public void Register(Action command) => Register((Delegate) command);
        public void Register<T>(Action<T> command) => Register((Delegate) command);
        public void Register<T0, T1>(Action<T0, T1> command) => Register((Delegate) command);
        public void Register<T0, T1, T2>(Action<T0, T1, T2> command) => Register((Delegate) command);
        public void Register<T0, T1, T2, T3>(Action<T0, T1, T2, T3> command) => Register((Delegate) command);
        public void Register<T0, T1, T2, T3, T4>(Action<T0, T1, T2, T3, T4> command) => Register((Delegate) command);

        public void Register<T0, T1, T2, T3, T4, T5>(Action<T0, T1, T2, T3, T4, T5> command) =>
            Register((Delegate) command);

        public void Execute(string cmd)
        {
            try
            {
                cmd = cmd.Trim();
                foreach (var command in _commands)
                {
                    if (command.Match(cmd))
                    {
                        command.Call(cmd);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [DevCmd(name: "help", help: "prints help command")]
        private static void Help([DevCmdArgInject] DevConsole console, string commandName = "")
        {
            if (!string.IsNullOrEmpty(commandName))
            {
                var command = console.Commands.FirstOrDefault(
                    command => command.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase)
                );
                if (command != null)
                {
                    Debug.Log($"<b>{command.Usage,-40}</b>  {command.Help}");
                    return;
                }
            }

            var sb = new StringBuilder();

            for (var i = 0; i < console.Commands.Count; i++)
            {
                if (i != 0) sb.AppendLine();

                var command = console.Commands[i];
                sb.Append($"<b>{command.Usage,-40}</b>  {command.Help}");
            }

            Debug.Log(sb.ToString());
        }
    }
}
