#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Godot.NativeInterop;

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
                catch (FileNotFoundException)
                {
                    // "FileNotFoundException" is expected if the project doesn't have any generated extension dll.
                    // Any other exception could indicate an issue with the assembly and is therefore unhandled.
                    NativeFuncs.godotsharp_internal_set_extension_assembly_loaded(godot_bool.False);
                    return;
                }
            }

            var populateConstructorMethod =
                extensionAssembly
                    .GetType("Godot.Extension.ExtensionConstructors")?
                    .GetMethod("AddExtensionConstructors",
                        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            if (populateConstructorMethod == null)
            {
                throw new MissingMethodException("Godot.Extension.ExtensionConstructors",
                    "AddExtensionConstructors");
            }

            populateConstructorMethod?.Invoke(null, null);
            NativeFuncs.godotsharp_internal_set_extension_assembly_loaded(godot_bool.True);
        }

        public static void UnloadExtensionAssembly()
        {
            Constructors.ExtensionMethodConstructors?.Clear();
            NativeFuncs.godotsharp_internal_set_extension_assembly_loaded(godot_bool.False);
        }
    }
}
