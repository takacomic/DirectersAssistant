using Directer_Machine;
using MelonLoader;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(ModInfo.Description)]
[assembly: AssemblyDescription(ModInfo.Description)]
[assembly: AssemblyCompany(ModInfo.Company)]
[assembly: AssemblyProduct(ModInfo.Name)]
[assembly: AssemblyCopyright("Copyright " + ModInfo.Author + " 2025")]
[assembly: AssemblyTrademark(ModInfo.Company)]
[assembly: AssemblyVersion(ModInfo.Version)]
[assembly: AssemblyFileVersion(ModInfo.Version)]
[assembly: MelonInfo(typeof(DirecterMachineMod), ModInfo.Name, ModInfo.Version, ModInfo.Author, ModInfo.Download)]
[assembly: MelonGame("poncle", "Vampire Survivors")]
[assembly: ComVisible(false)]

[assembly: Guid("8c8346d7-68ba-469e-9d87-bd4bac7fdb3d")]
