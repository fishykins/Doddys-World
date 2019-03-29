using UnityEditor;
using UnityEngine;

public class Toolbar
{
    [MenuItem("Doddy's World/Goon")]
    private static void Goon()
    {
        Debug.Log("Goon");
    }

    [MenuItem("Build Type/Client + Server")]
    private static void Hybrid()
    {
        SetDefines("CLIENT;SERVER");
    }

    [MenuItem("Build Type/Client + Server", true)]
    private static bool HybridValidation()
    {
        // This returns true when the selected object is a Texture2D (the menu item will be disabled otherwise).
#if (CLIENT && SERVER)
        return false;
#else
        return true;
#endif
    }

    [MenuItem("Build Type/Client")]
    private static void Client()
    {
        SetDefines("CLIENT");
    }

    [MenuItem("Build Type/Client", true)]
    private static bool ClientValidation()
    {
        // This returns true when the selected object is a Texture2D (the menu item will be disabled otherwise).
#if (CLIENT && !SERVER)
        return false;
#else
        return true;
#endif
    }

    [MenuItem("Build Type/Server")]
    private static void Server()
    {
        SetDefines("SERVER");
    }

    [MenuItem("Build Type/Server", true)]
    private static bool ServerValidation()
    {
        // This returns true when the selected object is a Texture2D (the menu item will be disabled otherwise).
#if (!CLIENT && SERVER)
        return false;
#else
        return true;
#endif
    }

    [MenuItem("Build Type/Local")]
    private static void Local()
    {
        SetDefines("LOCAL");
    }

    [MenuItem("Build Type/Local", true)]
    private static bool LocalValidation()
    {
        // This returns true when the selected object is a Texture2D (the menu item will be disabled otherwise).
#if (LOCAL)
        return false;
#else
        return true;
#endif
    }

    private static void SetDefines(string defines)
    {
        BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
        //string oldDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
    }


}
