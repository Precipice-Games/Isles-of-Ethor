using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public class SoapWindow : EditorWindow
    {
        private Texture[] _icons;
        private TextAsset[] _texts;
        private GUIStyle _bgStyle;
        private GUISkin _skin;
        private readonly List<CategoryButton> _leftButtons = new List<CategoryButton>();
        private Action _onCategoryButtonClicked;
        private CategoryButton _currentCategoryButton;
        private SoapWindowSettings _soapWindowSettings;
        private string _version;

        private string HeaderTitle => " SOAP - Version " + _version;
        private const string DocURL = "https://obvious-game.gitbook.io/soap";
        private const string SceneDocURL = "https://obvious-game.gitbook.io/soap/scene-documentation/";
        private const string DiscordURL = "https://discord.gg/CVhCNDbxF5";

        private const string StoreURL = "https://assetstore.unity.com/packages/tools/utilities/soap-scriptableobject-architecture-pattern-232107#reviews";

        private const string YoutubeURL = "https://www.youtube.com/playlist?list=PLSHqi2dTiNGCncSOksACfJChpfPa6qz9w";

        private readonly string[] _sceneNames =
        {
            "1_ScriptableVariables", "2_Bindings", "3_ScriptableLists", "4_ScriptableEvents", "5_ScriptableSaves",
            "6_ScriptableEnums", "7_ScriptableSubAssets", "8_RuntimeVariables", "9_ScriptableDictionaries",
            "10_ScriptableSingletons"
        };

        [MenuItem("Window/Obvious/\ud83e\uddfc Soap/Soap Window", priority = 0)]
        public static void Open()
        {
            var window = GetWindow(typeof(SoapWindow), true, "Soap Window");
            window.minSize = new Vector2(650, 520);
            window.maxSize = new Vector2(650, 520);
            window.Show();
        }

        private void OnEnable()
        {
            LoadAssets();
            SetUpCategoryButtons();
            _skin = Resources.Load<GUISkin>("GUISkins/SoapWizardGUISkin");
            _soapWindowSettings = new SoapWindowSettings(this);
            _bgStyle = new GUIStyle(GUIStyle.none);
            _version = SoapPackageInfo.TryGetVersion(out var v) ? v : string.Empty;
        }

        private void LoadAssets()
        {
            _icons = new Texture[10];
            _icons[0] = Resources.Load<Texture>("Icons/icon_discord");
            _icons[1] = Resources.Load<Texture>("Icons/icon_youtube");
            _icons[2] = EditorGUIUtility.IconContent("SceneAsset Icon").image;
            _icons[3] = Resources.Load<Texture>("Icons/icon_documentation");
            _icons[4] = EditorGUIUtility.IconContent("UnityLogoLarge").image;
            _icons[5] = EditorGUIUtility.IconContent("SceneViewTools").image;
            _icons[6] = EditorGUIUtility.IconContent("cs Script Icon").image;
            _icons[7] = Resources.Load<Texture>("Icons/icon_docs");
            _icons[8] = Resources.Load<Texture>("Icons/icon_soapLogo");
            _icons[9] = Resources.Load<Texture>("Icons/icon_obviousLogo");
            _texts = new TextAsset[_sceneNames.Length];
            for (int i = 0; i < _texts.Length; i++)
                _texts[i] = Resources.Load<TextAsset>($"Texts/{_sceneNames[i]}_Text");
        }

        private void SetUpCategoryButtons()
        {
            //General
            var gettingStarted = new CategoryButton
                { Name = "Getting Started", OnClick = DrawGettingStarted, GroupName = "General" };
            _leftButtons.Add(gettingStarted);
            var settingsButton = new CategoryButton { Name = "Settings", OnClick = DrawSettings };
            _leftButtons.Add(settingsButton);

            //Examples Scenes
            for (int i = 0; i < _texts.Length; i++)
            {
                var group = i == 0 ? "Examples Scenes" : string.Empty;
                var groupIcon = i == 0 ? _icons[2] : null;
                var index = i;
                var scene = new CategoryButton
                {
                    Name = _sceneNames[i], OnClick = () => DrawScene(index), GroupName = group, GroupIcon = groupIcon
                };
                _leftButtons.Add(scene);
            }

            //Tools
            var soapWizard = new CategoryButton
                { Name = "Soap Wizard", OnClick = DrawSoapWizard, GroupName = "Tools", GroupIcon = _icons[5] };
            _leftButtons.Add(soapWizard);
            var soapTypeCreator = new CategoryButton
                { Name = "Soap Type Creator", OnClick = DrawSoapTypeCreator };
            _leftButtons.Add(soapTypeCreator);
            var eventsDebugWindow = new CategoryButton
                { Name = "Events Debug Window", OnClick = DrawEventsDebugWindow };
            _leftButtons.Add(eventsDebugWindow);

            //Select last category
            var lastCategory = SoapEditorUtils.WindowLastCategory;
            var button = string.IsNullOrEmpty(lastCategory)
                ? _leftButtons.FirstOrDefault(b => b.Name == gettingStarted.Name)
                : _leftButtons.FirstOrDefault(b => b.Name == lastCategory);
            SelectCategory(button);
        }

        private void OnGUI()
        {
            DrawHeader();
            GUILayout.Space(2);
            DrawLinkButtons();
            SoapInspectorUtils.DrawLine(2);
            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            DrawLeftPanel();
            DrawRightPanel();
            EditorGUILayout.EndHorizontal();
            DrawFooter();
        }

        private void DrawHeader()
        {
            _bgStyle.normal.background = SoapInspectorUtils.CreateTexture(SoapEditorUtils.SoapColor);
            GUILayout.BeginVertical(_bgStyle);
            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal();
            var iconStyle = new GUIStyle(GUIStyle.none);
            iconStyle.margin = new RectOffset(5, 0, 0, 0);
            GUILayout.Box(_icons[8], iconStyle, GUILayout.Width(60), GUILayout.Height(60));

            var titleStyle = _skin.customStyles.ToList().Find(x => x.name == "title");
            EditorGUILayout.LabelField(HeaderTitle, titleStyle);

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            var copyrightStyle = _skin.customStyles.ToList().Find(x => x.name == "copyright");
            GUILayout.Label("© 2026 Obvious Game", copyrightStyle);
            iconStyle.padding = new RectOffset(0, 1, 35, 1);
            GUILayout.Box(_icons[9], iconStyle, GUILayout.Width(25), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawLinkButtons()
        {
            GUILayout.BeginHorizontal();
            var buttonWidth = position.width / 4f;

            var guiContent = new GUIContent("Documentation", _icons[7]);
            if (GUILayout.Button(guiContent, GUILayout.MaxWidth(buttonWidth), GUILayout.MaxHeight(20f)))
                Application.OpenURL(DocURL);

            guiContent = new GUIContent("Discord", _icons[0]);
            if (GUILayout.Button(guiContent, GUILayout.MaxWidth(buttonWidth), GUILayout.MaxHeight(20f)))
                Application.OpenURL(DiscordURL);

            guiContent = new GUIContent("Asset Store", _icons[4]);
            if (GUILayout.Button(guiContent, GUILayout.MaxWidth(buttonWidth), GUILayout.MaxHeight(20f)))
                Application.OpenURL(StoreURL);

            guiContent = new GUIContent("Youtube", _icons[1]);
            if (GUILayout.Button(guiContent, GUILayout.MaxWidth(buttonWidth), GUILayout.MaxHeight(20f)))
                Application.OpenURL(YoutubeURL);

            GUILayout.EndHorizontal();
        }

        private void DrawLeftPanel()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150f), GUILayout.ExpandHeight(true));

            var index = 0;
            foreach (var categoryButton in _leftButtons)
            {
                if (!string.IsNullOrEmpty(categoryButton.GroupName))
                {
                    GUILayout.Space(index == 0 ? 0 : 10f);
                    if (categoryButton.GroupIcon != null)
                    {
                        var guiStyle = new GUIStyle(GUI.skin.label);
                        guiStyle.alignment = TextAnchor.MiddleLeft;
                        guiStyle.imagePosition = ImagePosition.ImageLeft;
                        guiStyle.fixedWidth = 150f;
                        GUILayout.Label(new GUIContent(categoryButton.GroupName, categoryButton.GroupIcon), guiStyle,
                            GUILayout.MaxHeight(20f));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(categoryButton.GroupName, GUILayout.MaxWidth(150));
                    }
                }

                if (GUILayout.Button(categoryButton.Name))
                    SelectCategory(categoryButton);
                index++;
            }

            EditorGUILayout.EndVertical();
        }

        private string GetScenePath(string sceneName)
        {
            var guid = AssetDatabase.FindAssets("t:Scene " + sceneName).FirstOrDefault();
            return guid != null ? AssetDatabase.GUIDToAssetPath(guid) : null;
        }

        private void DrawRightPanel()
        {
            if (_onCategoryButtonClicked == null)
                return;
            EditorGUILayout.BeginVertical();
            GUILayout.Space(5f);
            EditorGUILayout.LabelField(_currentCategoryButton.Name, EditorStyles.boldLabel);
            _onCategoryButtonClicked?.Invoke();
            EditorGUILayout.EndVertical();
        }

        private void DrawGettingStarted()
        {
            EditorGUILayout.LabelField(
                "Thank you for purchasing Soap : Scriptable Object Architecture Pattern." +
                "\nHow to get started ?",
                EditorStyles.wordWrappedLabel);

            DrawGroup(
                "- Read the user guide. You can learn about why to use Soap and its features.",
                "User Guide", _icons[7], () =>
                {
                    Application.OpenURL(DocURL);
                });

            DrawGroup(
                "- Check out the examples scenes and their documentation. They will show you practical examples of using Soap in a game.",
                "Example Scenes", _icons[2],
                () => SelectCategory(_leftButtons.FirstOrDefault(b => b.Name == _sceneNames[0])));

            DrawGroup(
                "- Watch the Youtube tutorials series. They will show you step-by-step how to make a Roguelite game with Soap.",
                "Youtube", _icons[1], () => Application.OpenURL(YoutubeURL));
        }

        private void DrawGroup(string description, string buttonName, Texture icon, Action onButtonClick)
        {
            GUILayout.Space(5f);
            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var guiContent = new GUIContent(buttonName, icon);
            if (GUILayout.Button(guiContent, GUILayout.MaxWidth(150f), GUILayout.MaxHeight(20f)))
                onButtonClick?.Invoke();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawScene(int i)
        {
            var sceneName = _sceneNames[i];
            EditorGUILayout.LabelField(_texts[i].text, EditorStyles.wordWrappedLabel);

            GUILayout.Space(25f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(50f);
            var guiContent = new GUIContent("Open Example Scene", _icons[2]);
            if (GUILayout.Button(guiContent, GUILayout.MaxHeight(50), GUILayout.ExpandWidth(true)))
                EditorSceneManager.OpenScene(GetScenePath(sceneName + "_Example_Scene"));

            GUILayout.Space(25f);
            guiContent = new GUIContent("Open Documentation", _icons[3]);
            if (GUILayout.Button(guiContent, GUILayout.MaxHeight(50), GUILayout.ExpandWidth(true)))
            {
                var docPath = SceneDocURL + sceneName;
                Application.OpenURL(docPath);
            }

            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSoapWizard()
        {
            EditorGUILayout.LabelField(
                "The Soap Wizard enables you to:" +
                "\n- Manage and see the value of all SOAP Scriptable Objects in one place" +
                "\n- Filter all SOAP SO by tag" +
                "\n- Add a Soap SO to favorite" +
                "\n- Ping, rename, duplicate, delete a SO" +
                "\n \n The Soap Wizard is the ultimate hub you need when using SOAP", EditorStyles.wordWrappedLabel);
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Open Soap Wizard", GUILayout.MaxHeight(50), GUILayout.MaxWidth(200)))
                SoapWizardWindow.Show();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawSoapTypeCreator()
        {
            EditorGUILayout.LabelField(
                "The Soap type creator enables you to create automatically your own type of SOAP scriptable objects." +
                "\nEasily extend Soap and create the classes your game needs.",
                EditorStyles.wordWrappedLabel);
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Open Soap Type Creator", GUILayout.MaxHeight(50), GUILayout.MaxWidth(200)))
                SoapTypeCreatorWindow.Show();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawEventsDebugWindow()
        {
            EditorGUILayout.LabelField(
                "If you use EventListeners and call methods from your class in the Unity Events, some IDE won't show you that the method is being used." +
                "\nThe Events Debug Window allows you to search for a specific method (by name) to check from which Scriptable Event and EventListener it is called.",
                EditorStyles.wordWrappedLabel);
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Open Events Debug Window", GUILayout.MaxHeight(50), GUILayout.MaxWidth(200)))
                EventsDebugWindow.Show();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawFooter()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(
                "For any suggestions or questions, please reach out on discord or by email : obviousgame.contact@gmail.com",
                EditorStyles.miniLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void SelectCategory(CategoryButton categoryButton)
        {
            _onCategoryButtonClicked = categoryButton.OnClick;
            _currentCategoryButton = categoryButton;
            SoapEditorUtils.WindowLastCategory = categoryButton.Name;
        }

        private void DrawSettings()
        {
            if (_soapWindowSettings == null)
                _soapWindowSettings = new SoapWindowSettings(this);

            _soapWindowSettings.Draw();
        }
    }

    public class CategoryButton
    {
        public string Name;
        public Action OnClick;
        public string GroupName;
        public Texture GroupIcon;
    }
}