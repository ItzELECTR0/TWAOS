using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GPUInstancer.GPUIPackageImporterData;

namespace GPUInstancer
{
    [Serializable]
    public class GPUIPackageImporterImportedData : ScriptableObject
    {
        public List<ImportedPackageInfo> importedPackageInfos;
    }
}