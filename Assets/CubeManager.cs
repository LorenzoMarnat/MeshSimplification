using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{

    public void HideAndShow()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubes)
            cube.GetComponent<Renderer>().enabled = !cube.GetComponent<Renderer>().enabled;
    }
}
