using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class CgVideoImportSettings
{
    private const string ClipGuid = "bbe322d3d6e074441944e7310d8e410d";

    static CgVideoImportSettings()
    {
        EditorApplication.delayCall += Configure;
    }

    private static void Configure()
    {
        string path = AssetDatabase.GUIDToAssetPath(ClipGuid);
        if (string.IsNullOrEmpty(path)) return;

        VideoClipImporter importer = AssetImporter.GetAtPath(path) as VideoClipImporter;
        if (importer == null) return;

        VideoImporterTargetSettings settings = importer.GetTargetSettings("Default");
        if (settings.enableTranscoding && settings.codec == VideoCodec.H264) return;

        settings.enableTranscoding = true;
        settings.codec = VideoCodec.H264;
        importer.SetTargetSettings("Default", settings);
        importer.SaveAndReimport();
        Debug.Log("CG video configured for H.264 transcoding: " + path);
    }
}
