using UnityEngine;
using UnityEditor;
using Unity.AI.Navigation;

public class NavMeshCleaner : Editor
{
    [MenuItem("Tools/Wipe All NavMesh Components")]
    public static void CleanUpNavMesh()
    {
        int surfaceCount = 0;
        int modifierCount = 0;

        // Find and destroy all NavMeshSurface components
        NavMeshSurface[] surfaces = FindObjectsOfType<NavMeshSurface>(true);
        foreach (var surface in surfaces)
        {
            Undo.DestroyObjectImmediate(surface);
            surfaceCount++;
        }

        // Find and destroy all NavMeshModifier components
        NavMeshModifier[] modifiers = FindObjectsOfType<NavMeshModifier>(true);
        foreach (var mod in modifiers)
        {
            Undo.DestroyObjectImmediate(mod);
            modifierCount++;
        }

        // Clear any baked data in the background
        UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();

        Debug.Log($"<color=green><b>SUCCESS:</b></color> Completely wiped {surfaceCount} NavMeshSurfaces and {modifierCount} NavMeshModifiers from the scene!");
    }
}
