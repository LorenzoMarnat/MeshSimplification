using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    //public GameObject go;
    public float potential = 1;

    public float cubeSize = 1;

    public int id;
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> indexes = new List<int>();

}
