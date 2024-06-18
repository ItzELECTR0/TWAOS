using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(WelcomeRepository))]
    public class WelcomeRepositoryDrawer : PropertyDrawer
    {
        private const string USS_PATH = EditorPaths.COMMON + "Settings/StyleSheets/Welcome";

        private const string NAME_ROOT = "GC-Welcome-Root";
        private const string NAME_HEAD = "GC-Welcome-Head";
        private const string NAME_BODY = "GC-Welcome-Body";
        private const string NAME_FOOT = "GC-Welcome-Foot";

        private const string NAME_CONTENT = "GC-Welcome-Body-Content";
        private const string NAME_BULLETS = "GC-Welcome-Body-Bullets";
        
        private const string CLASS_PAGE = "gc-welcome-body-page";
        private const string CLASS_BUTTON_L = "gc-welcome-body-button-left";
        private const string CLASS_BUTTON_R = "gc-welcome-body-button-right";
        private const string CLASS_BULLET_SELECT = "gc-welcome-body-bullets-select";

        private const float LABEL_MARGIN = 5f;
        private const string LABEL_STARTUP = "Show this Panel on Startup";
        private const string PROP_STARTUP = "m_OpenOnStartup";
        
        private static readonly IIcon BUTTON_L = new IconChevronLeft(Color.white);
        private static readonly IIcon BUTTON_R = new IconChevronRight(Color.white);
        
        private const int SCROLL_DURATION = 300;

        private static int SCROLL_CURRENT_INDEX = 0;
        private static int SCROLL_TARGET_INDEX = 0;
        private static long SCROLL_TIME = 0;
        
        // MEMBERS: -------------------------------------------------------------------------------

        private VisualElement m_Content;

        private VisualElement m_ButtonScrollL;
        private VisualElement m_ButtonScrollR;
        private VisualElement m_Bullets;

        private WelcomeData m_WelcomeData;
        private WelcomeTextures m_WelcomeTextures;
        
        // PAINT METHOD: --------------------------------------------------------------------------
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement { name = NAME_ROOT };

            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in styleSheets) root.styleSheets.Add(sheet);

            VisualElement head = new VisualElement { name = NAME_HEAD };
            VisualElement body = new VisualElement { name = NAME_BODY };
            VisualElement foot = new VisualElement { name = NAME_FOOT };

            root.Add(head);
            root.Add(body);
            root.Add(foot);
            
            this.CreateHead(head, property);
            this.CreateBody(body, property);
            this.CreateFoot(foot, property);

            return root;
        }

        // HEAD: ----------------------------------------------------------------------------------
        
        private void CreateHead(VisualElement head, SerializedProperty property)
        {
            LabelTitle title = new LabelTitle("Welcome to Game Creator");
            head.Add(title);
        }
        
        // BODY: ----------------------------------------------------------------------------------

        private void CreateBody(VisualElement body, SerializedProperty property)
        {
            SCROLL_CURRENT_INDEX = 0;
            SCROLL_TARGET_INDEX = 0;
            
            this.m_Content = new VisualElement
            {
                name = NAME_CONTENT,
                style = { marginLeft = 0 }
            };

            this.m_ButtonScrollL = new Image { image = BUTTON_L.Texture };
            this.m_ButtonScrollR = new Image { image = BUTTON_R.Texture };
            
            this.m_ButtonScrollL.AddToClassList(CLASS_BUTTON_L);
            this.m_ButtonScrollR.AddToClassList(CLASS_BUTTON_R);
            
            this.m_ButtonScrollL.RegisterCallback<ClickEvent>(_ => this.ScrollTo(SCROLL_TARGET_INDEX - 1));
            this.m_ButtonScrollR.RegisterCallback<ClickEvent>(_ => this.ScrollTo(SCROLL_TARGET_INDEX + 1));

            this.m_Bullets = new VisualElement { name = NAME_BULLETS };

            this.m_WelcomeData = WelcomeManager.Data;
            this.m_WelcomeTextures = WelcomeManager.Textures;

            for (int i = 0; i < this.m_WelcomeData.pages.Length; ++i)
            {
                VisualElement bullet = this.CreateBullet(i);
                this.m_Bullets.Add(bullet);
            }

            foreach (WelcomePage welcomePage in this.m_WelcomeData.pages)
            {
                VisualElement page = new VisualElement();
                page.AddToClassList(CLASS_PAGE);

                Texture2D texture = GetImage(welcomePage);
                
                Image image = new Image
                {
                    image = texture,
                    style =
                    {
                        backgroundColor = ColorUtils.Parse(welcomePage.color)
                    }
                };
                page.Add(image);
                
                image.RegisterCallback<ClickEvent>(_ =>
                {
                    bool success = WelcomeCommands.Parse(welcomePage.steps);
                    if (!success) this.ScrollTo(SCROLL_TARGET_INDEX + 1);
                });
                
                this.m_Content.Add(page);
            }

            this.RefreshControls();
            this.RefreshBullets();

            body.Add(this.m_Content);
            body.Add(this.m_ButtonScrollL);
            body.Add(this.m_ButtonScrollR);
            body.Add(this.m_Bullets);
        }

        private Texture2D GetImage(WelcomePage welcomePage)
        {
            Texture2D texture = WelcomeInternalTextures.Get(welcomePage.image);
            if (texture != null) return texture;

            texture = new Texture2D(1,1);
            string imageID = WelcomeManager.HashFromPath(welcomePage.image);

            if (this.m_WelcomeTextures.TryGetValue(imageID, out string base64))
            {
                byte[] data = Convert.FromBase64String(base64);
                texture.LoadImage(data);
                texture.hideFlags = HideFlags.HideAndDontSave;
            }

            return texture;
        }

        private VisualElement CreateBullet(int index)
        {
            VisualElement bullet = new VisualElement();
            bullet.RegisterCallback<ClickEvent>(_ => this.ScrollTo(index));
            return bullet;
        }

        private void ScrollTo(int index)
        {
            if (this.m_Content == null) return;
            if (index < 0 || index >= this.m_WelcomeData.pages.Length) return;

            SCROLL_CURRENT_INDEX = SCROLL_TARGET_INDEX;
            SCROLL_TARGET_INDEX = index;
            SCROLL_TIME = -1;

            this.RefreshControls();
            this.RefreshBullets();
            
            this.m_Content.schedule
                .Execute(this.UpdateScrollPosition)
                .Every(1)
                .ForDuration(SCROLL_DURATION);
        }

        private void UpdateScrollPosition(TimerState timerState)
        {
            if (SCROLL_TIME == -1) SCROLL_TIME = timerState.start;

            float t = (timerState.now - SCROLL_TIME) / (float) SCROLL_DURATION;
            float index = Easing.QuadInOut(
                SCROLL_CURRENT_INDEX, 
                SCROLL_TARGET_INDEX,
                Mathf.Clamp01(t)
            );

            this.m_Content.style.marginLeft = new Length(index * -100f, LengthUnit.Percent);
        }

        private void RefreshControls()
        {
            this.m_ButtonScrollL.style.display = SCROLL_TARGET_INDEX > 0
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            int pagesLength = this.m_WelcomeData.pages.Length;
            this.m_ButtonScrollR.style.display = SCROLL_TARGET_INDEX < pagesLength - 1
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        private void RefreshBullets()
        {
            for (int i = 0; i < this.m_Bullets.childCount; ++i)
            {
                if (SCROLL_TARGET_INDEX == i)
                {
                    this.m_Bullets[i].AddToClassList(CLASS_BULLET_SELECT);
                }
                else
                {
                    this.m_Bullets[i].RemoveFromClassList(CLASS_BULLET_SELECT);
                }
            }
        }

        // FOOT: ----------------------------------------------------------------------------------
        
        private void CreateFoot(VisualElement foot, SerializedProperty property)
        {
            SerializedProperty openOnStartup = property.FindPropertyRelative(PROP_STARTUP);
            PropertyField fieldOpenOnStartup = new PropertyField(openOnStartup, string.Empty);
            Label labelOpenOnStartup = new Label
            {
                text = LABEL_STARTUP,
                style = { marginLeft = LABEL_MARGIN }
            };

            foot.Add(fieldOpenOnStartup);
            foot.Add(labelOpenOnStartup);
        }
    }
}