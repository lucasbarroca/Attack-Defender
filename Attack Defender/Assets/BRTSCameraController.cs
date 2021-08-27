using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRTSCameraController : MonoBehaviour
{
    public float panSpeed = 10f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;

    public bool mouseMovementEnable = false;
    public float scrollSpeed = 20f;
    public float minY = 10f;
    public float maxY = 100f;

    // Start is called before the first frame update
    // void Start()
    // {

    // }

    // Update is called once per frame
    void Update()
    {
        // Get current position
        Vector3 pos = transform.position;

        // Check for input
        if (Input.GetKey("w") || (mouseMovementEnable && Input.mousePosition.y >= Screen.height - panBorderThickness))
        {
            pos.z += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") || (mouseMovementEnable && Input.mousePosition.y <= panBorderThickness))
        {
            pos.z -= panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d") || (mouseMovementEnable && Input.mousePosition.x >= Screen.width - panBorderThickness))
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a") || (mouseMovementEnable && Input.mousePosition.x <= panBorderThickness))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        // Zoom with mouse scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

        // Limit camera movement
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        // Set the new position
        transform.position = pos;

    }
}
