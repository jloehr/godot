#nullable enable

using System;
using System.Linq;
using System.Reflection;


namespace Godot.Bridge
{
    public static class GodotSharpExtension
    {
        public static void TryLoadExtensionAssembly()
        {
            var populateConstructorMethod =
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .First(x => x.GetName().Name == "GodotSharpExtension")?
                    .GetType("Godot.ExtensionMethodConstructors")?
                    .GetMethod("AddExtensionConstructors",
                        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            // if (populateConstructorMethod == null)
            // {
            //     throw new MissingMethodException("Godot.ExtensionMethodConstructors",
            //         "AddExtensionConstructors");
            // }

            populateConstructorMethod?.Invoke(null, null);

            // ToDo: ScriptManagerBridge.LookupScriptsInAssembly(projectAssembly);
        }

        public static void  UnloadExtensionAssembly()
        {
            Constructors.ExtensionMethodConstructors?.Clear();
        }
    }
}

