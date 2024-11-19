using System;
using System.Collections.Generic;
using System.Reflection;
using DynamicExpresso;

namespace DeveloperConsole
{
    public class Command
    {
        public readonly string Name;
        public readonly string Usage;
        public readonly string Help;

        private readonly Interpreter _interpreter;
        private readonly ParameterInfo[] _parameterInfos;

        public Command(Delegate command, Dictionary<Type, object> dependencies)
        {
            var cmdAttribute = command.Method.GetCustomAttribute<DevCmdAttribute>();
            if (cmdAttribute == null)
            {
                throw new Exception("Can't register command: command should have `DevCmdAttribute`!");
            }

            var argsInfo = command.Method.GetParameters();

            Name = cmdAttribute.Name.ToLower();
            Help = cmdAttribute.Help;
            Usage = DevConsoleUtils.GenerateUsage(cmdAttribute, argsInfo);

            var interpreter = new Interpreter();
            interpreter.SetFunction(Name, command);
            _parameterInfos = command.Method.GetParameters();
            foreach (var argInfo in _parameterInfos)
            {
                // NOTE: Injecting dependencies
                if (argInfo.GetCustomAttribute<DevCmdArgInjectAttribute>() != null)
                {
                    var injectedArgName = DevConsoleUtils.GetInjectedArgName(argInfo);
                    interpreter.SetVariable(injectedArgName, dependencies[argInfo.ParameterType]);
                }
            }

            _interpreter = interpreter;
        }

        public void Call(string cmd)
        {
            cmd = DevConsoleUtils.InjectDependencies(cmd, _parameterInfos);
            _interpreter.Eval(cmd);
        }

        public bool Match(string cmd)
        {
            var (name, _) = DevConsoleUtils.ChopCommandName(cmd);
            var nameEquals = name.Equals(Name, StringComparison.OrdinalIgnoreCase);
            return nameEquals;
        }
    }
}
