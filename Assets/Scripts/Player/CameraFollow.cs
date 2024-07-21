using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //Target that the camera will follow
    public Transform Target;
    //Speed of the camera movement
    public float Speed;
    //Smoothing of the movement
    public float Smoothing;

    //Desired Position
    Vector3 currentPos;
    //The cameras Z position
    float startZ;

    // Start is called before the first frame update
    void Start()
    {
        //Set The camera Z position
        startZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        //Get The current desired position
        currentPos = Vector3.MoveTowards(transform.position, Target.position, Speed * Time.deltaTime);
        //Smooth it
        transform.position = Vector3.Slerp(transform.position, new Vector3(currentPos.x, currentPos.y, startZ), Smoothing * Time.deltaTime);
    }
}
