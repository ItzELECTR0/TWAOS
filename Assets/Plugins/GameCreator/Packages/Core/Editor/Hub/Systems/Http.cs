using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace GameCreator.Editor.Hub
{
    internal static class Http
    {
        [Serializable]
        public struct ReceiveResponse
        {
            public ReceiveData result;
        }

        [Serializable]
        public struct ReceiveData
        {
            public bool error;
            public string data;
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public static bool IsRequesting { get; private set; }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static async Task<ReceiveData> Send(string gate, object data)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return new ReceiveData
                {
                    error = true,
                    data = "No internet connection"
                };
            }

            if (IsRequesting)
            {
                return new ReceiveData
                {
                    error = true,
                    data = "Cannot send another request while one is being processed"
                };
            }

            IsRequesting = true;
            string post = $"{{ \"data\": {EditorJsonUtility.ToJson(data)} }}";

            // TODO: [18/03/2023] Remove once Unity 2022.3 LTS is released
            
            #if UNITY_2022_2_OR_NEWER
            UnityWebRequest request = UnityWebRequest.PostWwwForm(
                GameCreatorHub.URI_CF + gate,
                string.Empty
            );
            #else
            UnityWebRequest request = UnityWebRequest.Post(
                GameCreatorHub.URI_CF + gate,
                string.Empty
            );
            #endif

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(post))
            {
                contentType = "application/json"
            };

            UnityWebRequestAsyncOperation handle = request.SendWebRequest();
            while (!handle.isDone) await Task.Yield();

            IsRequesting = false;

            if (handle.webRequest.responseCode != 200)
            {
                return new ReceiveData
                {
                    error = true,
                    data = $"Error while communicating with our server: {handle.webRequest.responseCode}"
                };
            }

            string receiveText = handle.webRequest.downloadHandler.text;
            
            ReceiveResponse receiveResponse = JsonUtility.FromJson<ReceiveResponse>(receiveText);
            ReceiveData receiveData = receiveResponse.result;

            request.Dispose();
            return receiveData;
        }
    }
}