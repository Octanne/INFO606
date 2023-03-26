using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrueController : MonoBehaviour
{
    // Tableau de pieds
    public GameObject[] piedsBase = new GameObject[4];
    public GameObject[] piedsMiddle = new GameObject[4];
    public GameObject[] piedsTop = new GameObject[4];

    public GameObject mat;
    public GameObject moufle;
    public GameObject crochet;

    public int angleBase = 135;
    public int angleMiddle = 45;
    public int angleTop = 90;
    public int timerAfterFirstStep = 30;
    int resetTimerAfterFirstStep;

    public float speedRotate = 30f;
    public float speedRotateMat = 30f;
    public float speedTravel = 1f;

    public float speedDeploy = 50f;

    bool firstDeployStep = false;
    bool secondDeployStep = false;
    bool startDeployLast = false;
    bool isFullyDeployed = false;

    bool deployementInProgress = false;
    bool pauseAfterFirstDeployStepEnded = false;

    bool inDeployement = false;
    bool flecheIsRotate = false;
    bool crochetDown = false;
    float crochetPosition = 0f;

    // Start is called before the first frame update
    void Start()
    {
        initDeployParts();
        resetTimerAfterFirstStep = timerAfterFirstStep;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey(KeyCode.Z) && !inDeployement && !firstDeployStep)
        {
            transform.Translate(-Vector3.forward * speedTravel * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S) && !inDeployement && !firstDeployStep)
        {
            transform.Translate(Vector3.forward * speedTravel * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            if (!inDeployement && !firstDeployStep) {
                transform.Rotate(Vector3.up, -Time.deltaTime * speedRotate);
            } else if (isFullyDeployed) {
                matLeftRotate();
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (!inDeployement && !firstDeployStep) {
                transform.Rotate(Vector3.up, Time.deltaTime * speedRotate);
            } else if (isFullyDeployed) {
                matRightRotate();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!inDeployement && !deployementInProgress) {
                inDeployement = true;
                Debug.Log("Deployement");
            } else if (inDeployement && !deployementInProgress){
                inDeployement = false;
                Debug.Log("Undeployement");
            }
        }

        // Control corchet 
        if (Input.GetKey(KeyCode.PageUp)) {
            monterCrochet();
        }

        if (Input.GetKey(KeyCode.PageDown)) {
            descendreCrochet();
        }
    }

    void descendreCrochet() {
        ArticulationBody articulation = crochet.GetComponent<ArticulationBody>();
        
        float newPos = crochetPosition + 0.005f;

        if (newPos > 4f) {
            newPos = 4f;
        }

        crochetPosition = newPos;

        var xDrive = articulation.xDrive;
        xDrive.target = newPos;
        xDrive.upperLimit = newPos;
        xDrive.lowerLimit = newPos;
        articulation.xDrive = xDrive;

        // add Force to the articulation
        //articulation.AddRelativeForce(transform.forward * -speedDeploy);
    }

    void monterCrochet() {
        ArticulationBody articulation = crochet.GetComponent<ArticulationBody>();
        
        float newPos = crochetPosition - 0.005f;

        if (newPos < 0f) {
            newPos = 0f;
        }

        crochetPosition = newPos;

        var xDrive = articulation.xDrive;
        xDrive.target = newPos;
        xDrive.upperLimit = newPos;
        xDrive.lowerLimit = newPos;
        articulation.xDrive = xDrive;

        // add Force to the articulation
        //articulation.AddRelativeForce(transform.forward * speedDeploy);
    }

    void FixedUpdate() {
        if (inDeployement) {
            deploy();
        } else {
            undeploy();
        }
    }

    void deploy() {
        // Base
        if (!firstDeployStep) {
            deployementInProgress = true;
            for (int i = 0; i < piedsBase.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsBase[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, angleBase)) {
                        firstDeployStep = true;
                        Debug.Log("First step");
                    }
                } else {
                    if (updateRotation(articulationBody, -angleBase)) {
                        firstDeployStep = true;
                        Debug.Log("First step");
                    }
                }
            }
        }

        if (firstDeployStep && !secondDeployStep && !isFullyDeployed && !pauseAfterFirstDeployStepEnded) {
            timerAfterFirstStep--;
            if (timerAfterFirstStep <= 0) {
                pauseAfterFirstDeployStepEnded = true;
                Debug.Log("Unpaused");
            }
        }

        if (firstDeployStep && pauseAfterFirstDeployStepEnded && !secondDeployStep && !isFullyDeployed) {
            for (int i = 0; i < piedsTop.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsTop[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, angleTop)) {
                        secondDeployStep = true;
                        Debug.Log("Second deployed");
                    }
                } else {
                    if (updateRotation(articulationBody, -angleTop)) {
                        secondDeployStep = true;
                        Debug.Log("Second deployed");
                    }
                }

                if (compareRotation(articulationBody, angleTop/2f, 2f)) {
                    startDeployLast = true;
                    Debug.Log("Start deploy last");
                }
            }
        }

        if (startDeployLast && firstDeployStep && !isFullyDeployed) {
            for (int i = 0; i < piedsMiddle.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsMiddle[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, angleMiddle)) {
                        isFullyDeployed = true;
                        deployementInProgress = false;
                        Debug.Log("Fully deployed");
                    }
                } else {
                    if (updateRotation(articulationBody, -angleMiddle)) {
                        isFullyDeployed = true;
                        deployementInProgress = false;
                        Debug.Log("Fully deployed");
                    }
                }
            }
        }
    }

    void undeploy() {
        if (flecheIsRotate) {
            // Rotate the mat body to 0
            ArticulationBody articulationBody = mat.GetComponent<ArticulationBody>();
            if (updateRotation(articulationBody, 0)) {
                flecheIsRotate = false;
                Debug.Log("Mat rotate to 0");
            }
        }

        if (!flecheIsRotate && firstDeployStep && isFullyDeployed) {
            deployementInProgress = true;
            for (int i = 0; i < piedsTop.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsTop[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, 0)) {
                        isFullyDeployed = false;
                        Debug.Log("Second step");
                    }
                } else {
                    if (updateRotation(articulationBody, 0)) {
                        isFullyDeployed = false;
                        Debug.Log("Second step");
                    }
                }

                if (rotationAsReached(articulationBody, angleMiddle/2f, 0.5f)) {
                    startDeployLast = false;
                    Debug.Log("Start undeploy second step");
                }
            }
        }

        if (firstDeployStep && secondDeployStep && !startDeployLast) {
            for (int i = 0; i < piedsMiddle.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsMiddle[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, 0)) {
                        secondDeployStep = false;
                        Debug.Log("Not fully deployed");
                    }
                } else {
                    if (updateRotation(articulationBody, 0)) {
                        secondDeployStep = false;
                        Debug.Log("Not fully deployed");
                    }
                }

                
            }
        }

        if (firstDeployStep && !secondDeployStep && !isFullyDeployed && pauseAfterFirstDeployStepEnded) {
            timerAfterFirstStep++;
            if (timerAfterFirstStep >= resetTimerAfterFirstStep) {
                pauseAfterFirstDeployStepEnded = false;
                Debug.Log("Reset timer");
            }
        }

        if (firstDeployStep && !secondDeployStep && !isFullyDeployed && !pauseAfterFirstDeployStepEnded) {
            for (int i = 0; i < piedsBase.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsBase[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, 0)) {
                        firstDeployStep = false;
                        deployementInProgress = false;
                        Debug.Log("First step");
                    }
                } else {
                    if (updateRotation(articulationBody, 0)) {
                        firstDeployStep = false;
                        deployementInProgress = false;
                        Debug.Log("First step");
                    }
                }
            }
        }
    }

    void matRightRotate() {
        // Rotate mat left
        ArticulationBody articulation = mat.GetComponent<ArticulationBody>();
        float rotationChange = (float)speedRotateMat*15 * Time.fixedDeltaTime;
        float rotationGoal = getRotation(articulation) + rotationChange;
        if (rotationGoal > 180) {
            rotationGoal = 180;
        }

        flecheIsRotate = true;
        setRotation(articulation, rotationGoal);
    }

    void matLeftRotate() {
        // Rotate mat right
        ArticulationBody articulation = mat.GetComponent<ArticulationBody>();
        float rotationChange = (float)-speedRotateMat*15 * Time.fixedDeltaTime;
        float rotationGoal = getRotation(articulation) + rotationChange;
        if (rotationGoal < -180) {
            rotationGoal = -180;
        }

        flecheIsRotate = true;
        setRotation(articulation, rotationGoal);
    }

    void initDeployParts() {
        // Init angle max des pieds
        for (int i = 0; i < piedsBase.Length; i++)
        {
            // Obtain artiulation body
            ArticulationBody articulationBody = piedsBase[i].GetComponent<ArticulationBody>();
            if (i == 0 || i == 1) {
                // Set target position
                setMaxRotation(articulationBody, angleBase, 0);
            } else {
                // Set target position
                setMaxRotation(articulationBody, 0, -angleBase);
            }

            // Obtain artiulation body
            articulationBody = piedsMiddle[i].GetComponent<ArticulationBody>();
            if (i == 0 || i == 1) {
                // Set target position
                setMaxRotation(articulationBody, angleMiddle, 0);
            } else {
                // Set target position
                setMaxRotation(articulationBody, 0, -angleMiddle);
            }

            // Obtain artiulation body
            articulationBody = piedsTop[i].GetComponent<ArticulationBody>();
            if (i == 0 || i == 1) {
                // Set target position
                setMaxRotation(articulationBody, angleTop, 0);
            } else {
                // Set target position
                setMaxRotation(articulationBody, 0, -angleTop);
            }
        }

        // Lock rotation of  mat
        ArticulationBody articulation = mat.GetComponent<ArticulationBody>();
        setMaxRotation(articulation, 180, -180);

        // Lock crochet
        articulation = crochet.GetComponent<ArticulationBody>();
        setMaxRotation(articulation, 0, 0);
    }

    void setMaxRotation(ArticulationBody articulation, float rotationMax, float rotationMin) {
        var drive = articulation.xDrive;
        drive.upperLimit = rotationMax;
        drive.lowerLimit = rotationMin;

        /// Config stiffness & damping
        drive.stiffness = 10000;
        drive.damping = 1000;
        drive.targetVelocity = 0;
        drive.target = 0;

        articulation.xDrive = drive;
    }

    float getRotation(ArticulationBody articulation) {
        float RotationRads = articulation.jointPosition[0];
        float Rotation = Mathf.Rad2Deg * RotationRads;
        return Rotation;
    }

    void setRotation(ArticulationBody articulation, float rotation) {
        var drive = articulation.xDrive;
        drive.target = rotation;
        articulation.xDrive = drive;
    }

    bool updateRotation(ArticulationBody articulation, float targetRotation) {
        int rotationEtat = 0;
        if (targetRotation > 0) {
            rotationEtat = 1;
        } else if (targetRotation < 0) {
            rotationEtat = -1;
        } else if (targetRotation == 0 && getRotation(articulation) > 0) {
            rotationEtat = -1;
        } else if (targetRotation == 0 && getRotation(articulation) < 0) {
            rotationEtat = 1;
        }

        float rotationChange = (float)rotationEtat * speedDeploy*15 * Time.fixedDeltaTime;
        float rotationGoal = getRotation(articulation) + rotationChange;

        if (rotationAsReached(articulation, targetRotation, 0.5f)) {
            rotationGoal = targetRotation;
            setRotation(articulation, rotationGoal);
            return true;
        } else {
            setRotation(articulation, rotationGoal);
            return false;
        }
    }

    bool rotationAsReached(ArticulationBody articulation, float targetRotation, float delta) {
        // With a tolerance of delta degrees
        float rotation = getRotation(articulation);
        
        float diff = Mathf.Abs(rotation - targetRotation);
        if (diff <= delta) {
            return true;
        } else {
            // Retourner vrai si targetRotation est > 0 et que rotation > targetRotation
            if (targetRotation > 0 && rotation > targetRotation) {
                return true;
            } else if (targetRotation < 0 && rotation < targetRotation) {
                return true;
            } else {
                return false;
            }
        }
    }

    bool compareRotation(ArticulationBody articulation, float targetRotation, float delta) {
        // With a tolerance of delta degrees
        float rotation = getRotation(articulation);
        if (rotation >= targetRotation - delta && rotation <= targetRotation + delta) {
            return true;
        } else {
            return false;
        }
    }

}
