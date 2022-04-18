using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public GameObject theCube;
    public KeyCode up, right, down, left;

    public float rotationSpeed = 2f;

    public bool rotating = false;

    public Quaternion currentRotation;
    public Quaternion destinationRotation;

    Coroutine rotCoroutine;

    public enum Axis
    {
        up, down, left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        currentRotation = theCube.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //theCube.transform.rotation = Quaternion.AngleAxis(3, Vector3.right) * theCube.transform.rotation;

        if (Input.GetKeyDown(up) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            setCubeRotation(Vector3.right);
        }
        if (Input.GetKeyDown(down) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            setCubeRotation(Vector3.left);
        }
        if (Input.GetKeyDown(right) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            setCubeRotation(Vector3.down);
        }
        if (Input.GetKeyDown(left) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            setCubeRotation(Vector3.up);
        }
    }

    void setCubeRotation(Vector3 axis)
    {
        // only set new rotation if no longer rotating
        if (rotating) return;

        destinationRotation = Quaternion.AngleAxis(90, axis) * theCube.transform.rotation;
        Vector3 eulers = destinationRotation.eulerAngles;
        Debug.LogWarning("Current: Destination " + theCube.transform.rotation.eulerAngles + ":" + eulers);
        // play it safe with coroutines: set a ref, nullify it later
        rotCoroutine = StartCoroutine(rotateCube(axis));
    }

    IEnumerator rotateCube(Vector3 axis)
    {
        // typical lerp pattern, but we're using slerp
        rotating = true;
        currentRotation = theCube.transform.rotation;
        float rotTime = 0;
        while (theCube.transform.rotation != destinationRotation)
        { 
            theCube.transform.localRotation = Quaternion.Slerp(currentRotation, destinationRotation, rotTime * rotationSpeed);

            rotTime += Time.deltaTime;
            yield return false;
        }
        rotCoroutine = null;
        rotating = false;
        Debug.LogWarning("Finished Rotating!");
        yield return true;
    }
}
