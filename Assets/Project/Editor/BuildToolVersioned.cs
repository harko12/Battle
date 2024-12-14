using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;

public class BuildToolVersioned : MonoBehaviour
{
    [MenuItem("Build/Build with Version")]
    public static void MyBuild()
    {
        // This gets the Build Version from Git via the `git describe` command
        PlayerSettings.bundleVersion = GetVersionString();
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Applications", "Builds", "");
        // ===== This sample is taken from the Unity scripting API here:
        // https://docs.unity3d.com/ScriptReference/BuildPipeline.BuildPlayer.html
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] 
        {
			"Assets/Harko Games/Tnet/connect.unity",
			"Assets/Project/Scenes/Game.unity",
			"Assets/Project/Scenes/GameUI.unity",
			"Assets/Project/Scenes/GameMenu.unity",
        };
        buildPlayerOptions.locationPathName = path + "\\" + PlayerSettings.bundleVersion + "\\Battle.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    public static string GetVersionString()
    {
        return Git.BuildVersion;
    }
}
