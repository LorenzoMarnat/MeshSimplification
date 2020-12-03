using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Material mat;
    public GameObject simplified;
    public CubeManager cubeManager;
    public GameObject cubeGO;
    public float cubeSize = 0.1f;

    private Mesh mesh;
    public Vector3 origine;

    private int cubes;

    public int[] newTriangles;

    public IDictionary<int, int> verticesDictionary = new Dictionary<int, int>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            cubeManager.HideAndShow();

        if(Input.GetKeyDown(KeyCode.G))
        {
            mesh = GetComponent<MeshFilter>().mesh;

            SetBox();
            CreateGrid();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine("Simplified");

        }
    }

    private void CreateGrid()
    {
        int id = 0;
        for (int i = 0; i < cubes; i++)
        {
            for (int j = 0; j < cubes; j++)
            {
                for (int k = 0; k < cubes; k++)
                {
                    Vector3 center = new Vector3(origine.x + i * cubeSize, origine.y + j * cubeSize, origine.z + k * cubeSize);


                    GameObject c = Instantiate(cubeGO, center, Quaternion.identity);
                    c.GetComponent<Cube>().cubeSize = cubeSize;
                    c.GetComponent<Cube>().id = id;
                    c.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

                    Vector3 min = new Vector3(center.x - cubeSize / 2f, center.y - cubeSize / 2f, center.z - cubeSize / 2f);
                    Vector3 max = new Vector3(center.x + cubeSize / 2f, center.y + cubeSize / 2f, center.z + cubeSize / 2f);

                    int index = 0;

                    foreach (Vector3 vertice in mesh.vertices)
                    {
                        if (VerticeInCube(vertice, min, max))
                        {
                            c.GetComponent<Cube>().vertices.Add(vertice);
                            c.GetComponent<Cube>().indexes.Add(index);
                            verticesDictionary.Add(index, id);
                        }
                        index++;
                    }

                    if (c.GetComponent<Cube>().vertices.Count <= 0)
                        Destroy(c);
                    else
                        id++;

                }
            }
        }
    }
    private bool VerticeInCube(Vector3 v, Vector3 min, Vector3 max)
    {
        Vector3 vertice = transform.position + v;

        if (vertice.x >= min.x && vertice.x <= max.x && vertice.y >= min.y && vertice.y <= max.y && vertice.z >= min.z && vertice.z <= max.z)
            return true;
        else
            return false;
    }
    private IEnumerator Simplified()
    {
        GameObject[] activeCubes = GameObject.FindGameObjectsWithTag("Cube");

        Vector3[] newVertices = new Vector3[activeCubes.Length];
        
        newTriangles = new int[mesh.triangles.Length];
        int i = 0;
        foreach (GameObject cube in activeCubes)
        {
            Cube c = cube.GetComponent<Cube>();

            int size = c.vertices.Count;

            Vector3 average = Vector3.zero;

            foreach (Vector3 vertice in c.vertices)
                average += vertice;

            average /= size;
            newVertices[i] = average;

            i++;
            yield return null;
        }
        for (int j = 0;j<mesh.triangles.Length;j++)
        {
            newTriangles[j] = verticesDictionary[mesh.triangles[j]];
        }
        newTriangles = CleanTriangles(newTriangles);

        simplified.AddComponent<MeshFilter>();         
        simplified.AddComponent<MeshRenderer>();
        Mesh msh = new Mesh();                          

        msh.vertices = newVertices;
        msh.triangles = newTriangles;
        msh.RecalculateNormals();

        simplified.GetComponent<MeshFilter>().mesh = msh;
        simplified.GetComponent<MeshRenderer>().material = mat;
    }
    private int[] CleanTriangles(int[] triangles)
    {
        int[] cleanedTriangles = new int[triangles.Length];

        int size = 0;

        for(int i = 0;i<triangles.Length;i+=3)
        {
            if(!(triangles[i] == triangles[i+1] || triangles[i] == triangles[i + 2] || triangles[i+1] == triangles[i + 2]))
            {
                cleanedTriangles[size] = triangles[i];
                cleanedTriangles[size+1] = triangles[i+1];
                cleanedTriangles[size+2] = triangles[i+2];
                size += 3;
            }
        }
        Array.Resize(ref cleanedTriangles, size);
        return cleanedTriangles;
    }
    private void SetBox()
    {
        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float minZ = Mathf.Infinity;

        float maxX = Mathf.NegativeInfinity;
        float maxY = Mathf.NegativeInfinity;
        float maxZ = Mathf.NegativeInfinity;

        foreach (Vector3 vertice in mesh.vertices)
        {
            if (vertice.x + 0.01f > maxX)
                maxX = vertice.x + 0.01f;
            if (vertice.x - 0.01f < minX)
                minX = vertice.x - 0.01f;

            if (vertice.y + 0.01f > maxY)
                maxY = vertice.y + 0.01f;
            if (vertice.y - 0.01f < minY)
                minY = vertice.y - 0.01f;

            if (vertice.z + 0.01f > maxZ)
                maxZ = vertice.z + 0.01f;
            if (vertice.z - 0.01f < minZ)
                minZ = vertice.z - 0.01f;
        }
        origine = new Vector3(transform.position.x + minX, transform.position.y + minY, transform.position.z + minZ);

        float tailleX = maxX - minX + 0.01f;
        float tailleY = maxY - minY + 0.01f;
        float tailleZ = maxZ - minZ + 0.01f;

        float taille = Mathf.Max(tailleX, tailleY, tailleZ);

        cubes = (int)(taille * (1 / cubeSize)) + 1;
    }
}
