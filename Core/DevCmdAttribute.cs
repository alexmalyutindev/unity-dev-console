using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DevCmdAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Help;

        public DevCmdAttribute(string name, string help)
        {
            Name = name;
            Help = help;
        }
    }
}
