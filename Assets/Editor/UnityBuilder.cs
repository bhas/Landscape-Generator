using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class UnityBuilder
{
    static string[] SCENES = FindEnabledEditorScenes();

    static string TARGET_NAME = "TestProject";
    static string TARGET_DIR = "Builds";

    // method for building for windows
    [MenuItem("Build/Windows")]
    static void BuildForWindows()
    {
        string path = TARGET_DIR + "/" + TARGET_NAME + ".exe";
        Directory.CreateDirectory(TARGET_DIR);
        GenericBuild(SCENES, path, BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    // method for building for android
    [MenuItem("Build/Android")]
    static void BuildForAndroid()
    {
        string path = TARGET_DIR + "/" + TARGET_NAME + ".apk";
        Directory.CreateDirectory(TARGET_DIR);
        GenericBuild(SCENES, path, BuildTarget.Android, BuildOptions.None);
    }

    // find all the scenes from the build settings
    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    // generic method for building the game
    private static void GenericBuild(string[] scenes, string filePath, BuildTarget buildTarget, BuildOptions build_options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);
        string res = BuildPipeline.BuildPlayer(scenes, filePath, buildTarget, build_options);
        if (res.Length > 0)
        {
            throw new System.Exception("BuildPlayer failure: " + res);
        }
    }
}