using DuckovThrowVoice.Settings;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace DuckovThrowVoice.UI
{
    internal sealed class ModSettingsContent : MonoBehaviour
    {
        private enum Language
        {
            English,
            Chinese
        }

        //private Toggle? _togglePrefab;
        private Button? _buttonPrefab;
        private TextMeshProUGUI? _labelPrefab;
        private Button? _languageButton;
        private TextMeshProUGUI? _languageButtonLabel;
        private Image? _languageButtonBackground;
        //新增输入条
        private TMP_InputField? _inputFieldPrefab;

        private readonly List<(TMP_InputField toggle, Func<string> getter)> _inputFieldBindings = new List<(TMP_InputField, Func<string> )>();
        private readonly List<(TextMeshProUGUI label, string key, bool header)> _localizedLabels = new List<(TextMeshProUGUI , string , bool )>();
        private readonly List<(TMP_InputField label, string key, bool header)> _inputFieldLabels = new List<(TMP_InputField, string, bool)>();


        public void RefreshInputLabels()
        {
            foreach (var (label, key, header) in _inputFieldLabels)
            {
                if (label == null)
                {
                    continue;
                }

                string text = GetText(key);
                label.text = text;
            }
        }
        
        private bool _applyingFromSettings;
        private bool _built;
        private Language _language = Language.English;

        private static readonly Dictionary<string, (string en, string zh)> TextTable = new Dictionary<string, (string en, string zh)>()
        {
            ["header"] = ("DuckovThrowVoice Settings", "投掷物品音效设置"),
            //路径设置相关
            ["ClipsFilePath"] = ("Main ClipsFile Path", "主音频文件夹路径"),
            ["SmokeAddPath"] = ("Append SmokeBomb Voice File", "烟雾弹音效子文件夹"),
            ["BombAddPath "] = ("Append Grenade Voice File", "手雷音效子文件夹"),
            ["FlashAddPath"] = ("Append FlashBomb Voice File", "闪光手雷音效子文件夹"),
            ["FireAddPath"] = ("Append FireBomb Voice File", "燃烧弹音效子文件夹"),
            //音效选择器
            ["BombClipIndex"] = ("Now Grenade Voice Selection", "当前手雷音效选择"),
            ["FlashClipIndex"] = ("Now FlashBomb Voice Selection", "当前闪光手雷音效选择"),
            ["FireClipIndex"] = ("Now FireBomb Voice Selection", "当前燃烧弹音效选择"),
            ["SmokeClipIndex"] = ("Now SmokeBomb Voice Selection", "当前烟雾弹音效选择"),
            //调试相关
            ["reset"] = ("Reset to Defaults", "恢复默认")
        };

        public void Build(TMP_InputField? inputFieldPrefab/*Toggle? togglePrefab*/, Button? buttonPrefab)
        {
            if (_built)
            {
                return;
            }

            DuckovThrowVoiceSettings.Load();

            //_togglePrefab = togglePrefab;
            _buttonPrefab = buttonPrefab;
            _inputFieldPrefab = inputFieldPrefab;

            _language = ((Application.systemLanguage is SystemLanguage.ChineseSimplified) || (Application.systemLanguage is SystemLanguage.ChineseTraditional))
                ? Language.Chinese
                : Language.English;

            var layout = gameObject.GetComponent<VerticalLayoutGroup>() ?? gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = false; // 改为false让子控件自己控制高度
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false; // 改为false防止过度拉伸
            layout.spacing = 15f;
            layout.padding = new RectOffset(20, 20, 20, 20);

            var fitter = gameObject.GetComponent<ContentSizeFitter>() ?? gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;

            // 确保有Canvas Group用于正确的交互层级
            var canvasGroup = gameObject.GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            AddHeader();
            AddLanguageSwitchButton();
            AddInputField("ClipsFilePath", () => DuckovThrowVoiceSettings.Current.ClipsFilePath, DuckovThrowVoiceSettings.SetClipsFilePath);
            AddInputField("SmokeAddPath", () => DuckovThrowVoiceSettings.Current.SmokeAddPath, DuckovThrowVoiceSettings.SetSmokeAddPath);
            AddInputField("BombAddPath", () => DuckovThrowVoiceSettings.Current.BombAddPath, DuckovThrowVoiceSettings.SetBombAddPath);
            AddInputField("FlashAddPath", () => DuckovThrowVoiceSettings.Current.FlashAddPath, DuckovThrowVoiceSettings.SetFlashAddPath);
            AddInputField("FireAddPath", () => DuckovThrowVoiceSettings.Current.FireAddPath, DuckovThrowVoiceSettings.SetFireAddPath);
            AddInputField("FlashClipIndex", () => DuckovThrowVoiceSettings.Current.FlashClipIndex, DuckovThrowVoiceSettings.SetFlashClipIndex);
            AddInputField("SmokeClipIndex", () => DuckovThrowVoiceSettings.Current.SmokeClipIndex, DuckovThrowVoiceSettings.SetSmokeClipIndex);
            AddInputField("FireClipIndex", () => DuckovThrowVoiceSettings.Current.FireClipIndex, DuckovThrowVoiceSettings.SetFireClipIndex);
            AddInputField("BombClipIndex", () => DuckovThrowVoiceSettings.Current.BombClipIndex, DuckovThrowVoiceSettings.SetBombClipIndex);

            AddResetButton();

            DuckovThrowVoiceSettings.OnSettingsChanged += HandleSettingsChanged;
            _built = true;
        }

        private void AddHeader()
        {
            var label = CreateLabel("header", transform, true);
            if (label != null)
            {
                label.fontSize = 44f;
            }
        }

        private void AddLanguageSwitchButton()
        {
            if (_buttonPrefab == null)
            {
                return;
            }

            var buttonObj = Instantiate(_buttonPrefab.gameObject, transform);
            buttonObj.name = "BetterFire_LanguageSwitch";
            buttonObj.SetActive(true);
            RemoveLocalization(buttonObj);

            _languageButton = buttonObj.GetComponent<Button>();
            _languageButtonLabel = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            _languageButtonBackground = buttonObj.GetComponent<Image>();

            if (_languageButtonLabel != null)
            {
                _languageButtonLabel.enableAutoSizing = false;
                _languageButtonLabel.fontSize = 32f;
                _languageButtonLabel.fontStyle = FontStyles.Bold;
            }

            if (_languageButtonBackground != null)
            {
                _languageButtonBackground.color = new Color(0.13f, 0.2f, 0.35f, 0.95f);
            }

            var layout = buttonObj.GetComponent<LayoutElement>() ?? buttonObj.AddComponent<LayoutElement>();
            layout.minHeight = 48f;
            layout.flexibleWidth = 0f;
            layout.preferredHeight = 60f;

            if (_languageButton != null)
            {
                _languageButton.onClick.RemoveAllListeners();
                _languageButton.onClick.AddListener(() =>
                {
                    _language = _language == Language.English ? Language.Chinese : Language.English;
                    RefreshLocalizedLabels();
                    RefreshInputLabels();
                });
            }

            RefreshLanguageButtonVisuals();
        }

        private void AddInputField(string key, Func<string> getter, Action<string> setter)
        {
            if (_inputFieldPrefab == null)
            {
                return;
            }

            // 1. 创建行容器 - 水平布局
            var row = new GameObject($"DuckoveThrowVoice_Row_{key}",
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup));
            row.transform.SetParent(transform, false);

            var layout = row.GetComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.spacing = 15f;
            layout.childControlWidth = true; // 改为true让布局控制宽度
            layout.childForceExpandWidth = true; // 改为true让子项扩展宽度
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(10, 10, 5, 5);

            var rowRect = row.GetComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(600f, 60f);

            // 2. 创建标签（左侧）- 占50%宽度
            string text = GetText(key);
            var inputFieldText = Instantiate(_inputFieldPrefab.gameObject, row.transform);
            inputFieldText.SetActive(true);
            var element = inputFieldText.GetComponent<LayoutElement>() ??
                              inputFieldText.AddComponent<LayoutElement>();
            element.preferredWidth = 280f; // 约50%宽度
            element.flexibleWidth = 1f; // 允许弹性调整
            element.preferredHeight = 58f;
            element.minHeight = 55f;
            // 调整输入框的RectTransform
            var inputRect2 = inputFieldText.GetComponent<RectTransform>();
            inputRect2.sizeDelta = new Vector2(280f, 38f);
            var labelText = inputFieldText.GetComponent<TMP_InputField>();
            labelText.text = text;
            labelText.readOnly = true;
            _inputFieldLabels.Add((labelText,key,false));

            // 3. 创建输入框（右侧）- 占50%宽度
            var inputFieldObj = Instantiate(_inputFieldPrefab.gameObject, row.transform);
            inputFieldObj.SetActive(true);

            // 添加布局元素控制输入框尺寸
            var inputElement = inputFieldObj.GetComponent<LayoutElement>() ??
                              inputFieldObj.AddComponent<LayoutElement>();
            inputElement.preferredWidth = 280f; // 约50%宽度
            inputElement.flexibleWidth = 1f; // 允许弹性调整
            inputElement.preferredHeight = 58f;
            inputElement.minHeight = 55f;

            // 调整输入框的RectTransform
            var inputRect = inputFieldObj.GetComponent<RectTransform>();
            inputRect.sizeDelta = new Vector2(280f, 38f);

            // 4. 配置输入框（其余部分保持不变）
            var inputField = inputFieldObj.GetComponent<TMP_InputField>();

            // 设置输入框的文本（显示路径值）
            var currentValue = getter();
            inputField.text = !string.IsNullOrEmpty(currentValue) ? currentValue : "";

            // 设置占位符
            if (inputField.placeholder is TMP_Text placeholder)
            {
                placeholder.text = "输入路径...";
                placeholder.fontSize = 22f;
                placeholder.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
            }

            // 设置文本样式
            if (inputField.textComponent != null)
            {
                inputField.textComponent.fontSize = 32f;
                inputField.textComponent.color = Color.white;
            }
            
            // 5. 添加监听器
            inputField.onValueChanged.RemoveAllListeners();
            inputField.onValueChanged.AddListener(value =>
            {
                if (_applyingFromSettings) return;
                setter(value);
            });
            inputField.characterLimit = 0;//取消输入限制
            inputField.contentType = TMP_InputField.ContentType.Standard; // 允许任何字符
            inputField.lineType = TMP_InputField.LineType.SingleLine;//单行


            _inputFieldBindings.Add((inputField, getter));
        }
        private void AddResetButton()
        {
            if (_buttonPrefab == null)
            {
                return;
            }

            var buttonObj = Instantiate(_buttonPrefab.gameObject, transform);
            buttonObj.name = "DuckovThrowVoice_ResetButton";
            buttonObj.SetActive(true);
            RemoveLocalization(buttonObj);

            var label = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.enableAutoSizing = false;
                label.text = GetText("reset");
                RegisterLabel(label, "reset", false);
            }

            var buttonLayout = buttonObj.GetComponent<LayoutElement>() ?? buttonObj.AddComponent<LayoutElement>();
            buttonLayout.minHeight = 64f;
            buttonLayout.flexibleWidth = 1f;

            var button = buttonObj.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => DuckovThrowVoiceSettings.ResetToDefaults());
        }

        private void HandleSettingsChanged(DuckovThrowVoiceSettings.Data data)
        {
            _applyingFromSettings = true;
            foreach (var (inputField, getter) in _inputFieldBindings)
            {
                //toggle.SetIsOnWithoutNotify(getter());
                inputField.SetTextWithoutNotify(getter());
            }

            _applyingFromSettings = false;
            RefreshLocalizedLabels();
        }

        private TextMeshProUGUI? CreateLabel(string key, Transform parent, bool header)
        {
            if (_labelPrefab == null)
            {
                if (_inputFieldPrefab == null)
                {
                    return null;
                }

                _labelPrefab = _inputFieldPrefab.GetComponentInChildren<TextMeshProUGUI>();
            }

            if (_labelPrefab == null)
            {
                return null;
            }

            var label = Instantiate(_labelPrefab, parent);
            label.text = GetText(key);
            label.enableAutoSizing = false;
            label.color = header ? new Color(0.95f, 0.98f, 1f) : Color.white;
            label.gameObject.SetActive(true);
            RemoveLocalization(label.gameObject);
            RegisterLabel(label, key, header);
            return label;
        }

        private void RegisterLabel(TextMeshProUGUI label, string key, bool header)
        {
            _localizedLabels.Add((label, key, header));
        }

        private string GetText(string key)
        {
            if (!TextTable.TryGetValue(key, out var pair))
            {
                return key;
            }

            return _language == Language.Chinese ? pair.zh : pair.en;
        }

        private void RefreshLocalizedLabels()
        {
            foreach (var (label, key, header) in _localizedLabels)
            {
                if (label == null)
                {
                    continue;
                }

                label.text = GetText(key);
                label.color = header ? new Color(0.95f, 0.98f, 1f) : Color.white;
            }

            RefreshLanguageButtonVisuals();
        }

        private static void RemoveLocalization(GameObject root)
        {
            var components = root.GetComponentsInChildren<Component>(true);
            foreach (var component in components)
            {
                var typeName = component.GetType().Name;
                if (typeName.Contains("Localized", StringComparison.OrdinalIgnoreCase) ||
                    typeName.Contains("Localizor", StringComparison.OrdinalIgnoreCase))
                {
                    Destroy(component);
                }
            }
        }

        private void OnDestroy()
        {
            DuckovThrowVoiceSettings.OnSettingsChanged -= HandleSettingsChanged;
        }

        private void RefreshLanguageButtonVisuals()
        {
            if (_languageButtonLabel != null)
            {
                _languageButtonLabel.text = GetLanguageToggleText();
                _languageButtonLabel.color = _language == Language.English
                    ? new Color(0.95f, 0.85f, 0.2f)
                    : new Color(0.2f, 0.95f, 0.7f);
            }

            if (_languageButtonBackground != null)
            {
                _languageButtonBackground.color = _language == Language.English
                    ? new Color(0.15f, 0.22f, 0.4f, 0.95f)
                    : new Color(0.1f, 0.3f, 0.25f, 0.95f);
            }
        }

        private string GetLanguageToggleText()
        {
            return _language == Language.Chinese ? "Switch to English" : "切换至中文";
        }
    }
}