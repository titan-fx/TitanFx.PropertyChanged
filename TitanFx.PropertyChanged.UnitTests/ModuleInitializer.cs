using System.Runtime.CompilerServices;

namespace TitanFx.PropertyChanged.UnitTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}
