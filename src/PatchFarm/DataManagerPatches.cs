using System.Collections;
using System.Data;
using Directers_Assistant.src.DataModels;
using Directers_Assistant.src.Textures;
using HarmonyLib;
using Il2CppDarkTonic.MasterAudio;
using Il2CppNewtonsoft.Json.Linq;
using Il2CppSystem.Reflection;
using Il2CppVampireSurvivors.App.Data;
using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.UI;
using MelonLoader;
using UnityEngine;
using SpriteManager = Il2CppVampireSurvivors.Graphics.SpriteManager;

#pragma warning disable S1118
#pragma warning disable S3398

namespace Directers_Assistant.src.PatchFarm
{
    internal class DataManagerPatches : BasePatch
    {
        internal static int CharacterIDs { get; private set; } = 10000;
        internal static int MusicIDs { get; private set; } = 10000;
        internal static int AlbumIDs { get; private set; } = 10000;

        [HarmonyPatch(typeof(DataManager))]
        class DataManagerPatch
        {
            [HarmonyPatch(nameof(DataManager.LoadBaseJObjects))]
            [HarmonyPostfix]
            static void LoadBaseJObjects_Postfix(DataManager __instance, object[] __args, MethodBase __originalMethod)
            {
                SpriteRegister();
                if (Melon<DirecterAssistantMod>.Instance.AudioImport)
                {
                    MusicRegister(__instance);
                    AlbumRegisterBD(__instance);
                    AlbumRegister(__instance);
                }
                CharacterRegister(__instance, (DlcType)10000, null!);
            }

            [HarmonyPatch(nameof(DataManager.MergeInJsonData))]
            [HarmonyPrefix]
            static void MergeInJsonData_Patch(DataManager __instance, DataManagerSettings settings, DlcType dlcType)
            {
                CharacterRegister(__instance, dlcType, settings);
            }
        }

        static void SpriteRegister()
        {
            foreach (SpriteDataModelWrapper spriteWrapper in GetManager()!.Sprites)
            {
                if (SpriteImporter.textures.ContainsKey(spriteWrapper.TextureNameWithoutExtension))
                {
                    Sprite sprite = SpriteImporter.LoadSprite(SpriteImporter.textures[spriteWrapper.TextureNameWithoutExtension], spriteWrapper.Sprite.Rect, spriteWrapper.SpriteNameWithoutExtension);
                    SpriteManager.RegisterSprite(sprite);
                }
                else
                {
                    Texture2D texture = SpriteImporter.LoadTexture(spriteWrapper.TexturePath, spriteWrapper.TextureNameWithoutExtension);
                    Sprite sprite = SpriteImporter.LoadSprite(texture, spriteWrapper.Sprite.Rect, spriteWrapper.SpriteNameWithoutExtension);
                    SpriteManager.RegisterSprite(sprite);
                }
            }
        }

        static void CharacterRegister(DataManager __instance, DlcType dlcType, DataManagerSettings settings)
        {
            foreach (CharacterDataModelWrapper characterWrapper in GetManager()!.Characters)
            {
                CharacterDataModel character = characterWrapper.Character;
                if (dlcType != character.DlcSort) continue;
                CharacterType characterType = (CharacterType)CharacterIDs++;
                characterWrapper.CharacterType = characterType;
                if (Melon<DirecterAssistantMod>.Instance.AudioImport && character.BGM == null && character.CustomBgm != null)
                {
                    character.BGM = GetManager()!.MusicID2Type[character.CustomBgm];
                }

                GetLogger().Msg($"Adding character... {characterType} {character.CharName}");
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(characterWrapper.CharacterSettings,
                    Newtonsoft.Json.Formatting.Indented,
                    SerializerSettings);

                JArray json = JArray.Parse(jsonString);

                if (settings != null && character.DlcSort == dlcType)
                {
                    JObject dlc = JObject.Parse(settings._CharacterDataJsonAsset.text);
                    dlc.Add(characterType.ToString(), json);
                    TextAsset textAsset = new TextAsset(dlc.ToString());
                    settings._CharacterDataJsonAsset = textAsset;
                }
                else if (character.DlcSort == dlcType)
                    __instance._allCharactersJson.Add(characterType.ToString(), json);
                else
                    continue;
                GetManager()!.CharacterDict.Add(characterType, characterWrapper);
                if (character.InternalID == null) character.InternalID = characterType.ToString();
                GetManager()!.CharacterID2Type.Add(character.InternalID, characterType);
            }
        }

