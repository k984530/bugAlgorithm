using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRender : MonoBehaviour
{
    public GameObject[] list = new GameObject[2];

    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        List<Vector3> vectorlist = new List<Vector3>();

        for (int i = 0; i < list.Length; i++)
        {

            vectorlist.Add(gameObject.transform.position);
            vectorlist.Add(list[i].transform.position);

        }

        lineRenderer.positionCount = vectorlist.Count;
        lineRenderer.SetPositions(vectorlist.ToArray());
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        Mesh mesh = new Mesh();
        meshCollider.sharedMesh = mesh;
        lineRenderer.BakeMesh(mesh, true);

    }


    // Update is called once per frame
    void Update()
    {

        List<Vector3> vectorlist = new List<Vector3>();

        for (int i = 0; i < list.Length; i++)
        {

            vectorlist.Add(gameObject.transform.position);
            vectorlist.Add(list[i].transform.position);

        }

        lineRenderer.positionCount = vectorlist.Count;
        lineRenderer.SetPositions(vectorlist.ToArray());

    }
}