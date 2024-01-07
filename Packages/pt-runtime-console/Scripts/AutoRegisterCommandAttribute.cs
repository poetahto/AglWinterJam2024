using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using poetools.Console.Commands;
using UnityEngine;

namespace poetools.Console
{
    /// <summary>
    /// An attribute that allows commands to be automatically registered with a console when created.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRegisterCommandAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleCommandAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Help;
        
        public ConsoleCommandAttribute(string name)
        {
            Name = name;
            Help = string.Empty;
        }
        
        public ConsoleCommandAttribute(string name, string help)
        {
            Name = name;
            Help = help;
        }
    }

    public class MethodCommand : ICommand
    {
        private readonly MethodInfo _methodInfo;
        
        public string Name { get; }
        public string Help { get; }
        public IEnumerable<string> AutoCompletions { get; }
        
        public MethodCommand(string name, string help, MethodInfo info)
        {
            _methodInfo = info;
            Name = name;
            Help = help;
            AutoCompletions = new[] { name };
        }
        
        public void Execute(string[] args, RuntimeConsole console)
        {
            ParameterInfo[] p = _methodInfo.GetParameters();

            if (args.Length < p.Length)
                return;

            object[] parameters = new object[p.Length];

            for (int i = 0; i < p.Length; i++)
            {
                if (p[i].ParameterType == typeof(string))
                    parameters[i] = args[i];
                
                else if (p[i].ParameterType == typeof(int))
                    parameters[i] = int.Parse(args[i]);
                
                else if (p[i].ParameterType == typeof(float))
                    parameters[i] = float.Parse(args[i]);
                
                else if (p[i].ParameterType == typeof(bool))
                    parameters[i] = bool.Parse(args[i]);
            }
            
            _methodInfo.Invoke(null, parameters);
        }
        
        public void Dispose()
        {
        }
    }

    public static class AutoCommandRegister
    {
        // Note: this must run after Subsystems, since that is when RuntimeConsole resets stuff.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            RuntimeConsole.OnCreate += AutoRegister;
        }

        private static void AutoRegister(RuntimeConsole.CreateEvent eventData)
        {
            CommandRegistry commandRegistry = eventData.Console.CommandRegistry;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyName[] ra = assembly.GetReferencedAssemblies();
                Assembly xa = Assembly.GetExecutingAssembly();
                
                if (ra.All(name => name.Name != xa.GetName().Name))
                    continue;
                
                foreach(Type type in assembly.GetTypes())
                {
                    if (typeof(ICommand).IsAssignableFrom(type) && type.GetCustomAttributes(typeof(AutoRegisterCommandAttribute), true).Length > 0)
                    {
                        ICommand instance = Activator.CreateInstance(type) as ICommand;
                        commandRegistry.Register(instance);
                    }

                    foreach (MethodInfo methodInfo in type.GetMethods())
                    {
                        object[] attr = methodInfo.GetCustomAttributes(typeof(ConsoleCommandAttribute), true);
                        
                        if (methodInfo.IsStatic && attr.Length > 0)
                        {
                            ConsoleCommandAttribute a = (ConsoleCommandAttribute)attr[0];
                            commandRegistry.Register(new MethodCommand(a.Name, a.Help, methodInfo));
                        }
                    }
                }
            }
        }
    }
}
