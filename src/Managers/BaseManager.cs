using HarmonyLib.Tools;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directer_Machine.Managers
{
    internal class BaseManager
    {
        internal MelonLogger.Instance logger = Melon<DirecterMachineMod>.Logger;
        readonly string ZipPath;
        readonly string DataPath;
        readonly string SavePackPath;
        readonly string CharacterPath;
        readonly string SpritePath;
        readonly string MusicPath;
        readonly string WeaponPath;
        readonly string ArcanaPath;
        public readonly bool BaseSuccess;
        private BaseManager(string ZipPath, string DataPath)
        {
            this.ZipPath = ZipPath;
            this.DataPath = DataPath;
            this.SavePackPath = Path.Combine(DataPath, "Installed Packs");
            this.CharacterPath = Path.Combine(DataPath, "Characters");
            this.SpritePath = Path.Combine(DataPath, "Sprites");
            this.MusicPath = Path.Combine(DataPath, "Music");
            this.WeaponPath = Path.Combine(DataPath, "Weapons");
            this.ArcanaPath = Path.Combine(DataPath, "Arcana");

            try
            {
                CreateInitDirectories()
                ParseZipFiles();
                this.BaseSuccess = true;
            }
            catch
            {
                this.BaseSuccess = false;
            }
        }
        void CreateInitDirectories()
        {
            List<string> directories = new List<string>() { this.SavePackPath, this.CharacterPath, this.SpritePath,
                this.MusicPath, this.WeaponPath, this.ArcanaPath };

            foreach (string dir in directories)
            {
                CreateDirectory(dir);
            }
        }
        void CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }
        void EmptyDir(string directory)
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
                        this.logger.Msg($"Parsing {Path.GetFileName(zip)}");
                        handleZipFile(zip);
                    }
                    catch
                    {
                        this.logger.Error($"Failed to extract zip file: '{zip}'");
                    }
                }
            }
        }
        void handleZipFile(string filePath)
        {
            List<string> filesToClean = new();
            try
            {
                string jsonString;

                using (FileStream fileStream = File.OpenRead(filePath))
                using (ZipArchive zipArchive = new (fileStream, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zipArchive.Entries)
                    {
                        if (entry.FullName.EndsWith('/'))
                        {
                            string[] dir = entry.FullName.Split('/');
                            string outputDir = Path.Combine(this.DataPath, dir[0], dir[1]);
                            if (dir[0] != "")
                            {
                                if (Directory.Exists(outputDir))
                                {
                                    this.logger.Warning($"Output directory '{outputDir}' already exists");
                                    if (!EmptyDir(outputDir))
                                    {
                                        this.logger.Warning($"Output directory '{outputDir}' is not empty. Is this the same character?");
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
                            if (entry.Name.EndsWith(".json"))
                            {
                                entry.ExtractToFile(Path.Combine(this.DataPath, entry.FullName));
                                filesToClean.Add(Path.Combine(this.DataPath, entry.FullName));
                            }
                            else if (entry.Name.EndsWith(".png"))
                            {
                                try
                                {
                                    // Don't overwrite output for now.
                                    entry.ExtractToFile(imageFilePath, false);
                                    filesToClean.Add(imageFilePath);
                                }
                                catch (IOException ioe)
                                {
                                    this.logger
                                        .Error($"Error: {imageFilePath} already exists. Are you trying to reimport the same character?");

                                    throw;
                                }
                                catch (Exception e)
                                {
                                    this.logger
                                        .Error($"Error: Unexpected error while extracting image {entry.Name} from zip file. {e}");

                                    throw;
                                }
                            }
                            else
                            {
                                this.logger.Warning($"Found invalid file: '{entry.FullName}', ignored.")
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException exception)
            {
                this.logger
                    .Error($"Error: File did not exist. Do you have permission to access the directory?");

                this.logger.Error($"Skipping file: {filePath} - {exception}");
                CleanupFiles(filesToClean);
                return;
            }
            catch (Exception e)
            {
                this.logger.Error($"Copy and paste the following exception to new issue on github.");
                this.logger.Error($"Caught unexpected exception: {e}");
                CleanupFiles(filesToClean);
                throw;
            }

            this.logger
                .Msg($"Extraction of {Path.GetFileNameWithoutExtension(filePath)} successful. Deleting zip file.");

            CleanupFiles(new List<string>() { filePath });
        }
        void CleanupFiles(List<string> filesToClean)
        {
            foreach (string file in filesToClean.Where(file => File.Exists(file)))
            {
                try
                {
                    SavePack(file);
                }
                catch (Exception e)
                {
                    this.logger.Error($"An error occured trying to clean up '{file}': {e}\n***** Manually clean up this file *****")
                }
            }
        }
        void SavePack(string filePath)
        {
            if (File.Exists(filePath))
            {
                string fileName = Path.GetFileName(filePath);
                string newFilePath = GetUnusedName(Path.Combine(this.SavePackPath, fileName), 0);

                File.Move(filePath, newFilePath);
            }
        }
        string GetUnusedName(string filePath, int level)
        {
            if (!File.Exists(filePath))
                return filePath;

            string newFilePath = filePath.Insert(filePath.Length - 4, "-" + level);
            if (File.Exists(newFilePath))
                return newFilePath;

            return GetUnusedName(filePath, level);
        }
    }
}
