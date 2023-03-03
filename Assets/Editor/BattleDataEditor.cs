using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class BattleDataEditor
{

    public string Name;

    [MenuItem("Assets/Create/Ladybug/Battle Data")]
    public static void CreateData()
    {
        BattleDataAsset asset = ScriptableObject.CreateInstance<BattleDataAsset>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(BattleDataAsset).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}

