using UnityEngine;
using UnityEditor;

public class ApplyMedievalDoorTextures : EditorWindow
{
    [MenuItem("Tools/Fix Medieval Door Textures")]
    public static void ApplyTextures()
    {
        string texturePath = "Assets/medieval-door-pack/textures";
        
        // Find textures
        Texture2D albedo = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturePath}/SM_DoorEntranceCastle_Albedo.png");
        Texture2D metallic = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturePath}/SM_DoorEntranceCastle_Metallic.png");
        Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturePath}/SM_DoorEntranceCastle_Normal.png");
        Texture2D ao = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturePath}/SM_DoorEntranceCastle_AO.png");

        // Try other names if first ones fail (.tif/.mat? the screenshot shows .png or similar)
        if (albedo == null)
        {
            // The screenshot shows "textures", unity handles extensions. Let's find by search.
            string[] guids = AssetDatabase.FindAssets("SM_DoorEntranceCastle_Albedo");
            if (guids.Length > 0) albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
            
            guids = AssetDatabase.FindAssets("SM_DoorEntranceCastle_Metallic");
            if (guids.Length > 0) metallic = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
            
            guids = AssetDatabase.FindAssets("SM_DoorEntranceCastle_Normal");
            if (guids.Length > 0) normal = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
            
            guids = AssetDatabase.FindAssets("SM_DoorEntranceCastle_AO");
            if (guids.Length > 0) ao = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        if (albedo == null)
        {
            Debug.LogError("Could not find the Door Albedo texture in the project!");
            return;
        }

        // Create material - changing to Standard to fix pink material issue
        Material mat = new Material(Shader.Find("Standard"));

        mat.SetTexture("_MainTex", albedo); // Standard Albedo
        
        if (metallic != null) 
        {
            mat.SetTexture("_MetallicGlossMap", metallic);
            mat.SetFloat("_Metallic", 1f);
        }
        
        if (normal != null)
        {
            mat.SetTexture("_BumpMap", normal);
            mat.EnableKeyword("_NORMALMAP");
            
            // Mark as normal map in importer
            string normalPath = AssetDatabase.GetAssetPath(normal);
            TextureImporter importer = AssetImporter.GetAtPath(normalPath) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.NormalMap)
            {
                importer.textureType = TextureImporterType.NormalMap;
                importer.SaveAndReimport();
            }
        }

        if (ao != null)
        {
            mat.SetTexture("_OcclusionMap", ao);
        }

        // Save material
        if (!AssetDatabase.IsValidFolder("Assets/medieval-door-pack/Materials"))
        {
            AssetDatabase.CreateFolder("Assets/medieval-door-pack", "Materials");
        }
        
        string matPath = "Assets/medieval-door-pack/Materials/M_DoorEntranceCastle.mat";
        AssetDatabase.CreateAsset(mat, matPath);
        Debug.Log("Created Material at: " + matPath);

        // Apply to objects in scene
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        int appliedCount = 0;
        foreach (GameObject obj in objects)
        {
            if (obj.name.Contains("DoorEntrance"))
            {
                MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = mat;
                    appliedCount++;
                    EditorUtility.SetDirty(obj);
                }
            }
        }

        Debug.Log($"Successfully applied the material to {appliedCount} Door objects in the scene!");
        AssetDatabase.SaveAssets();
    }
}