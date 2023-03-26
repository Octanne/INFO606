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

    bool pauseAfterFirstDeployStepEnded = false;

    bool inDeployement = false;
    bool isFullyDeployed = false;
    bool flecheIsRotate = false;
    bool moufleDown = false;

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
            if (!inDeployement) {
                inDeployement = true;
                Debug.Log("Deployement");
            } else {
                inDeployement = false;
                Debug.Log("Undeployement");
            }
        }

        // Control moufle 
        if (Input.GetKey(KeyCode.PageUp)) {
            monterMoufle();
        }

        if (Input.GetKey(KeyCode.PageDown)) {
            descendreMoufle();
        }
    }

    void descendreMoufle() {
        ArticulationBody articulation = moufle.GetComponent<ArticulationBody>();
        float rotationChange = (float)speedDeploy*15 * Time.fixedDeltaTime;
        float rotationGoal = getRotation(articulation) + rotationChange;
        if (rotationGoal > 3) {
            rotationGoal = 3;
        }

        moufleDown = true;
        setRotation(articulation, rotationGoal);
    }

    void monterMoufle() {
        ArticulationBody articulation = moufle.GetComponent<ArticulationBody>();
        float rotationChange = (float)speedDeploy*15 * Time.fixedDeltaTime;
        float rotationGoal = getRotation(articulation) - rotationChange;
        if (rotationGoal < 0) {
            rotationGoal = 0;
        }

        moufleDown = true;
        setRotation(articulation, rotationGoal);
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
            for (int i = 0; i < piedsBase.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsBase[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, angleBase)) {
                        firstDeployStep = true;
                    }
                } else {
                    if (updateRotation(articulationBody, -angleBase)) {
                        firstDeployStep = true;
                    }
                }
            }
        }

        if (firstDeployStep && !secondDeployStep && !pauseAfterFirstDeployStepEnded) {
            timerAfterFirstStep--;
            if (timerAfterFirstStep <= 0) {
                pauseAfterFirstDeployStepEnded = true;
            }
        }

        if (firstDeployStep && !secondDeployStep && pauseAfterFirstDeployStepEnded) {
            for (int i = 0; i < piedsTop.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsTop[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, angleTop)) {
                        secondDeployStep = true;
                    }
                } else {
                    if (updateRotation(articulationBody, -angleTop)) {
                        secondDeployStep = true;
                    }
                }

                if (compareRotation(articulationBody, angleMiddle/3.33f, 2f)) {
                    startDeployLast = true;
                }
            }
        }

        if (startDeployLast && !isFullyDeployed) {
            for (int i = 0; i < piedsMiddle.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsMiddle[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, angleMiddle)) {
                        isFullyDeployed = true;
                        
                    }
                } else {
                    if (updateRotation(articulationBody, -angleMiddle)) {
                        isFullyDeployed = true;
                    }
                }
            }
        }
    }

    void undeploy() {
        if (!flecheIsRotate && isFullyDeployed) {
            for (int i = 0; i < piedsMiddle.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsMiddle[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, 0)) {
                        isFullyDeployed = false;
                    }
                } else {
                    if (updateRotation(articulationBody, 0)) {
                        isFullyDeployed = false;
                    }
                }

                if (compareRotation(articulationBody, angleMiddle/3.33f, 2f)) {
                    startDeployLast = false;
                }
            }
        }

        if (!isFullyDeployed && secondDeployStep && !startDeployLast) {
            for (int i = 0; i < piedsTop.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsTop[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, 0)) {
                        secondDeployStep = false;
                    }
                } else {
                    if (updateRotation(articulationBody, 0)) {
                        secondDeployStep = false;
                    }
                }
            }
        }

        if (firstDeployStep && !secondDeployStep && pauseAfterFirstDeployStepEnded) {
            timerAfterFirstStep++;
            if (timerAfterFirstStep >= resetTimerAfterFirstStep) {
                pauseAfterFirstDeployStepEnded = false;
            }
        }

        if (firstDeployStep && !secondDeployStep && !pauseAfterFirstDeployStepEnded) {
            for (int i = 0; i < piedsBase.Length; i++)
            {
                // Obtain articulation body
                ArticulationBody articulationBody = piedsBase[i].GetComponent<ArticulationBody>();
                if (i == 0 || i == 1) {
                    if (updateRotation(articulationBody, 0)) {
                        firstDeployStep = false;
                    }
                } else {
                    if (updateRotation(articulationBody, 0)) {
                        firstDeployStep = false;
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
    }

    void setMaxRotation(ArticulationBody articulation, float rotationMax, float rotationMin) {
        var drive = articulation.xDrive;
        drive.upperLimit = rotationMax;
        drive.lowerLimit = rotationMin;

        /// Config stiffness & damping
        drive.stiffness = 1000;
        drive.damping = 100;
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

        if ((rotationEtat == 1 && rotationGoal >= targetRotation) || (rotationEtat == -1 && rotationGoal <= targetRotation)) {
            rotationGoal = targetRotation;
            setRotation(articulation, rotationGoal);
            return true;
        } else {
            setRotation(articulation, rotationGoal);
            return false;
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
