//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace easyar
{
    internal class APIDocGenerator : Editor
    {
        // Unity does not generate XML documentation file for DLLs.
        // Even worse, it removes the file after compilation if -doc is used in compiler options.
        // So, let's hack!
        [DidReloadScripts(1)]
        public static void GenerateAPIDoc()
        {
            var gen = true;
            if (EasyARSettings.Instance)
            {
                gen = EasyARSettings.Instance.GenerateXMLDoc;
            }
            if (!gen) { return; }
            GenDocForDLL(UnityPackage.Name, "EasyAR.Sense");
#if EASYAR_EXT_ARFOUNDATION
            GenDocForDLL(UnityPackage.Name + ".ext.arfoundation", "EasyAR.Sense.Ext.ARFoundation");
#endif
#if EASYAR_EXT_NREAL
            GenDocForDLL(UnityPackage.Name + ".ext.nreal", "EasyAR.Sense.Ext.Nreal");
#endif
        }

        static void GenDocForDLL(string pakcage, string lib)
        {
            var src = Path.GetFullPath($"Packages/{pakcage}/Documentation~/{lib}.xml");
            if (!File.Exists(src)) { return; }
            var dstFolder = Path.GetDirectoryName(Application.dataPath) + "/Library/ScriptAssemblies";
            if (!File.Exists(dstFolder + $"/{lib}.dll")) { return; }
            File.Copy(src, dstFolder + $"/{lib}.xml", true);
        }
    }
}
