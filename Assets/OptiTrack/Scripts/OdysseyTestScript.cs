using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OdysseyTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<UnityEngine.XR.InputDevice> inputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.RightHanded, inputDevices);

        foreach (var device in inputDevices)
        {
            Debug.Log(string.Format("Device found with name '{0}' and role '{1}'", device.name, device.role.ToString()));
        }

        UnityEngine.XR.InputDevice d = inputDevices[0];

        var inputFeatures = new List<UnityEngine.XR.InputFeatureUsage>();
        if (d.TryGetFeatureUsages(inputFeatures))
        {
            foreach (var feature in inputFeatures)
            {
                if (feature.type == typeof(bool))
                {
                    bool featureValue;
                    if (d.TryGetFeatureValue(feature.As<bool>(), out featureValue))
                    {
                        Debug.Log(string.Format("Bool feature '{0}''s value is '{1}'", feature.name, featureValue.ToString()));
                    }
                }
            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
