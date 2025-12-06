using UnityEditor;
using UnityEngine;

public class ConvertToURP : EditorWindow
{
    [MenuItem("Tools/Convert All Materials to URP Lit")]
    public static void ConvertMaterials()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material");
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat == null) continue;

            if (mat.shader.name == "Standard")
            {
                mat.shader = Shader.Find("Universal Render Pipeline/Lit");
                count++;
            }
        }

        Debug.Log("변환 완료! 총 " + count + "개의 머티리얼이 URP로 변경됨!");
    }
}
