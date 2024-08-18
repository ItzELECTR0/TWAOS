using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace GameCreator.Editor.Hub
{
    internal static class Collection
    {
        [Serializable]
        public struct OutSearch
        {
            public string query;
        }

        // CONSTANTS: -----------------------------------------------------------------------------

        private const string CF_SEARCH = "editorSearch";
        private const string CF_LATEST = "editorLatest";

        // PROPERTIES: ----------------------------------------------------------------------------

        public static bool IsFetching { get; private set; }

        // EVENTS: --------------------------------------------------------------------------------

        public static event Action<bool> EventIsFetching;
        public static event Action<string, HitPayload> EventFetch;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static async Task<HitPayload> Latest()
        {
            if (IsFetching) return null;

            IsFetching = true;
            EventIsFetching?.Invoke(true);
            
            Http.ReceiveData response = await Http.Send(CF_LATEST, string.Empty);

            IsFetching = false;
            EventIsFetching?.Invoke(false);

            if (response.error)
            {
                EditorUtility.DisplayDialog(
                    "Error while fetching latest packages",
                    response.data,
                    "Accept"
                );

                return null;
            }
            
            HitPayload payload = JsonUtility.FromJson<HitPayload>(response.data);
            EventFetch?.Invoke(string.Empty, payload);
            
            return payload;
        }

        public static async Task<HitPayload> Search(string query)
        {
            if (IsFetching) return null;

            IsFetching = true;
            EventIsFetching?.Invoke(true);

            OutSearch data = new OutSearch
            {
                query = query
            };
            
            Http.ReceiveData response = await Http.Send(CF_SEARCH, data);

            IsFetching = false;
            EventIsFetching?.Invoke(false);

            if (response.error)
            {
                EditorUtility.DisplayDialog(
                    "Error while searching",
                    response.data,
                    "Accept"
                );

                return null;
            }

            HitPayload payload = JsonUtility.FromJson<HitPayload>(response.data);
            EventFetch?.Invoke(query, payload);

            return payload;
        }
    }
}