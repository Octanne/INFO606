using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrochetController : MonoBehaviour
{

    FixedJoint jointS;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If space is pressed, destroy the fixed joint
        if (Input.GetKey(KeyCode.E))
        {
            // Suppression du fixed joint
            Destroy(jointS);
            Debug.Log("Destroy");
        }
    }

    // When collide with something
    void OnCollisionEnter(Collision Collision)
    {
        // Si l'objet est un articulation body, on crée un fixed joint que si il n'y en a pas déjà un
        if (Collision.gameObject.GetComponent<Rigidbody>() != null && Collision.gameObject.GetComponent<FixedJoint>() == null)
        {   
            var objCollide = Collision.gameObject;
            // Création du fixed joint
            Debug.Log("Création du fixed joint");
            FixedJoint joint = objCollide.AddComponent<FixedJoint>();
            jointS = joint;
            joint.connectedArticulationBody = this.gameObject.GetComponent<ArticulationBody>();
        }
    }
}
