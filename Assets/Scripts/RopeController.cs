using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    public Transform moufle;
    public Transform crochet;

    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouflePos = moufle.position + new Vector3(0, 0, 0);

        // Calculer la distance entre les deux objets
        float distance = Vector3.Distance(mouflePos, crochet.position);

        // Définir le nombre de points de la ligne Renderer
        lineRenderer.positionCount = 2;

        // Définir les positions de la ligne Renderer
        lineRenderer.SetPosition(0, mouflePos); //lineRenderer.SetPosition(0, moufle.position); 
        lineRenderer.SetPosition(1, crochet.position);

        // Redimensionner la ligne Renderer en fonction de la distance
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.widthMultiplier = distance / 2;
    }
}
