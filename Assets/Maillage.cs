using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using UnityEngine;

public class Maillage : MonoBehaviour
{
    public string fileName;

    public Vector3[] vertices;
    public int[] triangles;
    public int[] sommetsParFace;
    public Vector3[] normals;

    public Material mat;

    private Vector3 centreGrav;

    private float maxCoord;

    private int nbSommets;
    private int nbFaces;
    private int nbArretes;

    private Mesh msh;
    // Start is called before the first frame update
    void Start()
    {
        //string fileName = "buddha.off";
        TextReader reader;
        reader = new StreamReader(fileName);
        string line;
        reader.ReadLine();

        line = reader.ReadLine();
        string[] nombres = line.Split(' ');
        nbSommets = int.Parse(nombres[0]);
        nbFaces = int.Parse(nombres[1]);
        nbArretes = int.Parse(nombres[2]);

        vertices = new Vector3[nbSommets];
        triangles = new int[nbFaces * 3];
        sommetsParFace = new int[nbFaces];

        float sommeX = 0;
        float sommeY = 0;
        float sommeZ = 0;

        float maxCoordX = -1;
        float minCoordX = -1;

        float maxCoordY = -1;
        float minCoordY = -1;

        float maxCoordZ = -1;
        float minCoordZ = -1;

        for (int i = 0; i < nbSommets; i++)
        {
            line = reader.ReadLine();
            string[] coords = line.Split(' ');
            //Debug.Log(coords[0]);
            float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
            float y = float.Parse(coords[1], CultureInfo.InvariantCulture);
            float z = float.Parse(coords[2], CultureInfo.InvariantCulture);

            if (Mathf.Abs(x) > maxCoordX)
                maxCoordX = Mathf.Abs(x);

            if (Mathf.Abs(y) > maxCoordY)
                maxCoordY = Mathf.Abs(y);

            if (Mathf.Abs(z) > maxCoordZ)
                maxCoordZ = Mathf.Abs(z);

            if (Mathf.Abs(x) < minCoordX || minCoordX == -1)
                minCoordX = Mathf.Abs(x);

            if (Mathf.Abs(y) < minCoordY || minCoordY == -1)
                minCoordY = Mathf.Abs(y);

            if (Mathf.Abs(z) < minCoordZ || minCoordZ == -1)
                minCoordZ = Mathf.Abs(z);

            sommeX += x;
            sommeY += y;
            sommeZ += z;

            vertices[i] = new Vector3(x, y, z);
        }
        maxCoordX = maxCoordX - minCoordX;
        maxCoordY = maxCoordY - minCoordY;
        maxCoordZ = maxCoordZ - minCoordZ;

        maxCoord = Mathf.Max(maxCoordX, maxCoordY, maxCoordZ);

        sommeX /= nbSommets;
        sommeY /= nbSommets;
        sommeZ /= nbSommets;
        centreGrav = new Vector3(sommeX, sommeY, sommeZ);


        int k = 0;
        for (int i = 0; i < nbFaces; i++)
        {
            line = reader.ReadLine();
            string[] coords = line.Split(' ');
            int n = int.Parse(coords[0]);

            sommetsParFace[i] = n;

            for (int j = 1; j <= n; j++)
            {
                int sommet = int.Parse(coords[j]);
                triangles[k++] = sommet;
            }
        }

        reader.Close();

        CentrerMaillage();
        //NormaliserTaille();
        TraceMaillage();
        CalculerNormales();

        Ecrire();
    }
    private void Ecrire()
    {
        TextWriter writer;
        writer = new StreamWriter("OUT" + fileName);

        writer.WriteLine("OFF");
        writer.WriteLine(nbSommets + " " + nbFaces + " " + nbArretes);
        for (int i = 0; i < nbSommets; i++)
        {
            writer.WriteLine(vertices[i].x.ToString().Replace(",", ".") + " " + vertices[i].y.ToString().Replace(",", ".") + " " + vertices[i].z.ToString().Replace(",", "."));
        }

        int k = 0;
        for (int i = 0; i < nbFaces; i++)
        {
            string line = sommetsParFace[i].ToString();
            for (int j = 0; j < sommetsParFace[i]; j++)
                line += " " + triangles[k++].ToString();

            writer.WriteLine(line);

        }
        writer.Close();
    }
    private void NormaliserTaille()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].x /= maxCoord;
            vertices[i].y /= maxCoord;
            vertices[i].z /= maxCoord;
        }
    }

    private void CentrerMaillage()
    {
        for (int i = 0; i < vertices.Length; i++)
            vertices[i] -= centreGrav;
    }

    private void CalculerNormales()
    {
        normals = new Vector3[nbSommets];

        for (int i = 0; i < nbSommets; i++)
        {
            normals[i] = Vector3.zero;
        }

        int k = 0;
        for (int i = 0; i < nbFaces; i++)
        {
            Vector3 p1 = vertices[triangles[k + 1]] - vertices[triangles[k]];
            Vector3 p2 = vertices[triangles[k + 2]] - vertices[triangles[k]];

            Vector3 n = Vector3.Cross(p1, p2);

            normals[triangles[k]] += n;
            normals[triangles[k + 1]] += n;
            normals[triangles[k + 2]] += n;

            k += 3;
        }

        for (int i = 0; i < nbSommets; i++)
        {
            normals[i] = Vector3.Normalize(normals[i]);
        }
    }
    private void TraceMaillage()
    {

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        msh = new Mesh();

        msh.vertices = vertices;
        msh.triangles = triangles;
        //msh.normals = normals;

        gameObject.GetComponent<MeshFilter>().mesh = msh;
        gameObject.GetComponent<MeshRenderer>().material = mat;

        gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();

    }
    // Update is called once per frame
    /*void Update()
    {
        //gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        CalculerNormales();
        msh.normals = normals;
    }*/
}

