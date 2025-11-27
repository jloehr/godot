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
            GD.Print("TryLoadExtensionAssembly");

            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load("GodotSharpExtension");
            }
            catch(Exception ex)
            {
                // Expected if the project doesn't have any generated extension dll.
                GD.Print("TryLoadExtensionAssembly - Unable to load assembly");
                GD.Print(ex);
                return;
            }

            var populateConstructorMethod = assembly
                    .GetType("Godot.ExtensionConstructors")?
                    .GetMethod("AddExtensionConstructors",
                        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            if (populateConstructorMethod == null)
            {
                throw new MissingMethodException("Godot.ExtensionConstructors",
                    "AddExtensionConstructors");
            }

            GD.Print("TryLoadExtensionAssembly - Involing populateConstructorMethod");
            populateConstructorMethod?.Invoke(null, null);

            // ToDo: ScriptManagerBridge.LookupScriptsInAssembly(projectAssembly);
        }

        public static void  UnloadExtensionAssembly()
        {
            GD.Print("UnloadExtensionAssembly");
            Constructors.ExtensionMethodConstructors?.Clear();
        }
    }
}

