using Directer_Machine.Managers;
using Directer_Machine.PatchFarm;
using MelonLoader;

namespace Directer_Machine
{
    internal static class ModInfo
    {
        public const string Name = "Directer's Machine";
        public const string Description = "";
        public const string Author = "Takacomic";
        public const string Company = "CorruptedInfluences";
        public const string Version = "0.0.1";
        public const string Download = "https://github.com/takacomic/DirectersMachine/latest";
    }
    internal class DirecterMachineMod : MelonMod
    {
        private static readonly string ModDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UserData", "DirectersMachine");
        private static readonly string DataDirectory = Path.Combine(ModDirectory, "data");
        internal BaseManager? BaseManager;
        internal BasePatch? BasePatch;
        public override void OnInitializeMelon()
        {
            if (!Directory.Exists(ModDirectory))
            {
                Directory.CreateDirectory(ModDirectory);
                Directory.CreateDirectory(DataDirectory);
            }
            BaseManager = new(ModDirectory, DataDirectory);
            BasePatch = new ();
        }
    }
}
