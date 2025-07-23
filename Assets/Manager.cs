using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor.Rendering;
public class Manager : MonoBehaviour
{
    public Transform cameraRig;     // This is usually OVRCameraRig or XR Origin
    public Transform head;          // Main camera inside the rig
    public float playermoveSpeed = 2f;


    public Transform rightHandRayOrigin;  // Assign RightHandAnchor
    public float rayLength = 15f;
    public float moveSpeed = 20f;
    public Renderer selectedRenderer;
    public Material selectedMaterial;
    public TextMeshProUGUI SelectedWall_text;
    public Slider tilingXSlider;
    public Slider tilingYSlider;
    public Texture[] textures;
    public GameObject selectedFurniture;
    public GameObject Furniture;

    public bool isHolding = false;
    private int currentColorIndex = 0;
    public LineRenderer lineRenderer;
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


    private void Start()
    {

        foreach (Transform item in Furniture.transform)
        {
            int Index = 0;
            foreach (Transform mesh in item.transform)
            {
                Index++;
                if (Index == 1)
                {
                    mesh.gameObject.SetActive(true);
                }
                else
                {
                    mesh.gameObject.SetActive(false);
                }

            }

        }

    }




    void Update()
    {

        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

        // If there's joystick input
        if (input.sqrMagnitude > 0.01f)
        {
            // Get camera's forward and right vectors
            Vector3 forward = head.forward;
            Vector3 right = head.right;

            // Flatten the vectors to XZ plane
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            // Calculate movement direction
            Vector3 moveDirection = forward * input.y + right * input.x;

            // Apply movement
            cameraRig.position += moveDirection * moveSpeed * Time.deltaTime;
        }




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

                if (hit.collider.CompareTag("Furniture"))
                {
                    selectedFurniture = hit.collider.gameObject;
                    Debug.Log("Furniture selected: " + selectedFurniture.name);
                }



            }


            if (selectedFurniture != null)
            {
                Vector2 Input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

                // Only move if joystick input is not zero
                 if (Input.sqrMagnitude > 0.01f)
                {
                    // Move relative to the world XZ plane
                    Vector3 move = new Vector3(Input.x, 0f, Input.y) * moveSpeed * Time.deltaTime;
                    selectedFurniture.transform.position += move;
                }
            }
        }




        //if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        //{
        //    if (selectedFurniture != null)
        //    {
        //        selectedFurniture.transform.Rotate(0f, 90f, 0f); // Rotate Y by 90 degrees
        //        Debug.Log("Rotated furniture: " + selectedFurniture.name);
        //    }
        //}
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            if (selectedFurniture != null)
            {
                isHolding = !isHolding;
            }
        }

        // If holding, follow controller
        if (isHolding && selectedFurniture != null)
        {
            Ray ray = new Ray(rightHandRayOrigin.position, rightHandRayOrigin.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLength))
            {
                selectedFurniture.transform.position = new Vector3(hit.point.x, selectedFurniture.transform.position.y, hit.point.z);
            }
        }

        // Rotate while holding with B button (optional)
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch) && isHolding)
        {
            selectedFurniture.transform.Rotate(0f, 90f, 0f);
        }


        Vector3 start = rightHandRayOrigin.position;
        Vector3 end = start + rightHandRayOrigin.forward * rayLength;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

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


    private int currentMeshIndex = 0;

    public void SwitchToNextMesh()
    {
        if (selectedFurniture == null) return;

        int childCount = selectedFurniture.transform.childCount;

        if (childCount == 0) return;

        // Deactivate all child meshes first
        for (int i = 0; i < childCount; i++)
        {
            selectedFurniture.transform.GetChild(i).gameObject.SetActive(false);
        }

        // Increment index and loop back if needed
        currentMeshIndex = (currentMeshIndex + 1) % childCount;

        // Activate the current mesh
        selectedFurniture.transform.GetChild(currentMeshIndex).gameObject.SetActive(true);
    }

}