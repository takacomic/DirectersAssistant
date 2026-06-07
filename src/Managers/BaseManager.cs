using System.IO.Compression;
using Directers_Assistant.src.DataModels;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using MelonLoader;
using MelonLoader.ICSharpCode.SharpZipLib.GZip;

namespace Directers_Assistant.src.Managers
{
    internal class BaseManager
    {
        internal List<CharacterDataModelWrapper> Characters { get; set; } = new();
        internal Dictionary<CharacterType, CharacterDataModelWrapper> CharacterDict { get; set; } = new();
        internal Dictionary<string, CharacterType> CharacterID2Type { get; set; } = new();

        internal List<SpriteDataModelWrapper> Sprites { get; set; } = new();
        internal List<AlbumDataModelWrapper> Albums { get; set; } = new();

        internal List<MusicDataModelWrapper> Music { get; set; } = new();
        internal Dictionary<BgmType, MusicDataModelWrapper> MusicDict { get; set; } = new();
        internal Dictionary<string, BgmType> MusicID2Type { get; set; } = new();

        internal static MelonLogger.Instance GetLogger() => Melon<DirecterAssistantMod>.Logger;
        readonly string ZipPath = null!;
        readonly string DataPath = null!;
        private readonly string _savePackPath = null!;
        internal string CharacterPath;
        internal string SpritePath = null!;
        internal string MusicPath = null!;
        internal string AlbumPath = null!;
        internal string WeaponPath = null!;
        internal string ArcanaPath = null!;
        public readonly bool BaseSuccess;

        internal CharacterManager CharacterManager = null!;
        internal MusicManager MusicManager = null!;
        internal SpriteManager SpriteManager = null!;
        internal BaseManager _BaseManager = null!;
        internal BaseManager(string ZipPath, string DataPath, string BloodlinesPath)
        {
            this.ZipPath = ZipPath;
            this.DataPath = DataPath;
            _savePackPath = Path.Combine(DataPath, "Installed Packs");
            CharacterPath = Path.Combine(DataPath, "Bloodlines");
            SpritePath = Path.Combine(DataPath, "Sprites");
            MusicPath = Path.Combine(DataPath, "Blackdisk");
            AlbumPath = Path.Combine(MusicPath, "Albums");
            WeaponPath = Path.Combine(DataPath, "Weapons");
            ArcanaPath = Path.Combine(DataPath, "Arcana");
            _BaseManager = this;

            try
            {
                CreateInitDirectories();
                if(Directory.Exists(BloodlinesPath)) HotPotatoBloodlinesFiles(BloodlinesPath);
                ParseZipFiles();
                BaseSuccess = true;
            }
            catch (Exception ex)
            {
                GetLogger().Error($"Failed to initialize BaseManager: {ex.Message}");
                BaseSuccess = false;
            }
            // Initialize sub-managers regardless of success state
            // This allows partial functionality even if some initialization failed
            SpriteManager = new SpriteManager(SpritePath, _BaseManager);
            MusicManager = new MusicManager(MusicPath, _BaseManager);
            CharacterManager = new CharacterManager(CharacterPath, _BaseManager, SpriteManager);
        }

        protected BaseManager()
        {
        }

        void CreateInitDirectories()
        {
            List<string> directories = new List<string> { _savePackPath, CharacterPath, SpritePath,
                MusicPath, AlbumPath/*, WeaponPath, ArcanaPath*/ };

            foreach (string dir in directories)
            {
                CreateDirectory(dir);
            }
        }

        static void CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }

        static bool EmptyDir(string directory)
        {
            return !Directory.EnumerateFileSystemEntries(directory).Any();
        }

        void ParseZipFiles()
        {
            if (Directory.Exists(ZipPath) && !EmptyDir(ZipPath))
            {

                List<string> zips = Directory.GetFiles(ZipPath, "*.zip").ToList();

                foreach (string zip in zips)
                {
                    try
                    {
                        GetLogger().Msg($"Parsing {Path.GetFileName(zip)}");
                        handleZipFile(zip);
                    }
                    catch
                    {
                        GetLogger().Error($"Failed to extract zip file: '{zip}'");
                    }
                }
            }
        }

