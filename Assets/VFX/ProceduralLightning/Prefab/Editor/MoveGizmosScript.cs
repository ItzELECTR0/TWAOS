using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

namespace DigitalRuby.ThunderAndLightning.Editor
{
    [InitializeOnLoad]
    public class MoveGizmosScript
    {
        static MoveGizmosScript()
        {
            try
            {
                string destinationPath = Path.Combine(Application.dataPath, "Gizmos");
                Directory.CreateDirectory(destinationPath);
                string[] pngFiles = Directory.GetFiles(Application.dataPath, "LightningPath*.png", SearchOption.AllDirectories);
                foreach (string gizmo in pngFiles)
                {
                    string fileName = Path.GetFileName(gizmo);
                    if (fileName.Equals("LightningPathStart.png", StringComparison.OrdinalIgnoreCase) ||
                        fileName.Equals("LightningPathNext.png", StringComparison.OrdinalIgnoreCase))
                    {
                        string destFile = Path.Combine(destinationPath, fileName);
                        FileInfo srcInfo = new FileInfo(gizmo);
                        FileInfo dstInfo = new FileInfo(destFile);
                        if (!dstInfo.Exists || srcInfo.LastWriteTimeUtc > dstInfo.LastWriteTimeUtc)
                        {
                            srcInfo.CopyTo(dstInfo.FullName, true);
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
