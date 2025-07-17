using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class MetaWallSelector : MonoBehaviour
{
    public Transform rightHandRayOrigin;  // Assign RightHandAnchor
    public float rayLength = 15f;

    public Renderer selectedRenderer;
    public Material selectedMaterial;
    public TextMeshProUGUI SelectedWall_text;
    public Slider tilingXSlider;
    public Slider tilingYSlider;
    public Texture[] textures;

   

    private int currentColorIndex = 0;

    private Color[] wallColors = new Color[]
    {
        new Color(0.9f, 0.9f, 0.9f),    // Soft White
        new Color(0.8f, 0.75f, 0.7f),  // Warm Beige
        new Color(0.7f, 0.8f, 0.85f),  // Light Blue-Grey
        new Color(0.9f, 0.8f, 0.85f),  // Blush Pink
        new Color(0.75f, 0.85f, 0.75f), // Sage Green
        new Color(0.6f, 0.65f, 0.7f),  // Cool Grey
    };
    private int currentTextureIndex = 0;







    void Update()
    {
        // Detect right trigger press
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            Ray ray = new Ray(rightHandRayOrigin.position, rightHandRayOrigin.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLength))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    selectedRenderer = hit.collider.GetComponent<Renderer>();
                    selectedMaterial = selectedRenderer.material;
                    Debug.Log("Wall Selected: " + hit.collider.name);
                    SelectedWall_text.text = "Selected Wall : " + selectedRenderer.gameObject.name;
                }
            }
        }
    }

    public void ChangeTilingX()
    {
        if (selectedMaterial != null)
        {
            Vector2 tiling = selectedMaterial.mainTextureScale;
            tiling.x = tilingXSlider.value;
            selectedMaterial.mainTextureScale = tiling;
        }
    }

    public void ChangeTilingY()
    {
        if (selectedMaterial != null)
        {
            Vector2 tiling = selectedMaterial.mainTextureScale;
            tiling.y = tilingYSlider.value;
            selectedMaterial.mainTextureScale = tiling;
        }
    }

    public void CycleColor()
    {
        if (selectedMaterial == null) return;

        selectedMaterial.color = wallColors[currentColorIndex];

        currentColorIndex = (currentColorIndex + 1) % wallColors.Length; // loop back to 0
    }

    public void CycleTexture()
    {
        if (selectedMaterial == null || textures.Length == 0) return;

        selectedMaterial.mainTexture = textures[currentTextureIndex];

        currentTextureIndex = (currentTextureIndex + 1) % textures.Length; // loop
    }
}