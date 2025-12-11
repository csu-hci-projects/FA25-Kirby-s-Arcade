using UnityEngine;

[ExecuteAlways]
public class MaterialScalingScript: MonoBehaviour
{
    public float tilesPerUnit = 1f;

    private MeshRenderer rend;

    void Update()
    {
        if (rend == null) rend = GetComponent<MeshRenderer>();
        if (rend == null) return;

        Vector3 s = transform.lossyScale;

        // Use X scale for horizontal tiling, Y scale for vertical.
        Vector2 tiling = new Vector2(
            Mathf.Abs(s.x) * tilesPerUnit,
            Mathf.Abs(s.y) * tilesPerUnit
        );

        rend.sharedMaterial.mainTextureScale = tiling;
    }
}