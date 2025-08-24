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
        GUILayout.Label("������ ���� �̸� �տ� ���ξ� �߰� / ����", EditorStyles.boldLabel);

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
            Debug.LogWarning("���õ� ������ �����ϴ�.");
            return;
        }

        foreach (var obj in selected)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            string oldName = obj.name;

            // �̹� prefix�� �پ������� �ǳʶٱ�
            if (oldName.StartsWith(prefix))
                continue;

            string error = AssetDatabase.RenameAsset(path, prefix + oldName);

            if (!string.IsNullOrEmpty(error))
                Debug.LogError($"�̸� ���� ����: {oldName}, ����: {error}");
            else
                Debug.Log($"�̸� ���� ����: {oldName} -> {prefix + oldName}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void RemovePrefix()
    {
        var selected = Selection.objects;

        if (selected.Length == 0)
        {
            Debug.LogWarning("���õ� ������ �����ϴ�.");
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
                    Debug.LogError($"�̸� ���� ����: {oldName}, ����: {error}");
                else
                    Debug.Log($"�̸� ���� ����: {oldName} -> {newName}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
