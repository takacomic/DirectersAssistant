using System.Reflection;
using System.Runtime.InteropServices;
using Directers_Assistant.src.Managers;
using Directers_Assistant.src.PatchFarm;
using MelonLoader;

namespace Directers_Assistant.src
{
    internal static class ModInfo
    {
        public const string Name = "Directer's Assistant";
        public const string Description = "";
        public const string Author = "Takacomic";
        public const string Company = "CorruptedInfluences";
        public const string Version = "0.1.2";
        public const string Download = "https://github.com/takacomic/DirectersMachine/latest";
    }
    internal class DirecterAssistantMod : MelonMod
    {
        private static readonly string ModDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UserData", "DirectersAssistant");
        private static readonly string BloodDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UserData", "Bloodlines", "data", "characters");
        private static readonly string LibDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UserLibs");
        private static readonly string DataDirectory = Path.Combine(ModDirectory, "data");
        internal bool AudioImport = false;
        internal BaseManager? BaseManager;
        internal BasePatch? BasePatch;
        public override void OnInitializeMelon()
        {
            if (!Directory.Exists(ModDirectory))
            {
                Directory.CreateDirectory(ModDirectory);
                Directory.CreateDirectory(DataDirectory);
            }
            BaseManager = new(ModDirectory, DataDirectory, BloodDirectory);
            BasePatch = new();
            if (LibCheck("AudioImportLib.dll")) AudioImport = true;
        }

        internal bool LibCheck(string lib)
        {
            try
            {
                string path = Path.Combine(LibDirectory, lib);
                AssemblyName testAssembly = AssemblyName.GetAssemblyName(path);
                return true;
            }
            catch (FileNotFoundException)
            {
                LoggerInstance.Warning($"{lib} does not exist in {LibDirectory}");
                return false;
            }
            catch (BadImageFormatException)
            {
                return false;
            }
            catch (FileLoadException)
            {
                return true;
            }
        }
    }
}
