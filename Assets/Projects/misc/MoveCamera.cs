using UnityEngine;

/// <summary>
/// Unity script that controls the camera interections
/// </summary>
public class MoveCamera : MonoBehaviour
{
    /// <summary>
    /// speed of camera rotation
    /// </summary>
    public float turnSpeed = 7.0f;

    public float maxSpeedX = 4.0f;

    /// <summary>
    /// speed of camera pan
    /// </summary>
    public float panSpeed = 2.0f;       

    /// <summary>
    /// speed of zoom
    /// </summary>
    public float zoomSpeed = 14.0f;

    /// <summary>
    /// position of center of rotation 
    /// </summary>
    private Vector3 mouseOrigin;   
    
    /// <summary>
    /// flag to identify panning
    /// </summary>
    private bool isPanning;

    /// <summary>
    /// flag to identify rotating
    /// </summary>
    private bool isRotating;

    /// <summary>
    /// flag to identify zooming
    /// </summary>
    private bool isZooming;
    private float zoomDepth = 0.0f;
    private float zoomMinDist = 30.0f;
    private float zoomMaxDist = 100.0f; // > 50 recommended

    /// <summary>
    /// locks the view for dragging operations
    /// </summary>
    public static bool lockView = false;

    void Update()
    {

        // if locked, return 
        if (lockView)
            return;

        // set the center of the view
        Vector3 center = new Vector3(0f, 0f, 0f);
        if (DesignerAssets.UAVDesigner.aiMode)
            center = new Vector3(-2000f, 0f, 0f);

        // check for orth view
        bool orthView = BaseDeliveryInterface.orthogonalView;

        // zoom
        //if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0)){
        //    mouseOrigin = Input.mousePosition;
        //    isZooming = true;
        //} else
        if (Input.GetMouseButtonDown(0)) // rotate
        {
            mouseOrigin = Input.mousePosition;
            if(mouseOrigin.x < (Screen.width - Screen.width / 8.0) 
                && mouseOrigin.x > 240) // restrict to center of screen
                isRotating = true;   
        //} else if (Input.GetMouseButtonDown(1)) // pan
        //{
         //   mouseOrigin = Input.mousePosition;
         //   isPanning = true;
        } else if (Input.mouseScrollDelta.y != 0) // zoom
        {
            mouseOrigin = Input.mousePosition;
            isZooming = true;
        }

        // disable movements on button release
        if (!Input.GetMouseButton(0)) isRotating = false;
        //if (!Input.GetMouseButton(1)) isPanning = false;
        //if (!Input.GetMouseButton(1)) isZooming = false; // && (!Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftShift))) isZooming = false;
        if (Input.mouseScrollDelta.y == 0) isZooming = false;
        // restrict y rotation level

        // rotate camera along X and Y axis
        if (isRotating && !orthView)
        {

            // in 3D space position
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            // create a y position vertical limit
            float radius = Vector3.Distance(Camera.main.transform.position, center);
            float yLimit = Mathf.Sin(45) * radius;

            // boolean to check for camera moving in the vertical direction
            bool moveUp = pos.y < 0.0f;

            // get vertical offset
            float yChange = Camera.main.transform.position.y - center.y;

            // if moving up, do not go above the yLimit
            // if moving down, do not go below the level view
            bool toggleY = (moveUp && yChange < yLimit) || (!moveUp && yChange > -yLimit) ;

            // if toggleY is enabled
            if (toggleY) {
                transform.RotateAround(center, transform.right, -pos.y * turnSpeed);
            }

            // rotate around the vertical axis
            if (Mathf.Abs(pos.x * turnSpeed) < maxSpeedX)
            {
                transform.RotateAround(center, Vector3.up, pos.x * turnSpeed);
                //Debug.Log(pos.x * turnSpeed + " " + maxSpeedX);
            }
            else
            {
                //Debug.Log("HIT MAX");
                transform.RotateAround(center, Vector3.up, pos.x * maxSpeedX);
            }

        }

        // move the camera on it's XY plane
        if (isPanning && !orthView)
        {
            Vector3 pos = -Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
            transform.Translate(move, Space.Self);
        }

        // zoom camera in and out
        if (isZooming && !orthView)
        {
            float deltaScroll = Input.mouseScrollDelta.y;
            if (zoomMinDist < Vector3.Distance(Camera.main.transform.position, new Vector3(0f, 0f, 0f)) && deltaScroll > 0) //min distance, zooming in
            {
                zoomDepth += deltaScroll;
                Vector3 move = Input.mouseScrollDelta.y * zoomSpeed * transform.forward;
                transform.Translate(move, Space.World);
            }
            if (zoomMaxDist > Vector3.Distance(Camera.main.transform.position, new Vector3(0f, 0f, 0f)) && deltaScroll < 0) { //max distance, zooming out
                zoomDepth += deltaScroll;
                Vector3 move = Input.mouseScrollDelta.y * zoomSpeed * transform.forward;
                transform.Translate(move, Space.World);
            }
        }

        // move the camera on it's XY plane
        if (isPanning && orthView)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            Vector3 p = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(p.x - pos.x, p.y, p.z - pos.y);
        }

        // move the camera linearly along Z axis
        if (isZooming && orthView)
        {
            //Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mouseScrollDelta.y * 0.1f);  //Input.mousePosition - mouseOrigin);
            //Debug.Log(Input.mouseScrollDelta.y);
            BaseDeliveryInterface.orthoZoom -= Input.mouseScrollDelta.y * 0.1f;//pos.y;
            Camera.main.orthographicSize = BaseDeliveryInterface.orthoZoom;
        }

    }

    private void FixedUpdate() {}

}