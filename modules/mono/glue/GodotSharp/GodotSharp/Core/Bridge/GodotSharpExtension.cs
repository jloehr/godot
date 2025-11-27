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
            GD.Print(System.Environment.CurrentDirectory);
            GD.Print(string.Join(System.Environment.NewLine, AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.FullName)));

            Assembly assembly = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault (x => x.GetName().Name == "GodotSharpExtension");

            try
            {
                if (assembly == null)
                {
                    GD.Print("Assembly.Load");
                    assembly = Assembly.Load("GodotSharpExtension");
                }
                else
                {
                    GD.Print("Assembly already loaded");
                }
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

