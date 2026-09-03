using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Zayno.Build
{
    public static class WebGLBuild
    {
        private const string DefaultOutput = "Builds/ZaynoWebGL";
        private static readonly string[] RequiredArabicAssets =
        {
            "Assets/_config/content_Arabic/DB Arabic_LearningBlock.asset",
            "Assets/_config/content_Arabic/DB Arabic_PlaySession.asset",
            "Assets/_config/content_Arabic/DB Arabic_Letter.asset",
            "Assets/_config/content_Arabic/DB Arabic_Word.asset"
        };

        [MenuItem("Zayno/Build Antura WebGL")]
        public static void Perform()
        {
            ValidateArabicContent();
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL))
                throw new InvalidOperationException("Unable to activate the WebGL build target.");

            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult addressablesResult);
            if (!string.IsNullOrEmpty(addressablesResult.Error))
                throw new InvalidOperationException("Addressables build failed: " + addressablesResult.Error);

            var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
            if (scenes.Length == 0)
                throw new InvalidOperationException("No enabled scenes were found in EditorBuildSettings.");

            var output = Environment.GetEnvironmentVariable("ANTURA_WEBGL_OUTPUT");
            if (string.IsNullOrWhiteSpace(output)) output = DefaultOutput;
            Directory.CreateDirectory(output);
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
            PlayerSettings.WebGL.decompressionFallback = false;

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = output,
                target = BuildTarget.WebGL,
                options = BuildOptions.CleanBuildCache
            });

            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("WebGL build failed: " + report.summary.result + " (" + report.summary.totalErrors + " errors).");

            Debug.Log("ANTURA_WEBGL_BUILD_OK path=" + output + " size=" + report.summary.totalSize);
        }

        private static void ValidateArabicContent()
        {
            var missing = RequiredArabicAssets.Where(path => !File.Exists(path)).ToArray();
            if (missing.Length > 0)
                throw new FileNotFoundException("Arabic content is incomplete. Missing: " + string.Join(", ", missing));

            var sessions = File.ReadAllText(RequiredArabicAssets[1]);
            if (!sessions.Contains("_Stage: 6") || !sessions.Contains("_Type: MiniGame") || !sessions.Contains("_Type: Assessment"))
                throw new InvalidDataException("Arabic progression validation failed: stages, minigames or assessments are missing.");

            Debug.Log("ANTURA_ARABIC_CONTENT_OK requiredDatabases=4 stage6=true minigames=true assessments=true");
        }
    }
}