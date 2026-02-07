using System;
using System.IO;
using UnityEngine;
using DuckovThrowVoice;

namespace DuckovThrowVoice.Settings
{
internal static class DuckovThrowVoiceSettings
{
    [Serializable]
    public sealed class Data
    {
        public string ClipsFilePath = "./Duckov_Data/Mods/DuckovThrowVoice/AudioClips";
        public string SmokeAddPath = "/SmokeBomb";//在clipFilePath文件夹下的子文件夹路径,代表所有烟雾弹音频路径
        public string BombAddPath = "/GrenadeBomb";//在clipFilePath文件夹下的子文件夹路径,代表所有手雷音频路径
        public string FlashAddPath = "/FlashBomb";//在clipFilePath文件夹下的子文件夹路径,代表所有闪光弹音频路径
        public string FireAddPath = "/FireBomb";//在clipFilePath文件夹下的子文件夹路径,代表所有燃烧弹音频路径
        public string BombClipIndex = "0";//手雷音频的索引
        public string FlashClipIndex = "0";//闪光弹音频的索引
        public string SmokeClipIndex = "0";//烟雾弹音频的索引
        public string FireClipIndex = "0";//燃烧弹音频的索引
    }

    private static readonly string SettingsPath =
    Path.Combine(Application.persistentDataPath, "DuckovThrowVoiceSettings.json");

    private static Data _data = new Data();
    private static bool _loaded;

    public static event Action<Data>? OnSettingsChanged;

    public static Data Current => _data;

    public static void Load()
    {
        if (_loaded)
        {
            return;
        }

        _loaded = true;

        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var loaded = JsonUtility.FromJson<Data>(json);
                if (loaded != null)
                {
                    _data = loaded;
                    OnSettingsChanged?.Invoke(_data);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[DuckovThrowVoice][Settings] Failed to load settings: {ex.Message}");
        }

        _data = new Data();
        Persist();
    }

    public static void ResetToDefaults()
    {
        _data = new Data();
        _loaded = true;
        Persist();
        SyncDataToVoicer();
    }
    
    public static void SetClipsFilePath(string value) => UpdateIfChanged(ref _data.ClipsFilePath, value);
    public static void SetSmokeAddPath(string value) => UpdateIfChanged(ref _data.SmokeAddPath, value);

    public static void SetFireAddPath(string value) => UpdateIfChanged(ref _data.FireAddPath, value);
    public static void SetFlashAddPath(string value) => UpdateIfChanged(ref _data.FlashAddPath, value);
    public static void SetBombAddPath(string value) => UpdateIfChanged(ref _data.BombAddPath, value);
    public static void SetBombClipIndex(string value) => UpdateIfChanged(ref _data.BombClipIndex, value);
    public static void SetSmokeClipIndex(string value) => UpdateIfChanged(ref _data.SmokeClipIndex, value);
    public static void SetFlashClipIndex(string value) => UpdateIfChanged(ref _data.FlashClipIndex, value);
    public static void SetFireClipIndex(string value) => UpdateIfChanged(ref _data.BombClipIndex, value);
    
    private static void UpdateIfChanged(ref string field, string value)
    {
        if (field.Equals(value))
        {
            return;
        }

        field = value;
        Persist();
        SyncDataToVoicer();
    }

    /*
    private static void UpdateIfChanged(ref int field, int value)
    {
        if (field == value)
        {
            return;
        }
        field = value;
        Persist();
    }
    */

    private static void SyncDataToVoicer() 
    {
        DuckovThrowVoice.ModBehaviour.clipsFilePath = Current.ClipsFilePath;
            DuckovThrowVoice.ModBehaviour.fireAddPath = Current.FireAddPath;
            DuckovThrowVoice.ModBehaviour.flashAddPath = Current.FlashAddPath;
            DuckovThrowVoice.ModBehaviour.bombAddPath = Current.BombAddPath;
            DuckovThrowVoice.ModBehaviour.smokeAddPath = Current.SmokeAddPath;
            int result = 0;
            if (int.TryParse(Current.BombClipIndex, out result)) 
                DuckovThrowVoice.ModBehaviour.bombClipIndex = result;
            if(int.TryParse(Current.FlashClipIndex,out result))
                DuckovThrowVoice.ModBehaviour.flashClipIndex = result;
            if (int.TryParse(Current.SmokeClipIndex, out result))
                DuckovThrowVoice.ModBehaviour.smokeClipIndex = result;
            if (int.TryParse(Current.FireClipIndex, out result))
                DuckovThrowVoice.ModBehaviour.fireClipIndex = result;
        }

    private static void Persist()
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonUtility.ToJson(_data, true);
            File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[DuckovThrowvoice][Settings] Failed to save settings: {ex.Message}");
        }

        OnSettingsChanged?.Invoke(_data);
    }
}
}