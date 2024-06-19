using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Multiplayer.Editor
{
    // A ScriptableSingleton that syncs its data to and from a file on disk.
    // This is specially necessary for virtual projects where the main Editor serializes the data to a file
    // and the other instances of the Editor needs to reload the data from that file when changed.
    internal class SyncedSingleton<T> : ScriptableSingleton<T> where T : ScriptableObject
    {
        private static bool s_NeedsRegeneration;

        // If the file is updated and the editor is not in focus and entered play mode
        // before focus is regained (e.g. multiplayer play mode), then the regeneration
        // method --called through delayCall-- could be executed after play mode.
        // To avoid this, we reload it on entering play mode state change. It's redundant
        // but safer.
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            ReloadIfNeeded();
        }

        static SyncedSingleton()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            s_NeedsRegeneration = true;
            ((IDisposable)source).Dispose();
            EditorApplication.delayCall += ReloadIfNeeded;
        }

        private static void ReloadIfNeeded()
        {
            if (s_NeedsRegeneration)
            {
                // By deleting the instance the Editor will reload the data from the file.
                DestroyImmediate(instance);
                s_NeedsRegeneration = false;
            }
        }

        private FileSystemWatcher m_FileWatcher;

        protected void OnEnable()
        {
            Assert.IsNull(m_FileWatcher);

            m_FileWatcher = new FileSystemWatcher();
            m_FileWatcher.Path = Path.GetDirectoryName(ScriptableSingleton<T>.GetFilePath());
            m_FileWatcher.Changed += OnFileChanged;
            m_FileWatcher.Created += OnFileChanged;
            m_FileWatcher.EnableRaisingEvents = true;
        }
    }
}
