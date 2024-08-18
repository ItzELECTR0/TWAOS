using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Hub
{
    public class HubExplorerContentDetails : VisualElement
    {
        private const string NAME_INTERACTIONS = "GC-Hub-Content-Details-Interactions";
        private const string NAME_SCROLLVIEW = "GC-Hub-Content-Details-ScrollView";
        
        // MEMBERS: -------------------------------------------------------------------------------

        private string m_PackageID;
        
        private readonly HubExplorerWindow m_Window;
        
        private ScrollView m_Content;
        private VisualElement m_Interactions;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public HubExplorerContentDetails(HubExplorerWindow window)
        {
            this.m_Window = window;
        }

        public void OnEnable()
        {
            this.m_Window.EventChangeSelection += this.OnChangeSelection;
            Upload.EventAfterUpload += this.Refresh;
        }

        public void OnDisable()
        {
            this.m_Window.EventChangeSelection -= this.OnChangeSelection;
            Upload.EventAfterUpload -= this.Refresh;
        }
        
        // CALLBACK METHODS: ----------------------------------------------------------------------
        
        private void Refresh()
        {
            int index = this.m_Window.Content.ContentList.Index;
            this.OnChangeSelection(index);
        }
        
        private void OnChangeSelection(int index)
        {
            this.Clear();
            
            this.m_Content = new ScrollView(ScrollViewMode.Vertical) { name = NAME_SCROLLVIEW };
            this.Add(this.m_Content);

            this.m_Interactions = new VisualElement { name = NAME_INTERACTIONS };
            this.Add(this.m_Interactions);
            
            if (index >= GameCreatorHub.Data.Length || index < 0) return;

            HitData package = GameCreatorHub.Data[index];
            this.m_PackageID = GameCreatorHub.Data[index].objectID;
            
            if (package == null) return;
            
            this.m_Content.Add(new DocumentationHub(package));
            
            Button buttonView = new Button { text = "More information" };
            buttonView.clicked += () =>
            {
                string uri = GameCreatorHub.URI_PACKAGE + this.m_PackageID;
                Application.OpenURL(uri);
            };
            
            Button buttonSelect = new Button { text = "Select" };
            buttonSelect.SetEnabled(false);
            
            Button buttonInstall = new Button { text = "Install" };
            buttonInstall.AddToClassList("gc-bold");
            buttonInstall.clicked += this.DownloadPackage;

            string candidatePath = Download.GetInstallationPath(
                package.type,
                package.category, 
                package.filename
            );
            
            TextAsset currentAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(candidatePath);
            
            Button buttonUpload = new Button { text = "Publish" };
            buttonUpload.AddToClassList("gc-bold");
            buttonUpload.style.display = DisplayStyle.None;
            buttonUpload.clicked += () => _ = Upload.Send(currentAsset);
            
            if (currentAsset != null)
            {
                string typeName = Path.GetFileNameWithoutExtension(package.filename);
                Type candidateType = TypeUtils.GetTypeFromName(typeName);

                if (candidateType != null)
                {
                    VersionAttribute candidateVersion = candidateType
                        .GetCustomAttributes<VersionAttribute>(true)
                        .FirstOrDefault();
                
                    VersionAttribute serverVersion = new VersionAttribute(
                        package.version.x,
                        package.version.y,
                        package.version.z
                    );

                    int compareVersion = serverVersion.CompareTo(candidateVersion);
                    switch (compareVersion)
                    {
                        case 0:
                            buttonInstall.SetEnabled(false);
                            buttonInstall.text = "Up to date";
                            break;
                        
                        case 1:
                            buttonInstall.SetEnabled(true);
                            buttonInstall.text = "Update";
                            break;
                        
                        case -1:
                            buttonInstall.SetEnabled(true);
                            buttonInstall.text = "Downgrade";
                            buttonUpload.style.display = DisplayStyle.Flex;
                            break;
                    }

                    buttonSelect.clicked += () =>
                    {
                        EditorGUIUtility.PingObject(currentAsset);
                        Selection.activeObject = currentAsset;
                    };
                    
                    buttonSelect.SetEnabled(true);
                }
            }

            VisualElement spacer = new VisualElement();
            spacer.AddToClassList("gc-spacer");
            
            this.m_Interactions.Add(buttonView);
            this.m_Interactions.Add(buttonSelect);
            this.m_Interactions.Add(spacer);
            this.m_Interactions.Add(buttonInstall);
            this.m_Interactions.Add(buttonUpload);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private async void DownloadPackage()
        {
            string installPath = await Download.Install(this.m_PackageID, true);
            if (string.IsNullOrEmpty(installPath)) return;
            
            TextAsset installAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(installPath);
            if (installAsset != null) EditorGUIUtility.PingObject(installAsset);
        }
    }
}