using UnityEngine;

/// <summary>Small, out-of-phase movement for the corridor's hanging lamps.</summary>
public sealed class SchoolLampSway : MonoBehaviour
{
    [SerializeField] private float amplitudeDegrees = 5f;
    [SerializeField] private float speed = 1.15f;
    [SerializeField] private float phase;
    [SerializeField] private bool showLightCone = true;

    private SpriteRenderer lampSprite;
    private Material coneMaterial;
    private Mesh coneMesh;

    private void Awake()
    {
        lampSprite = GetComponent<SpriteRenderer>();
        if (showLightCone) BuildLightCone();
    }

    private void BuildLightCone()
    {
        GameObject cone = new GameObject("Moving soft light");
        cone.transform.SetParent(transform, false);
        MeshFilter filter = cone.AddComponent<MeshFilter>();
        MeshRenderer renderer = cone.AddComponent<MeshRenderer>();
        renderer.sortingOrder = -30;
        coneMesh = new Mesh { name = "Feathered lamp beam" };
        coneMesh.vertices = new[] {
            new Vector3(-0.7f,-0.9f,0), new Vector3(0.7f,-0.9f,0),
            new Vector3(-4f,-10f,0), new Vector3(-2.6f,-10f,0),
            new Vector3(2.6f,-10f,0), new Vector3(4f,-10f,0) };
        coneMesh.colors = new[] {
            new Color(1,1,1,0.15f), new Color(1,1,1,0.15f),
            new Color(1,1,1,0), new Color(1,1,1,0.045f),
            new Color(1,1,1,0.045f), new Color(1,1,1,0) };
        coneMesh.triangles = new[] { 0,2,3, 0,3,4, 0,4,1, 1,4,5 };
        coneMesh.uv = new Vector2[6];
        coneMesh.RecalculateBounds();
        filter.sharedMesh = coneMesh;
        Shader shader = Shader.Find("Sprites/Default");
        if (shader != null)
        {
            coneMaterial = new Material(shader);
            coneMaterial.mainTexture = Texture2D.whiteTexture;
            renderer.sharedMaterial = coneMaterial;
        }
    }

    private void Update()
    {
        float wave = Mathf.Sin(Time.time * speed + phase);
        transform.localRotation = Quaternion.Euler(0f, 0f, wave * amplitudeDegrees);

        // Slight irregular flicker gives the stationary hallway some life.
        if (lampSprite != null)
        {
            float brightness = 0.91f + 0.09f * Mathf.Sin(Time.time * 7.3f + phase * 3f);
            lampSprite.color = new Color(brightness, brightness, brightness, 1f);
            if (coneMaterial != null) coneMaterial.color = new Color(1, 1, 1, brightness);
        }
    }

    private void OnDestroy()
    {
        if (coneMaterial != null) Destroy(coneMaterial);
        if (coneMesh != null) Destroy(coneMesh);
    }
}
