using UnityEngine;
using UnityEditor;

public class AssetRenamerWindow : EditorWindow
{
    private string prefix = "SE_";

    [MenuItem("Tools/Asset Renamer")]
    public static void ShowWindow()
    {
        GetWindow<AssetRenamerWindow>("Asset Renamer");
    }

    private void OnGUI()
    {
        GUILayout.Label("선택한 에셋 이름 앞에 접두어 추가 / 제거", EditorStyles.boldLabel);

        prefix = EditorGUILayout.TextField("Prefix", prefix);

        if (GUILayout.Button("Rename: Add Prefix"))
        {
            AddPrefix();
        }

        if (GUILayout.Button("Rename: Remove Prefix"))
        {
            RemovePrefix();
        }
    }

    private void AddPrefix()
    {
        var selected = Selection.objects;

        if (selected.Length == 0)
        {
            Debug.LogWarning("선택된 에셋이 없습니다.");
            return;
        }

        foreach (var obj in selected)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            string oldName = obj.name;

            // 이미 prefix가 붙어있으면 건너뛰기
            if (oldName.StartsWith(prefix))
                continue;

            string error = AssetDatabase.RenameAsset(path, prefix + oldName);

            if (!string.IsNullOrEmpty(error))
                Debug.LogError($"이름 변경 실패: {oldName}, 이유: {error}");
            else
                Debug.Log($"이름 변경 성공: {oldName} -> {prefix + oldName}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void RemovePrefix()
    {
        var selected = Selection.objects;

        if (selected.Length == 0)
        {
            Debug.LogWarning("선택된 에셋이 없습니다.");
            return;
        }

        foreach (var obj in selected)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            string oldName = obj.name;

            int idx = oldName.IndexOf("_");
            if (idx > -1)
            {
                string newName = oldName.Substring(idx + 1);

                string error = AssetDatabase.RenameAsset(path, newName);

                if (!string.IsNullOrEmpty(error))
                    Debug.LogError($"이름 변경 실패: {oldName}, 이유: {error}");
                else
                    Debug.Log($"이름 변경 성공: {oldName} -> {newName}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
