using System.Collections.Generic;
namespace Timberborn.BlueprintSystem
{
    // Timberborn 1.1: the return type is part of the CLR method signature.
    // Never model GetBundle as returning object; it will compile but crash.
    public sealed class BlueprintFileBundle { }
    public interface IBlueprintModifierProvider
    {
        string ModifierName { get; }
        IEnumerable<string> GetModifiers(string blueprintPath);
    }
    public sealed class BlueprintFileBundleLoader
    {
        public BlueprintFileBundle? GetBundle(string path) => null;
    }
}
