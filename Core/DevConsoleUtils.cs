using System;
using System.Reflection;
using System.Text;

namespace DeveloperConsole
{
    public static class DevConsoleUtils
    {
        public static string GenerateUsage(DevCmdAttribute cmdAttribute, ParameterInfo[] argsInfo)
        {
            var usageSb = new StringBuilder(cmdAttribute.Name).Append('(');
            for (var i = 0; i < argsInfo.Length; i++)
            {
                var info = argsInfo[i];
                if (info.GetCustomAttribute<DevCmdArgInjectAttribute>() == null)
                {
                    usageSb.Append(info.ParameterType.Name).Append(' ').Append(info.Name);
                    if (info.HasDefaultValue)
                    {
                        usageSb.Append('=').Append('"').Append(info.DefaultValue).Append('"');
                    }

                    if (i < argsInfo.Length - 1)
                    {
                        usageSb.Append(", ");
                    }
                }
            }

            usageSb.Append(')');
            return usageSb.ToString();
        }

        public static string GetInjectedArgName(ParameterInfo argInfo)
        {
            return $"_i_{argInfo.Name}";
        }

        public static string InjectDependencies(string cmd, ParameterInfo[] methodArgs)
        {
            var (_, tale) = ChopCommandName(cmd);
            var inputArgsCount = tale.Split(
                new[] { ',', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries
            ).Length;

            // NOTE: Setting current function.

            var openParenthesisIndex = cmd.IndexOf('(');
            var hasParenthesis = openParenthesisIndex > 0;

            var sb = new StringBuilder();
            if (hasParenthesis)
            {
                sb.Append(cmd[..(openParenthesisIndex + 1)]);
            }
            else
            {
                sb.Append(cmd).Append('(');
            }

            var injectedArgsCount = 0;
            for (var i = 0; i < methodArgs.Length; i++)
            {
                var argInfo = methodArgs[i];
                if (argInfo.GetCustomAttribute<DevCmdArgInjectAttribute>() != null)
                {
                    if (i != 0) sb.Append(", ");
                    var injectedArgName = GetInjectedArgName(argInfo);
                    sb.Append(injectedArgName);
                    injectedArgsCount++;
                }
            }

            if (injectedArgsCount > 0 && inputArgsCount > 0) sb.Append(", ");

            if (hasParenthesis)
            {
                sb.Append(cmd[(openParenthesisIndex + 1)..]);
            }
            else
            {
                sb.Append(')');
            }

            cmd = sb.ToString();
            return cmd;
        }

        public static (string name, string tale) ChopCommandName(string cmd)
        {
            var spaceIndex = cmd.IndexOf('(');
            return spaceIndex >= 0
                ? (cmd[..spaceIndex].Trim(), cmd[spaceIndex..].Trim())
                : (cmd, "");
        }
    }
}
