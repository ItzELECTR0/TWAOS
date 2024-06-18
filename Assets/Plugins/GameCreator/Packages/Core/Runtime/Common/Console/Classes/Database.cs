using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameCreator.Runtime.Console
{
    internal static class Database
    {
        [field: NonSerialized]
        private static Dictionary<PropertyName, Command> Commands { get; set; }
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public static Dictionary<PropertyName, Command> Get
        {
            get
            {
                if (Commands == null)
                {
                    Commands = new Dictionary<PropertyName, Command>();
                    Type type = typeof(Command);
                    
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    List<Type> types = new List<Type>();

                    foreach (Assembly assembly in assemblies)
                    {
                        Type[] assemblyTypes = assembly.GetTypes();
                        foreach (Type assemblyType in assemblyTypes)
                        {
                            if (assemblyType.IsInterface) continue;
                            if (assemblyType.IsAbstract) continue;
                            if (type.IsAssignableFrom(assemblyType)) types.Add(assemblyType);   
                        }
                    }

                    Type[] commandsList = types.ToArray();
                    foreach (Type commandType in commandsList)
                    {
                        Command instance = Activator.CreateInstance(commandType) as Command;
                        if (instance == null) continue;
                        
                        Commands[instance.Name] = instance;
                    }
                }
                
                return Commands;
            }
        }
    }
}