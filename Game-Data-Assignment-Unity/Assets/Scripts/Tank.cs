using UnityEngine;

public class Tank : MonoBehaviour {

    public GameObject crosshair;
    public Transform barrel;
    public GameObject trackPrefab;
    public Transform tracksHolder;
    public float distanceBetweenTracks;

    public GameObject shellPrefab;

    public Vector2 mouseScreenCoordinates;
    public Vector2 mouseViewPortCoordinates;
    public Vector3 mouseWorldCoordinates;

    public float speed;
    public float maxFiringDistance;
    float firingDistance;

    public float maxTurnDelta;

    public Vector3 destination;
    Vector3 forward;
    float destinationAngle;
    Vector3 destinationDirection;

    Vector3 mouseDirection;
    float mouseAngle;

    Vector3 previousTrackPosition;



    void Awake() {
        crosshair = GameObject.Find("Crosshair");
        tracksHolder = GameObject.Find("Tracks").transform;
    }

    void Start() {
        // set the initial previous track position as the initial position
        previousTrackPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {

        // mouse coordinates in screen, viewport and world spaces
        mouseScreenCoordinates = Input.mousePosition;
        mouseViewPortCoordinates = Camera.main.ScreenToViewportPoint(mouseScreenCoordinates);
        mouseWorldCoordinates = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenCoordinates.x, mouseScreenCoordinates.y, -Camera.main.transform.position.z));

        // set destination on left mouse click to the mouse world coordinates
        if (Input.GetMouseButtonDown(0)) {
            destination = mouseWorldCoordinates;
        }

        // calculate direction to destination
        destinationDirection = (destination - transform.position).normalized;
        // calculate angle to destination
        destinationAngle = Mathf.Atan2(destinationDirection.y, destinationDirection.x) * Mathf.Rad2Deg;
        Debug.DrawRay(transform.position, destinationDirection, Color.yellow);


        // the forward angle in 2D is simply the value of euler angle Z (assuming the sprite is oriented facing right)
        float forwardAngle = transform.eulerAngles.z;
        // calculate the difference in angle between the destination angle and the current angle
        float turnDelta = Mathf.DeltaAngle(forwardAngle, destinationAngle);

        // rotate the tank toward the destination angle using the maxTurnDelta
        if (Mathf.Abs(turnDelta) > maxTurnDelta * Time.deltaTime) {
            float turnAngle = Mathf.Sign(turnDelta) * maxTurnDelta;
            transform.eulerAngles = new Vector3(0, 0, (forwardAngle + turnAngle * Time.deltaTime));
        } else {
            transform.eulerAngles = new Vector3(0, 0, destinationAngle);
        }

        // set new forward vector based on rotation;
        forward = new Vector3(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad), 0);
        Debug.DrawRay(transform.position, forward, Color.blue);

        // rotate toward new forward
        transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);

        // move forward
        if (Input.GetMouseButton(1))
        {
            transform.Translate(forward * speed * Time.deltaTime * -1, Space.World);
        }
        else
        {
            transform.Translate(forward * speed * Time.deltaTime, Space.World);
        }


        // once the tank has moved, we push the crosshair out from the tank

        // direction to mouse position
        mouseDirection = mouseWorldCoordinates - transform.position;

        // limit crosshair distance according to max firing distance
        float crosshairDistance = (mouseDirection.magnitude < maxFiringDistance) ? mouseDirection.magnitude : maxFiringDistance;

        // direction from tank to crosshair
        crosshair.transform.position = transform.position + Vector3.ClampMagnitude(mouseDirection, crosshairDistance);

        // angle to mouse position
        mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
        // rotate the barrel according the relative angle to the mouse position
        barrel.eulerAngles = new Vector3(0, 0, mouseAngle);

        MakeTracks();
    }

    void MakeTracks() {
        // if the tank has travelled at least the distance between tracks
        if ((transform.position - previousTrackPosition).sqrMagnitude > distanceBetweenTracks * distanceBetweenTracks) {
            // create a new track
            GameObject track = Instantiate(trackPrefab, transform.position, transform.rotation) as GameObject;
            // make the new track object a child of the tracks parent
            track.transform.SetParent(tracksHolder);
            // update the value of the previous track position
            previousTrackPosition = transform.position;
        }
    }
}
