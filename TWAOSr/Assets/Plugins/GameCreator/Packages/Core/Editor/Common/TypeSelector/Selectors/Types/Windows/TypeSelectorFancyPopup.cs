using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    public class TypeSelectorFancyPopup : EditorWindow
    {
        private const int LEFT_BUTTON = 0;
        
        private const int MIN_WIDTH = 200;
        private const int MIN_HEIGHT = 400;
        private const int ITEM_HEIGHT = 24;
        
        private const int TRANSITION_DURATION = 350;
        
        private const string ITEM_NAME_IMAGE = "GC-Tsf-Item-Image";
        private const string ITEM_NAME_LABEL = "GC-Tsf-Item-Label";
        private const string ITEM_NAME_ARROW = "GC-Tsf-Item-Arrow";
        
        private const string STYLE_PATH_SELECTOR = "TypeSelector/StyleSheets/TypeSelectorFancy";

        private static readonly IIcon ICON_CHEVRON_L = new IconChevronLeft(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_CHEVRON_R = new IconChevronRight(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_FOLDER = new IconFolderSolid(ColorTheme.Type.TextLight);

        // STATIC PROPERTIES: ---------------------------------------------------------------------

        private static TypeSelectorFancyPopup Window;

        // MEMBERS: -------------------------------------------------------------------------------

        private string m_TypeTitle;
        
        private Type m_Type;
        private Action<Type> m_OnSelect;
        
        private VisualElement m_Head;
        private VisualElement m_Body;
        private VisualElement m_Foot;
        
        private TextField m_SearchField;
        
        private readonly Stack<VisualElement> m_ContentStack = new Stack<VisualElement>();
        private VisualElement m_ContentSearch;

        private readonly Stack<TypePage> m_PageStack = new Stack<TypePage>();

        // INITIALIZERS: --------------------------------------------------------------------------

        public static void Open(VisualElement element, Type type, Action<Type> onSelect)
        {
            if (Window != null)
            {
                Window.Close();
                return;
            }
            
            Search.Index.RequireIndex(type);
            TypeBook.Awake(type);

            Window = CreateInstance<TypeSelectorFancyPopup>();

            Window.m_TypeTitle = TypeUtils.GetTitleFromType(type);
            Window.m_Type = type;
            Window.m_OnSelect = onSelect;

            Rect rectActivator = new Rect(
                focusedWindow.position.x + element.worldBound.x,
                focusedWindow.position.y + element.worldBound.y,
                element.worldBound.width,
                element.worldBound.height
            );
            
            Vector2 windowSize = new Vector2(
                Math.Max(element.resolvedStyle.width, MIN_WIDTH),
                MIN_HEIGHT
            );
            
            Window.ShowAsDropDown(rectActivator, windowSize);
        }

        private void OnDestroy()
        {
            Window = null;
        }

        private void CreateGUI()
        {
            string customUSS = PathUtils.Combine(
                EditorPaths.COMMON, 
                STYLE_PATH_SELECTOR
            );
            
            StyleSheet[] sheets = StyleSheetUtils.Load(customUSS);
            foreach (StyleSheet sheet in sheets)
            {
                this.rootVisualElement.styleSheets.Add(sheet);
            }
            
            this.rootVisualElement.name = "GC-Selector-Root";
            
            this.m_Head = new VisualElement { name = "GC-Selector-Head" };
            this.m_Body = new VisualElement { name = "GC-Selector-Body" };
            this.m_Foot = new VisualElement { name = "GC-Selector-Foot" };
            
            this.rootVisualElement.Add(this.m_Head);
            this.rootVisualElement.Add(this.m_Body);
            this.rootVisualElement.Add(this.m_Foot);
            
            this.SetupHead();
            this.SetupBody();
            this.SetupFoot();

            this.m_SearchField.Focus();
        }

        // CONFIGURE HEAD: ------------------------------------------------------------------------

        private void SetupHead()
        {
            VisualElement headContainer = new VisualElement();
            headContainer.AddToClassList("gc-tsf-head--container");
            
            this.m_SearchField = new TextField();

            this.m_SearchField.RegisterValueChangedCallback(changeEvent =>
            {
                bool isEmptySearch = string.IsNullOrEmpty(changeEvent.newValue);
                
                if (this.m_ContentStack.TryPeek(out VisualElement previousPage))
                {
                    previousPage.SetEnabled(isEmptySearch);
                }
                
                if (isEmptySearch)
                {
                    this.SetupSearchPage(null);
                    return;
                }
                
                IEnumerable<Type> types = Search.Index.Get(this.m_Type, changeEvent.newValue);
                Trie<Type> pagesTrie = new Trie<Type>(changeEvent.newValue, null);
                
                foreach (Type type in types)
                {
                    string typeName = type.ToString();
                    pagesTrie.AddChild(new Trie<Type>(typeName, type));
                }
                
                TypePage typePage = new TypePage(pagesTrie, true);
                this.SetupSearchPage(typePage);
            });
            
            this.m_SearchField.RegisterCallback<KeyDownEvent>(eventKeyDown =>
            {
                if (eventKeyDown.keyCode != KeyCode.DownArrow) return;
                
                VisualElement page = string.IsNullOrEmpty(this.m_SearchField.value)
                    ? this.m_ContentStack.Peek()
                    : this.m_ContentSearch;
                    
                PutFocusOnList(page);
            });
            
            headContainer.Add(this.m_SearchField);
            this.m_Head.Add(headContainer);
        }

        // CONFIGURE BODY: ------------------------------------------------------------------------

        private void SetupBody()
        {
            TypePage rootPage = TypeBook.Fetch(this.m_Type)?.First;
            this.SetupNavigationPage(rootPage, false, false);
        }

        private void SetupNavigationPage(TypePage typePage, bool transition, bool setFocus)
        {
            VisualElement page = this.CreatePage(typePage, transition, false, setFocus);

            if (this.m_ContentStack.Count > 0)
            {
                this.m_ContentStack.Peek().SetEnabled(false);
            }
            
            this.m_ContentStack.Push(page);
            this.m_Body.Add(page);
        }

        private void SetupSearchPage(TypePage typePage)
        {
            if (this.m_ContentSearch != null && this.m_Body.Contains(this.m_ContentSearch))
            {
                this.m_Body.Remove(this.m_ContentSearch);
                this.m_PageStack.Pop();
            }
            
            if (typePage == null) return;

            this.m_ContentSearch = this.CreatePage(typePage, false, true, true);
            this.m_Body.Add(this.m_ContentSearch);
        }

        private VisualElement CreatePage(TypePage typePage, bool transition, bool isSearch, bool setFocus)
        {
            this.m_PageStack.Push(typePage);

            VisualElement page = new VisualElement();
            page.AddToClassList("gc-tsf-body--page");

            VisualElement header = new VisualElement
            {
                focusable = false
            };

            Label headerTitle = new Label
            {
                focusable = false,
                text = string.IsNullOrEmpty(this.m_PageStack.Peek()?.Title)
                    ? this.m_TypeTitle
                    : this.m_PageStack.Peek().Title
            };

            if (isSearch || this.m_ContentStack.Count > 0)
            {
                switch (isSearch)
                {
                    case true:
                        header.RegisterCallback<MouseDownEvent>(
                            _ => this.m_SearchField.value = string.Empty
                        );
                        break;
                    
                    case false:
                        header.RegisterCallback<MouseDownEvent>(this.PreviousPage);
                        break;
                }
                
                headerTitle.Add(new Image
                {
                    focusable = false,
                    image = ICON_CHEVRON_L.Texture
                });
            }

            header.AddToClassList("gc-tsf-body--header");
            headerTitle.AddToClassList("gc-tsf-body--header-title");

            header.Add(headerTitle);
            
            ListView content = new ListView(
                this.m_PageStack.Peek()?.Content,
                ITEM_HEIGHT,
                this.ContentMakeItem,
                this.ContentBindItem
            )
            {
                focusable = true,
                delegatesFocus = true,
                selectionType = SelectionType.Single
            };

            content.RegisterCallback<FocusInEvent>(eventFocus =>
            {
                if (eventFocus.target is ListView {selectedIndex: -1} eventListView)
                {
                    eventListView.selectedIndex = 0;
                }
            });

            content.selectionChanged += ContentSelectItem;
            content.itemsChosen += ContentChooseItem;

            content.AddToClassList("gc-tsf-body--content");
            content.RegisterCallback<KeyDownEvent>(eventKeydown =>
            {
                int index = content.selectedIndex;
                List<TypeNode> nodes = this.m_PageStack.Peek()?.Content;
                TypeNode node = nodes != null && index >= 0 && nodes.Count > index 
                    ? nodes[index] 
                    : null;
                
                switch (eventKeydown.keyCode)
                {
                    case KeyCode.Escape:
                        if (isSearch)
                        {
                            this.m_SearchField.value = string.Empty;
                            this.m_SearchField.Focus();
                            eventKeydown.StopPropagation();
                        }
                        else if (this.m_PageStack.Count > 1)
                        {
                            this.PreviousPage();
                            eventKeydown.StopPropagation();   
                        }
                        
                        break;
                    
                    case KeyCode.Return:
                    {
                        if (node is TypeNodeValue)
                        {
                            this.ContentChooseItem(new [] { node });
                            eventKeydown.StopPropagation();
                        }
            
                        break;
                    }
            
                    default:
                        this.m_SearchField.Focus();
                        eventKeydown.StopPropagation();
                        break;
                }
            }, TrickleDown.TrickleDown);

            page.Add(header);
            page.Add(content);

            if (!isSearch && setFocus) page.schedule
                .Execute(() => PutFocusOnList(page))
                .StartingIn(1);

            if (transition)
            {
                page.experimental.animation.Start(
                    1f, 0f, TRANSITION_DURATION,
                    Transition
                );
            }

            return page;
        }

        private void ContentSelectItem(IEnumerable<object> list)
        {
            foreach (TypeNode node in list)
            {
                switch (node)
                {
                    case TypeNodeFolder nodeFolder:
                        this.SetupDocumentation(null);
                        break;
                    
                    case TypeNodeValue nodeValue:
                        this.SetupDocumentation(nodeValue.Value);
                        break;
                }
            }
        }
        
        private void ContentChooseItem(IEnumerable<object> list)
        {
            foreach (TypeNode node in list)
            {
                switch (node)
                {
                    case TypeNodeFolder nodeFolder:
                        this.SetupNavigationPage(nodeFolder.Page, true, true);
                        break;
                    
                    case TypeNodeValue nodeValue:
                        this.m_OnSelect?.Invoke(nodeValue.Value);
                        this.Close();
                        break;
                }
            }
        }

        private VisualElement ContentMakeItem()
        {
            VisualElement item = new VisualElement();
            item.focusable = false;
            
            item.AddToClassList("gc-tsf-body--content-item");

            Image image = new Image { name = ITEM_NAME_IMAGE };
            Label label = new Label { name = ITEM_NAME_LABEL };
            Image arrow = new Image { name = ITEM_NAME_ARROW };

            arrow.image = ICON_CHEVRON_R.Texture;

            image.focusable = false;
            label.focusable = false;
            arrow.focusable = false;

            item.Add(image);
            item.Add(label);
            item.Add(arrow);

            return item;
        }

        private void ContentBindItem(VisualElement element, int index)
        {
            if (index >= this.m_PageStack.Peek()?.Content.Count) return;
            
            TypeNode node = this.m_PageStack.Peek()?.Content[index];
            element.Query<Label>(ITEM_NAME_LABEL).First().text = node?.Name;

            Image arrow = element.Query<Image>(ITEM_NAME_ARROW).First();
            Image image = element.Query<Image>(ITEM_NAME_IMAGE).First();
            
            switch (node)
            {
                case TypeNodeFolder nodeFolder:
                    image.image = ICON_FOLDER.Texture;
                    arrow.style.display = DisplayStyle.Flex;
                    break;

                case TypeNodeValue nodeValue:
                    image.image = nodeValue.Texture;
                    arrow.style.display = DisplayStyle.None;
                    break;
            }

            element.userData = index;
            
            element.UnregisterCallback<MouseEnterEvent>(this.OnMouseOverItem);
            element.UnregisterCallback<MouseDownEvent>(this.OnMouseClickItem);
            
            element.RegisterCallback<MouseEnterEvent>(this.OnMouseOverItem);
            element.RegisterCallback<MouseDownEvent>(this.OnMouseClickItem);
        }
        
        private void OnMouseOverItem(MouseEnterEvent mouseEvent)
        {
            VisualElement element = mouseEvent.currentTarget as VisualElement;
            ScrollView scroll = element?.parent as ScrollView;
            ListView list = scroll?.parent as ListView;

            int index = (int?) element?.userData ?? -1;
            list?.SetSelection(index);
        }

        private void OnMouseClickItem(MouseDownEvent mouseEvent)
        {
            if (mouseEvent.button != LEFT_BUTTON)
            {
                this.PreviousPage(mouseEvent);
                return;
            }
            
            VisualElement element = mouseEvent.currentTarget as VisualElement;
            int index = (int?) element?.userData ?? -1;
            
            this.ContentChooseItem(new []
            {
                this.m_PageStack.Peek()?.Content[index]
            });
        }
        
        private void PreviousPage(MouseDownEvent mouseEvent = null)
        {
            if (this.m_ContentStack.Count <= 1) return;
            if (this.m_PageStack.Count <= 1) return;
            
            VisualElement page = this.m_ContentStack.Pop();
            if (this.m_ContentStack.TryPeek(out VisualElement previousPage))
            {
                previousPage.SetEnabled(true);
                PutFocusOnList(previousPage);
            }
            
            this.m_PageStack.Pop();
            
            page.experimental.animation.Start(
                0f, 1f, TRANSITION_DURATION, 
                Transition
            );

            this.SetupDocumentation(null);
            
            page.schedule
                .Execute(() => page.parent.Remove(page))
                .ExecuteLater(TRANSITION_DURATION);
        }

        // PRIVATE STATIC METHODS: ----------------------------------------------------------------

        private static void Transition(VisualElement element, float t)
        {
            element.style.marginLeft = t * t * Window.position.width;
        }
        
        private static void PutFocusOnList(VisualElement page)
        {
            page.Query<ListView>().First()?.Focus();
        }

        // CONFIGURE FOOT: ------------------------------------------------------------------------

        private void SetupFoot()
        {
            this.SetupDocumentation(null);
        }

        private void SetupDocumentation(Type focusType)
        {
            this.m_Foot.Clear();
            
            if (focusType == null)
            {
                this.m_Foot.style.display = DisplayStyle.None;
                return;
            }
            
            this.m_Foot.Add(new DocumentationSummary(focusType));
            this.m_Foot.style.display = DisplayStyle.Flex;
        }
    }
}