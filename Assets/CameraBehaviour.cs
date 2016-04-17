using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public GameObject camerasit;
    public GameObject cameralook;

    //units per pixel
    public float scrollSpeed = 1 / 16;
    public float rotateSpeed = 0.5f;
    public float zoomSpeed = 1;

    public float followTurnSpeed = Mathf.PI / 2;
    private float followAccelleration = 0.2f;

    public bool holdingLeftButton = false;
    public bool holdingRightButton = false;

    private Vector3 lastMousePosition;
    public GameObject followingAgent = null;

    public Vector3 orbitPoint = Vector3.zero;
    public bool orbiting;
    public float orbitRadius;
    public float orbitSpeed;
    public float orbitHeight;
    public float orbitTheta;

    private Vector3 positionBeforeOrbiting;
    private Vector3 rotationBeforeOrbiting;

    public void stopOrbiting()
    {
        orbiting = false;
        transform.position = positionBeforeOrbiting;
        transform.eulerAngles = rotationBeforeOrbiting;
    }

    public void startOrbiting(Vector3 v, float speed, float radius, float height)
    {
        positionBeforeOrbiting = transform.position;
        rotationBeforeOrbiting = transform.eulerAngles;

        orbiting = true;
        orbitPoint = v;
        orbitSpeed = speed;
        orbitRadius = radius;
        orbitHeight = height;
        orbitTheta = 0;
    }

    public void followAgent(GameObject agent)
    {
        /*
        followingAgent = agent;
        Vector3 followPosition = followingAgent.followPosition.transform.position;
        this.transform.position = new Vector3(followPosition.x, SimulationController.Instance.HeightMultiplier + 0.5f, followPosition.z);

        Vector3 lookPosition = followingAgent.overShoulder.transform.position;

        Vector3 relativePositon = new Vector3(lookPosition.x, SimulationController.Instance.HeightMultiplier / 2, lookPosition.z) - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePositon);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Mathf.PI * 2);
        */
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (GameController.Instance.MainGameMode == GameController.MainGameModes.GAME)
        {
            float ymin = TerrainHost.Instance.GetHeightForGameObject(this.transform.position.x, this.transform.position.z);
            if(camerasit.transform.position.y >= ymin)
                this.transform.position = camerasit.transform.position;

            else
            {
                this.transform.position = new Vector3(camerasit.transform.position.x,ymin, camerasit.transform.position.z);
            }

            this.transform.LookAt(cameralook.transform);
        }
        else
        {

            holdingLeftButton = Input.GetMouseButton(0);
            holdingRightButton = Input.GetMouseButton(1);

            if (followingAgent != null)
            {
                /*
                Vector3 followPosition = followingAgent.followPosition.transform.position;
                followPosition = new Vector3(followPosition.x, SimulationController.Instance.HeightMultiplier + 0.5f, followPosition.z);
                Vector3 lookPosition = followingAgent.overShoulder.transform.position;

                float step = Mathf.Max(followingAgent.Velocity.magnitude * 0.9f, 0.2f * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, followPosition, step);

                Vector3 relativePositon = new Vector3(lookPosition.x, SimulationController.Instance.HeightMultiplier / 2, lookPosition.z) - transform.position;
                Quaternion rotation = Quaternion.LookRotation(relativePositon);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, followTurnSpeed * Time.deltaTime); */
            }
            else if (holdingLeftButton || holdingRightButton || Input.mouseScrollDelta.magnitude != 0)
            {
                Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
                float dy = mouseDelta.y;
                float dx = mouseDelta.x;

                if (holdingLeftButton)
                {
                    this.transform.Translate(-this.transform.up * dy * scrollSpeed, Space.World);
                    this.transform.Translate(-this.transform.right * dx * scrollSpeed, Space.World);
                }
                if (holdingRightButton)
                {
                    this.transform.Rotate(Vector3.up, dx * rotateSpeed, Space.World);
                    this.transform.Rotate(-this.transform.right, dy * rotateSpeed, Space.World);
                }
                if (Input.mouseScrollDelta.magnitude != 0)
                {
                    this.transform.Translate(-this.transform.forward * Input.mouseScrollDelta.y * zoomSpeed, Space.World);
                }
            }

            lastMousePosition = Input.mousePosition;
        }
    }
}
