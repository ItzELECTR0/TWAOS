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

            UnityWebRequest request = UnityWebRequest.PostWwwForm(
                GameCreatorHub.URI_CF + gate,
                string.Empty
            );

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