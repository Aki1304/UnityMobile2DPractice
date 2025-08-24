using UnityEditor;
using UnityEngine;

public class CustomAtlasAutoSlicer : EditorWindow
{
    [MenuItem("Tools/Custom Atlas Auto Slice (Grid 64x64)")]
    static void SliceSelectedAtlas()
    {
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            // Import 기본 세팅
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = 64;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Point;
            importer.maxTextureSize = 2048;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            // 실제 텍스처 불러오기
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null) continue;

            int cellSizeX = 64;  // Slice > Grid by Cell Size X
            int cellSizeY = 64;  // Slice > Grid by Cell Size Y

            int colCount = tex.width / cellSizeX;
            int rowCount = tex.height / cellSizeY;

            SpriteMetaData[] metas = new SpriteMetaData[colCount * rowCount];
            int index = 0;

            for (int y = 0; y < rowCount; y++)
            {
                for (int x = 0; x < colCount; x++)
                {
                    SpriteMetaData meta = new SpriteMetaData();
                    meta.rect = new Rect(
                        x * cellSizeX,
                        tex.height - (y + 1) * cellSizeY, // 좌표계 보정
                        cellSizeX,
                        cellSizeY
                    );
                    meta.name = $"{obj.name}_{index}";
                    meta.pivot = new Vector2(0.5f, 0.5f);
                    meta.alignment = (int)SpriteAlignment.Center;
                    metas[index++] = meta;
                }
            }

            importer.spritesheet = metas;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Debug.Log($"✅ {obj.name} 자동 Slice 완료 (Grid by Cell Size {cellSizeX}x{cellSizeY}, 총 {metas.Length}개)");
        }
    }
}