        static void MusicRegister(DataManager __instance)
        {
            foreach (MusicDataModelWrapper musicWrapper in GetManager()!.Music)
            {
                MusicDataModel music = musicWrapper.Music;
                BgmType bgm = (BgmType)MusicIDs++;
                JObject json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(music,
                    Newtonsoft.Json.Formatting.Indented,
                    SerializerSettings));
                __instance._allMusicDataJson.Add(bgm.ToString(), json);
                GetManager()!.MusicDict.Add(bgm, musicWrapper);
                GetManager()!.MusicID2Type.Add(music.InternalID, bgm);

                MasterAudio.Playlist play = new MasterAudio.Playlist();
                play.playlistName = bgm.ToString();
                MasterAudio.CreatePlaylist(play, false);
                AudioClip a = musicWrapper.clip;
                MasterAudio.AddSongToPlaylist(bgm.ToString(), a, true);
            }
        }

        static void AlbumRegisterBD(DataManager __instance)
        {
            Texture2D texture2D = SpriteImporter.LoadTexture("blackDiskAlbumArt.png");
            Sprite sprite = SpriteImporter.LoadSprite(texture2D, new Rect(0f, 0f, 512f, 512f), "blackdisk");
            SpriteManager.RegisterSprite(sprite);

            AlbumType album = (AlbumType)AlbumIDs++;
            JObject json = new JObject();
            JArray jArray = new JArray();
            for (int i = 0; i < GetManager()!.MusicDict.Keys.Count; i++)
            {
                jArray.Add(GetManager()!.MusicDict.Keys.ToArray()[i].ToString());
            }


            json.Add("title", "Assistant");
            json.Add("icon", "blackdisk.png");
            json.Add("contentGroupType", "BASE");
            json.Add("trackList", jArray);
            json.Add("isUnlocked", true);

            __instance._allAlbumData.Add(album.ToString(), json);
        }

        static void AlbumRegister(DataManager __instance)
        {
            foreach(AlbumDataModelWrapper albumWrapper in GetManager().Albums)
            {
                AlbumType album = (AlbumType)AlbumIDs++;
                JArray jArray = new JArray();
                for (int i = 0; i < albumWrapper.Album.TrackList.Count; i++)
                    jArray.Add(GetManager()!.MusicID2Type[albumWrapper.Album.TrackList[i]].ToString());

                JObject json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(albumWrapper.Album,
                    Newtonsoft.Json.Formatting.Indented,
                    SerializerSettings));
                json.Remove("trackList");
                json.Add("trackList", jArray);
                __instance._allAlbumData.Add(album.ToString(), json);
            }
        }

        [HarmonyPatch(typeof(SongSelectionPanel))]
        class SSongSelectionPanelPatch
        {
            [HarmonyPatch(nameof(SongSelectionPanel.SetIcon))]
            [HarmonyPostfix]
            static void SetIcon_Postfix(SongSelectionPanel __instance)
            {
                BgmType bgm = __instance._selectedSong;
                if(GetManager().MusicDict.ContainsKey(bgm))
                {
                    MusicDataModelWrapper musicWrapper = GetManager().MusicDict[bgm];
                    __instance._Icon.sprite = SpriteManager.GetSprite(musicWrapper.Music.Icon);
                }
            }
        }

        [HarmonyPatch(typeof(AdvancedMusicSelection.__c__DisplayClass61_0))]
        class AdvancedMusicSelection__c__DisplayClass61_0Patch
        {
            [HarmonyPatch(nameof(AdvancedMusicSelection.__c__DisplayClass61_0._PlayAtSpeed_b__0))]
            [HarmonyPostfix]
            static void PlayMusic_Postfix(AdvancedMusicSelection.__c__DisplayClass61_0 __instance)
            {
                BgmType bgmType = __instance.__4__this._selectedTrack._bgmType;
                if (GetManager()!.MusicDict.ContainsKey(bgmType))
                {
                    MasterAudio.Playlist play = new MasterAudio.Playlist();
                    play.playlistName = bgmType.ToString();
                    MasterAudio.CreatePlaylist(play, false);
                    AudioClip a = GetManager()!.MusicDict[bgmType].clip;
                    MasterAudio.AddSongToPlaylist(bgmType.ToString(), a, true);
                    MasterAudio.StartPlaylist(bgmType.ToString());
                }
            }
        }
    }
}