        void HotPotatoBloodlinesFiles(string path)
        {
            foreach (string dir in Directory.GetDirectories(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);

                CreateDirectory(Path.Combine(CharacterPath, directoryInfo.Name));
                CreateDirectory(Path.Combine(SpritePath, directoryInfo.Name));
                foreach (string file in Directory.GetFiles(Path.Combine(path, dir)))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (file.EndsWith(".json"))
                    {
                        File.Move(file, Path.Combine(CharacterPath, directoryInfo.Name, fileInfo.Name));
                    }
                    else if (file.EndsWith(".png"))
                    {
                        File.Move(file, Path.Combine(SpritePath, directoryInfo.Name, fileInfo.Name));
                    }
                }
            }
        }
        void handleZipFile(string filePath)
        {
            List<string> filesToClean = new();
            try
            {
                string zipName = Path.GetFileNameWithoutExtension(filePath);
                using FileStream fileStream = File.OpenRead(filePath);
                using ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Read);
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    if (entry.FullName.EndsWith('/'))
                    {
                        string[] dir = entry.FullName.Split('/');
                        string outputDir = Path.Combine(DataPath, dir[0], dir[1]);
                        if (dir[0] != "")
                        {
                            if (Directory.Exists(outputDir))
                            {
                                GetLogger().Warning($"Output directory '{outputDir}' already exists");
                                if (!EmptyDir(outputDir))
                                {
                                    GetLogger().Warning($"Output directory '{outputDir}' is not empty. Is this the same character?");
                                }
                            }
                            else
                            {
                                CreateDirectory(outputDir);
                            }
                        }
                    }
                    else
                    {
                        if (entry.Name.EndsWith(".json") || entry.Name.EndsWith(".mp3") || entry.Name.EndsWith(".ogg"))
                        {
                            if (!entry.FullName.Contains('/'))
                            {
                                // For Bloodlines Characters
                                CreateDirectory(Path.Combine(CharacterPath, zipName));
                                entry.ExtractToFile(Path.Combine(CharacterPath, zipName, entry.Name));
                                filesToClean.Add(Path.Combine(CharacterPath, zipName, entry.Name));
                            }
                            else
                            {
                                entry.ExtractToFile(Path.Combine(DataPath, entry.FullName));
                                filesToClean.Add(Path.Combine(DataPath, entry.FullName));
                            }
                        }
                        else if (entry.Name.EndsWith(".png"))
                        {
                            string imageFilePath = Path.Combine(DataPath, entry.FullName);
                            if (!entry.FullName.Contains('/'))
                            {
                                // For Bloodlines Characters
                                CreateDirectory(Path.Combine(SpritePath, zipName));
                                imageFilePath = Path.Combine(SpritePath, zipName, entry.Name);
                            }

                            try
                            {
                                // Don't overwrite output for now.
                                entry.ExtractToFile(imageFilePath, false);
                                filesToClean.Add(imageFilePath);
                            }
#pragma warning disable CS0168 // Variable is declared but never used
                            catch (IOException ioe)
#pragma warning restore CS0168 // Variable is declared but never used
                            {
                                GetLogger()
                                    .Error($"Error: {imageFilePath} already exists. Are you trying to reimport the same character?");

                                throw;
                            }
                            catch (Exception e)
                            {
                                GetLogger()
                                    .Error($"Error: Unexpected error while extracting image {entry.Name} from zip file. {e}");

                                throw;
                            }
                        }
                        else
                        {
                            GetLogger().Warning($"Found invalid file: '{entry.FullName}', ignored.");
                        }
                    }
                }
            }
            catch (FileNotFoundException exception)
            {
                GetLogger()
                    .Error("Error: File did not exist. Do you have permission to access the directory?");

                GetLogger().Error($"Skipping file: {filePath} - {exception}");
                CleanupFiles(filesToClean);
                return;
            }
            catch (Exception e)
            {
                GetLogger().Error("Copy and paste the following exception to new issue on github.");
                GetLogger().Error($"Caught unexpected exception: {e}");
                CleanupFiles(filesToClean);
                throw;
            }

            GetLogger()
                .Msg($"Extraction of {Path.GetFileNameWithoutExtension(filePath)} successful. Deleting zip file.");

            CleanupFiles(new List<string> { filePath });
        }
        void CleanupFiles(List<string> filesToClean)
        {
            foreach (string file in filesToClean.Where(File.Exists))
            {
                try
                {
                    SavePack(file);
                }
                catch (Exception e)
                {
                    GetLogger().Error($"An error occured trying to clean up '{file}': {e}\n***** Manually clean up this file *****");
                }
            }
        }
        void SavePack(string filePath)
        {
            if (File.Exists(filePath))
            {
                string fileName = Path.GetFileName(filePath);
                string newFilePath = GetUnusedName(Path.Combine(_savePackPath, fileName), 0);

                File.Move(filePath, newFilePath);
            }
        }
        
        string GetUnusedName(string filePath, int level)
        {
            // If the file doesn't exist, we can use this path
            if (!File.Exists(filePath))
                return filePath;

            // Insert the level suffix before the extension (e.g., "file.json" -> "file-0.json")
            string newFilePath = filePath.Insert(filePath.Length - 4, "-" + level);

            // If the suffixed version exists, try the next level recursively
            if (File.Exists(newFilePath))
                return GetUnusedName(filePath, level + 1);

            // Return the unique suffixed path
            return newFilePath;
        }
    }
}
