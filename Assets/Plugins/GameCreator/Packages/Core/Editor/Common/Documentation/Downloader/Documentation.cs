using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace GameCreator.Editor.Common
{
    public static class Documentation
    {
        const string URL = "https://docs.gamecreator.io/assets/public/documentation.pdf";
        const string DOWNLOAD_PATH = EditorPaths.PACKAGES + "Documentation.pdf";

        const string DWN_TITLE = "Connecting with Game Creator servers...";
        const string DWN_INFO = "Please wait. This will take just a few seconds...";

        const string ERR_HTTP = "Http Error: There was an error when downloading the file.";
        const string ERR_NETWORK = "Network Error: There was an error when downloading the file.";

        const string SUC_TITLE = "Download complete.";
        const string SUC_INFO = "The latest documentation version was successfully downloaded.";

        const string OPT_OK = "Accept";

        // STATIC VARIABLES: ----------------------------------------------------------------------

        private static UnityWebRequest Request;
        private static bool InteractiveRequest;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        [MenuItem("Game Creator/Update Documentation", priority = 11)]
        public static void UpdateDocumentation()
        {
            DownloadDocumentation(true);
        }

        public static void DownloadDocumentation(bool interactive = true)
        {
            if (EditorApplication.isPlaying) return;
            if (EditorApplication.isCompiling) return;
            if (Request != null) return;

            Request = UnityWebRequest.Get(URL);
            InteractiveRequest = interactive;

            Request.downloadHandler = new DownloadHandlerFile(DOWNLOAD_PATH, false);
            Request.disposeDownloadHandlerOnDispose = true;

            EditorApplication.update -= Update;
            EditorApplication.update += Update;
            Request.SendWebRequest();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void Update()
        {
            if (Request == null)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update -= Update;

                return;
            }

            bool disposeRequest = false;
            bool isCancelled = EditorUtility.DisplayCancelableProgressBar(
                DWN_TITLE, DWN_INFO,
                Request.downloadProgress
            );

            if (isCancelled) disposeRequest = true;

            if (Request.isDone)
            {
                disposeRequest = true;

                if (Request.result == UnityWebRequest.Result.ProtocolError)
                {
                    if (InteractiveRequest) EditorUtility.DisplayDialog(ERR_HTTP, Request.error, OPT_OK);
                }
                else if (Request.result == UnityWebRequest.Result.ConnectionError)
                {
                    if (InteractiveRequest) EditorUtility.DisplayDialog(ERR_NETWORK, Request.error, OPT_OK);
                }
                else
                {
                    AssetDatabase.Refresh();
                    if (InteractiveRequest)
                    {
                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog(SUC_TITLE, SUC_INFO, OPT_OK);

                        Object asset = AssetDatabase.LoadMainAssetAtPath(DOWNLOAD_PATH);
                        EditorGUIUtility.PingObject(asset);
                    }
                }
            }

            if (disposeRequest)
            {
                Request.Abort();
                Request.Dispose();
                Request = null;

                EditorApplication.update -= Update;
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
