using System.Collections.Generic;
using UnityEditor;

public class UnityBuilder
{
    static string[] SCENES = FindEnabledEditorScenes();

    static string TARGET_NAME = "TestProject";
    static string TARGET_DIR = "Builds";

    [MenuItem("Build/Windows")]
    static void BuildForWindows()
    {
        string fileName = TARGET_NAME + ".exe";
        GenericBuild(SCENES, TARGET_DIR + "/" + fileName, BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    [MenuItem("Build/Android")]
    static void BuildForAndroid()
    {
        string fileName = TARGET_NAME + ".apk";
        GenericBuild(SCENES, TARGET_DIR + "/" + fileName, BuildTarget.Android, BuildOptions.None);
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