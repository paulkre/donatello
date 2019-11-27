using UnityEngine;
using UnityEngine.XR;

public class OptitrackOdysseyHmd : MonoBehaviour
{
    public Transform hmdRigidbody;
    public Transform positionOffset;

    void Update()
    {
        var hmdPosition = InputTracking.GetLocalPosition(XRNode.CenterEye);

        positionOffset.position = hmdRigidbody.position - hmdPosition;
    }
}
