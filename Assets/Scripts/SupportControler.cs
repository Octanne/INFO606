using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportControler : MonoBehaviour
{
    // Tableau de pieds
    public GameObject[] piedsBase;
    public GameObject[] piedsMiddle;
    public GameObject[] piedsTop;

    public GameObject mat;

    public int angleBase = 135;
    public int angleMiddle = 25;
    public int angleTop = 90;

    public float speed = 150.0f;

    bool positionMode = false;

    bool baseActioned = false;
    bool secondActioned = false;

    public float speedTravel = 1f;
    public float speedRotate = 50f;

    // Start is called before the first frame update
    void Start()
    {
        // Init angle max des pieds
        for (int i = 0; i < piedsBase.Length; i++)
        {
            // Obtain artiulation body
            ArticulationBody articulationBody = piedsBase[i].GetComponent<ArticulationBody>();
            if (i == 0 || i == 1) {
                // Set target position
                setMaxRotation(articulationBody, angleBase, 0, speed);
            } else {
                // Set target position
                setMaxRotation(articulationBody, 0, -angleBase, speed);
            }

            // Obtain artiulation body
            articulationBody = piedsMiddle[i].GetComponent<ArticulationBody>();
            if (i == 0 || i == 1) {
                // Set target position
                setMaxRotation(articulationBody, angleMiddle, 0, speed);
            } else {
                // Set target position
                setMaxRotation(articulationBody, 0, -angleMiddle, speed);
            }

            // Obtain artiulation body
            articulationBody = piedsTop[i].GetComponent<ArticulationBody>();
            if (i == 0 || i == 1) {
                // Set target position
                setMaxRotation(articulationBody, angleTop, 0, speed);
            } else {
                // Set target position
                setMaxRotation(articulationBody, 0, -angleTop, speed);
            }

            // Lock rotation of  mat
            ArticulationBody articulation = mat.GetComponent<ArticulationBody>();
            var xDrive = articulation.xDrive;
            // Get back the current rotation to 0 and lock it
            xDrive.target = 0;
            xDrive.upperLimit = 0;
            xDrive.lowerLimit = 0;
            articulation.xDrive = xDrive;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            positionMode = !positionMode;

            if (!positionMode)
            {
                Debug.Log("Position mode");
                // Lock rotation of  mat
                ArticulationBody articulation = mat.GetComponent<ArticulationBody>();
                var xDrive = articulation.xDrive;
                // Get back the current rotation to 0 and lock it
                xDrive.upperLimit = 0;
                xDrive.lowerLimit = 0;
                articulation.xDrive = xDrive;
            }
            else
            {
                // Unlock rotation of  mat
                ArticulationBody articulation = mat.GetComponent<ArticulationBody>();
                var xDrive = articulation.xDrive;
                // Unlock rotation
                xDrive.upperLimit = 360;
                xDrive.lowerLimit = -360;
                articulation.xDrive = xDrive;
                Debug.Log("Rotation mode");
            }
        }

        if (Input.GetKey(KeyCode.Z) && !secondActioned && !baseActioned)
        {
            transform.Translate(-Vector3.forward * speedTravel * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S) && !secondActioned && !baseActioned)
        {
            transform.Translate(Vector3.forward * speedTravel * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            if (!secondActioned && !baseActioned) {
                transform.Rotate(Vector3.up, -Time.deltaTime * speedRotate);
            } else {
                // Rotate mat
                ArticulationBody articulation = mat.GetComponent<ArticulationBody>();
                
                float RotationRads = articulation.jointPosition[0];
                float Rotation = Mathf.Rad2Deg * RotationRads;

                float rotationChange = -(speedRotate*5f) * Time.fixedDeltaTime;
                float rotationGoal = Rotation + rotationChange;

                var drive = articulation.xDrive;
                drive.target = rotationGoal;
                articulation.xDrive = drive;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (!secondActioned && !baseActioned) {
                transform.Rotate(Vector3.up, Time.deltaTime * speedRotate);
            } else {
                // Rotate mat
                ArticulationBody articulation = mat.GetComponent<ArticulationBody>();
                
                float RotationRads = articulation.jointPosition[0];
                float Rotation = Mathf.Rad2Deg * RotationRads;

                float rotationChange = (speedRotate*5f) * Time.fixedDeltaTime;
                float rotationGoal = Rotation + rotationChange;

                var drive = articulation.xDrive;
                drive.target = rotationGoal;
                articulation.xDrive = drive;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (positionMode) {
            // Si position ouverte
            if (!baseActioned) {
                piedsBaseDeploy();
                baseActioned = true;
                Debug.Log("Base deployed");
            } else if (!secondActioned && baseIsOpened()) {
                piedsMiddleDeploy();
                piedsTopDeploy();
                secondActioned = true;
                Debug.Log("Second deployed");
            }
        } else {
            // Si position fermee
            if (secondActioned) {
                piedsMiddleRetract();
                piedsTopRetract();
                secondActioned = false;
                Debug.Log("Second retracted");
            } else if (baseActioned && !secondIsOpened()) {
                piedsBaseRetract();
                baseActioned = false;
                Debug.Log("Base retracted");
            }
        }
    }

    void piedsMiddleDeploy() {
        // Open middle legs at angleMiddle
        for (int i = 0; i < piedsMiddle.Length; i++)
        {
            // Obtain artiulation body
            ArticulationBody articulationBody = piedsMiddle[i].GetComponent<ArticulationBody>();
            
            if (i == 0 || i == 1) {
                // Set target position
                setRotation(articulationBody, angleMiddle, speed);
            } else {
                // Set target position
                setRotation(articulationBody, -angleMiddle, speed);
            }
        }
    }

    void piedsMiddleRetract() {
        // Replier les pieds 0 degres
        for (int i = 0; i < piedsMiddle.Length; i++)
        {
            // Obtain artiulation body
            ArticulationBody articulationBody = piedsMiddle[i].GetComponent<ArticulationBody>();

            if (i == 0 || i == 1) {
                // Set target position
                setRotation(articulationBody, 0, speed);
            } else {
                // Set target position
                setRotation(articulationBody, 0, speed);
            }
        }
    }

    void piedsTopDeploy() {
        // Open top legs at angleTop
        for (int i = 0; i < piedsTop.Length; i++)
        {
            // Obtain artiulation body
            ArticulationBody articulationBody = piedsTop[i].GetComponent<ArticulationBody>();
            
            if (i == 0 || i == 1) {
                // Set target position
                setRotation(articulationBody, angleTop, speed);
            } else {
                // Set target position
                setRotation(articulationBody, -angleTop, speed);
            }
        }
    }

    void piedsTopRetract() {
        // Replier les pieds 0 degres
        for (int i = 0; i < piedsTop.Length; i++)
        {
            // Obtain artiulation body
            ArticulationBody articulationBody = piedsTop[i].GetComponent<ArticulationBody>();
            
            if (i == 0 || i == 1) {
                // Set target position
                setRotation(articulationBody, 0, speed);
            } else {
                // Set target position
                setRotation(articulationBody, 0, speed);
            }
        }
    }

    void piedsBaseDeploy() {
        // Open base legs at baseAngle
        for (int i = 0; i < piedsBase.Length; i++)
        {
            // Obtain artiulation body
            ArticulationBody articulationBody = piedsBase[i].GetComponent<ArticulationBody>();
           
            if (i == 0 || i == 1) {
                // Set target position
                setRotation(articulationBody, angleBase, speed);
            } else {
                // Set target position
                setRotation(articulationBody, -angleBase, speed);
            }
        }
    }

    void piedsBaseRetract() {
        // Replier les pieds 0 degres
        for (int i = 0; i < piedsBase.Length; i++)
        {
            // Obtain artiulation body
            ArticulationBody articulationBody = piedsBase[i].GetComponent<ArticulationBody>();
            
            if (i == 0 || i == 1) {
                // Set target position
                setRotation(articulationBody, 0, speed);
            } else {
                // Set target position
                setRotation(articulationBody, 0, speed);
            }
        }
    }

    void setRotation(ArticulationBody articulation, float rotation, float speed)
    {
        var drive = articulation.xDrive;
        drive.target = rotation;
        drive.targetVelocity = speed;

        articulation.xDrive = drive;
    }

    void setMaxRotation(ArticulationBody articulation, float rotationMax, float rotationMin, float speed)
    {
        var drive = articulation.xDrive;
        drive.upperLimit = rotationMax;
        drive.lowerLimit = rotationMin;

        /// Config stiffness & damping
        drive.stiffness = 1000;
        drive.damping = 100;
        drive.targetVelocity = speed;

        articulation.xDrive = drive;
    }

    bool isRotationReached(ArticulationBody articulation, float rotation)
    {
        float RotationRads = articulation.jointPosition[0];
        float currentRotation = Mathf.Rad2Deg * RotationRads;
        float delta = Mathf.Abs(currentRotation - rotation);
        Debug.Log("Delta : " + delta + " Rotation : " + currentRotation + " Target : " + rotation);
        return delta < 5f;
    }

    bool isRotationReached(ArticulationBody articulation, float rotation, float delta)
    {
        float RotationRads = articulation.jointPosition[0];
        float currentRotation = Mathf.Rad2Deg * RotationRads;
        float deltaC = Mathf.Abs(currentRotation - rotation);
        Debug.Log("Delta : " + delta + " Rotation : " + currentRotation + " Target : " + rotation);
        return deltaC < delta;
    }

    bool baseIsOpened()
    {
        for (int i = 0; i < piedsBase.Length; i++)
        {
            // Obtain artiulation body
            ArticulationBody articulationBody = piedsBase[i].GetComponent<ArticulationBody>();
            if (i == 0 || i == 1)
            {
                Debug.Log("Check base number : " + i);
                if (!isRotationReached(articulationBody, angleBase))
                {
                    return false;
                }
            }
            else
            {
                if (!isRotationReached(articulationBody, -angleBase))
                {
                    return false;
                }
            }
        }
        return true;
    }

    bool middleIsOpened()
    {
        for (int i = 0; i < piedsMiddle.Length; i++)
        {
            // Obtain artiulation body
            ArticulationBody articulationBody = piedsMiddle[i].GetComponent<ArticulationBody>();
            if (i == 0 || i == 1)
            {
                if (!isRotationReached(articulationBody, angleMiddle))
                {
                    return false;
                }
            }
            else
            {
                if (!isRotationReached(articulationBody, -angleMiddle))
                {
                    return false;
                }
            }
        }
        return true;
    }

    bool topIsOpened()
    {
        for (int i = 0; i < piedsTop.Length; i++)
        {
            // Obtain artiulation body
            ArticulationBody articulationBody = piedsTop[i].GetComponent<ArticulationBody>();
            if (i == 0 || i == 1)
            {
                if (!isRotationReached(articulationBody, angleTop))
                {
                    return false;
                }
            }
            else
            {
                if (!isRotationReached(articulationBody, -angleTop))
                {
                    return false;
                }
            }
        }
        return true;
    }

    bool secondIsOpened()
    {
        return middleIsOpened() && topIsOpened();
    }

}
