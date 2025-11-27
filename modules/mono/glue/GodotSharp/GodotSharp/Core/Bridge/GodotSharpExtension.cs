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
            var extensionAssembly =
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault (x => x.GetName().Name == "GodotSharpExtension");

            if (extensionAssembly == null)
            {
                try
                {
                        extensionAssembly = Assembly.Load("GodotSharpExtension");
                }
                catch
                {
                    // Expected if the project doesn't have any generated extension dll.
                    return;
                }
            }

            var populateConstructorMethod =
                extensionAssembly
                    .GetType("Godot.ExtensionConstructors")
                    .GetMethod("AddExtensionConstructors",
                        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            if (populateConstructorMethod == null)
            {
                throw new MissingMethodException("Godot.ExtensionConstructors",
                    "AddExtensionConstructors");
            }

            populateConstructorMethod?.Invoke(null, null);
        }

        public static void  UnloadExtensionAssembly()
        {
            Constructors.ExtensionMethodConstructors?.Clear();
        }
    }
}
