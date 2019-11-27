using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class OdysseyHMDAligner : MonoBehaviour
{
    public GameObject rigidbody;
    public GameObject camera;
    public GameObject cameraParentPos;
    public GameObject cameraParentRot;

    public float smoothing = 0.95f;

    public Vector3 hmdOffset;

    public Camera vrCamera;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //float alphaD = Quaternion.Angle(rigidbody.transform.rotation, camera.transform.rotation);
        //print(alphaD);

        if(Input.GetKeyDown(KeyCode.T))
        {
            //cameraParent.transform.position = (rigidbody.transform.position) - InputTracking.GetLocalPosition(XRNode.CenterEye);
            cameraParentRot.transform.rotation = Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.CenterEye)) * rigidbody.transform.rotation;
        }

        Vector3 target = (rigidbody.transform.position +
            rigidbody.transform.right * hmdOffset.x +
            rigidbody.transform.up * hmdOffset.y +
            rigidbody.transform.forward * hmdOffset.z) 
            -InputTracking.GetLocalPosition(XRNode.CenterEye);

        Quaternion tRot = rigidbody.transform.rotation * Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.CenterEye));


        cameraParentPos.transform.position = smoothing * cameraParentPos.transform.position + (1.0f - smoothing) * target;
        cameraParentRot.transform.rotation = Quaternion.Lerp(cameraParentRot.transform.rotation, tRot, (1.0f - smoothing));


        if (Vector3.Distance(cameraParentPos.transform.position,target) > 0.1f)
        {
            Debug.Log("too much, jump" + Vector3.Distance(cameraParentPos.transform.position, target));
            cameraParentPos.transform.position = target;
        }

        //cameraParentPos.transform.position = 0.95f * cameraParentPos.transform.position + 0.05f*target;
        //cameraParentRot.transform.rotation = Quaternion.Lerp(cameraParentRot.transform.rotation, tRot, 0.01f);

            //transform.position = -InputTracking.GetLocalPosition(XRNode.CenterEye);
            //transform.rotation = Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.CenterEye));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere((rigidbody.transform.position +
            rigidbody.transform.right * hmdOffset.x +
            rigidbody.transform.up * hmdOffset.y +
            rigidbody.transform.forward * hmdOffset.z), 0.05f);
    }

    public void Align()
    {

    }
}
