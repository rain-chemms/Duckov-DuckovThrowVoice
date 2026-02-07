using Duckov;
using Duckov.UI;
using Duckov.Utilities;
using DuckovThrowVoice.Settings;
using HarmonyLib;
using HarmonyLoadMod;
using ItemStatsSystem;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

namespace DuckovThrowVoice
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {        
        private void OnEnable()
        {
            SetClips();
        }

        private void OnDisable()
        {
            harmony.UnpatchAll("DuckovThrowVoice");
        }
        void Awake()
        {
            Debug.Log("DuckovThrowVoice Mod Loaded!");
        }
        void Start()
        {
            //创建Harmony
            harmony = new Harmony("DuckovThrowVoice");
            harmony.PatchAll();
            Debug.Log("DuckovThrowVoice Mod Harmony Started!");
        }

        void Update()
        {
            //SetVoicerPosition();
            //PlayVoice();
            ClipIndexControl();


        }

        private void GetNewestSetting() 
        {
            
        }


        [SerializeField] public Harmony harmony;//Harmony实例

        [SerializeField] public static string[] audioExtensions = { ".wav", ".ogg", ".mp3" };//音频文件的支持格式

        [SerializeField] public static string clipsFilePath = "./Duckov_Data/Mods/DuckovThrowVoice/AudioClips";//音频文件夹的路径
        [SerializeField] public static string smokeAddPath = "/SmokeBomb";//在clipFilePath文件夹下的子文件夹路径,代表所有烟雾弹音频路径
        [SerializeField] public static string bombAddPath = "/GrenadeBomb";//在clipFilePath文件夹下的子文件夹路径,代表所有手雷音频路径
        [SerializeField] public static string flashAddPath = "/FlashBomb";//在clipFilePath文件夹下的子文件夹路径,代表所有闪光弹音频路径
        [SerializeField] public static string fireAddPath = "/FireBomb";//在clipFilePath文件夹下的子文件夹路径,代表所有燃烧弹音频路径


        [NonSerialized] private static List<string> smokeClipsPath = new List<string>();//每个烟雾弹音频文件的具体路径列表
        [NonSerialized] private static List<string> bombClipsPath = new List<string>();//每个手雷音频文件的具体路径列表
        [NonSerialized] private static List<string> flashClipsPath = new List<string>();//每个闪光弹音频文件的具体路径列表
        [NonSerialized] private static List<string> fireClipsPath = new List<string>();//每个燃烧弹音频文件的具体路径列表


        [NonSerialized] public static int bombClipIndex = 0;//手雷音频的索引
        [NonSerialized] public static int flashClipIndex = 0;//闪光弹音频的索引
        [NonSerialized] public static int smokeClipIndex = 0;//烟雾弹音频的索引
        [NonSerialized] public static int fireClipIndex = 0;//燃烧弹音频的索引

        public static void SetClips()//该函数用于读取clipsFilePath中的所有文件,加载每个文件的路径并将每个索引归零
        {
            foreach (string ext in audioExtensions)
            {
                bombClipsPath.AddRange(Directory.GetFiles(clipsFilePath + bombAddPath, $"*{ext}", SearchOption.AllDirectories));
                smokeClipsPath.AddRange(Directory.GetFiles(clipsFilePath + smokeAddPath, $"*{ext}", SearchOption.AllDirectories));
                flashClipsPath.AddRange(Directory.GetFiles(clipsFilePath + flashAddPath, $"*{ext}", SearchOption.AllDirectories));
                fireClipsPath.AddRange(Directory.GetFiles(clipsFilePath + fireAddPath, $"*{ext}", SearchOption.AllDirectories));
            }
            bombClipIndex = 0;
            smokeClipIndex = 0;
            flashClipIndex = 0;
            fireClipIndex = 0;
        }

        //播放手雷音效:使用不同的手雷名称区分投掷音效
        public static void PlayVoice(String displayName = "手雷")
        {
            if(smokeClipsPath.Count <=0 || fireClipsPath.Count <= 0 || flashClipsPath.Count <= 0 || bombClipsPath.Count <= 0) return;
            switch (displayName)
            {
                case "闪光":
                    if (flashClipIndex >= flashClipsPath.Count) break;
                    AudioManager.PostCustomSFX(flashClipsPath[flashClipIndex]);
                    break;
                case "烟雾弹":
                    if (smokeClipIndex >= smokeClipsPath.Count) break;
                    AudioManager.PostCustomSFX(smokeClipsPath[smokeClipIndex]);
                    break;
                case "燃烧弹":
                    if (fireClipIndex >= fireClipsPath.Count) break; 
                    AudioManager.PostCustomSFX(fireClipsPath[fireClipIndex]);
                    break;
                case "管状炸弹":
                case "集束管状炸弹":
                case "毒雾弹":
                case "手雷":
                case "电击手雷":
                default:
                    if (bombClipIndex >= bombClipsPath.Count || bombClipIndex < 0) break;
                    AudioManager.PostCustomSFX(bombClipsPath[bombClipIndex]);
                    break;
            }
            Debug.Log("Voicer play Successful!");
        }

        //PlayVoice()函数重载,用物品ID区分手雷类型
        public static void PlayVoice(int itemID)
        {
            if (smokeClipsPath.Count <= 0 || fireClipsPath.Count <= 0 || flashClipsPath.Count <= 0 || bombClipsPath.Count <= 0) return;   
            switch (itemID) 
            {
                case 66://代表"闪光手雷"
                    if (flashClipIndex >= flashClipsPath.Count || flashClipIndex<0) break;
                    AudioManager.PostCustomSFX(flashClipsPath[flashClipIndex]);
                    break;
                case 660://代表"烟雾弹"
                    if (smokeClipIndex >= smokeClipsPath.Count || smokeClipIndex<0) break;
                    AudioManager.PostCustomSFX(smokeClipsPath[smokeClipIndex]);
                    break;
                case 941://代表"燃烧弹":
                    if(fireClipIndex >= fireClipsPath.Count || fireClipIndex < 0) break;
                    AudioManager.PostCustomSFX(fireClipsPath[fireClipIndex]);
                    break;
                case 23://代表"管状炸弹":
                case 24://代表"集束管状炸弹":
                case 933://代表"毒雾弹":
                case 67://代表"手雷"
                case 942://代表"电击手雷"
                default:
                    if (bombClipIndex >= bombClipsPath.Count || bombClipIndex < 0) break;
                    AudioManager.PostCustomSFX(bombClipsPath[bombClipIndex]);
                    break;
            }
            Debug.Log("Voicer play Successful!");
        }

        //用于控制当前音频的索引
        private void ClipIndexControl()
        {
            Duckov.UI.BarDisplay barDisplay = new BarDisplay();
            //barDisplay.Setup();
        }

        /*失败的实例+启示
        [HarmonyPatch(typeof(CA_Skill), "ReleaseSkill")]
        public class VoiceGenerator:CA_UseItem 
        {
            [HarmonyPrefix]
            public static void Prefix(CA_Skill __instance,ref SkillTypes ___skillTypeToRelease)
            {
                if (___skillTypeToRelease.Equals(SkillTypes.itemSkill))
                {
                    CharacterSkillKeeper skillKeeper = __instance.GetSkillKeeper(___skillTypeToRelease);//SkillKeeper获取
                    SkillBase skillBase = skillKeeper.Skill;//skill获取
                    Debug.Log("SkillBase信息:");
                    Debug.Log(skillBase);
                    //skill相关信息获取
                    Action releaseEvent = skillBase.OnSkillReleasedEvent;//获取释放函数委托
                    releaseEvent += PlayVoice;//委托赋值
                    
                }
            }
        }
        */

        [HarmonyPatch(typeof(SkillBase), "ReleaseSkill")]
        public class VoicePatcher 
        {
            [HarmonyPrefix]
            public static void Prefix(SkillBase __instance) 
            {
                Item nowItem = __instance.fromItem;
                int typeID = nowItem.TypeID;
                string displayName = nowItem.DisplayName;
                Debug.Log("投掷的物品TypeID:"+typeID);
                Debug.Log("投掷的物品DisplayName:" + displayName);
                PlayVoice(typeID);
            }
        }
    }
}
