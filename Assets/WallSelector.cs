using UnityEngine;
using UnityEngine.InputSystem;

public class MetaWallSelector : MonoBehaviour
{
    public Transform rightHandRayOrigin;  // Assign RightHandAnchor
    public float rayLength = 15f;

    public Renderer selectedRenderer;
    public Material selectedMaterial;

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
                }
            }
        }
    }
}