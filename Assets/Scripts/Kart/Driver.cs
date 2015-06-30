using UnityEngine;

public class Driver : MonoBehaviour 
{
    internal Transform driverSeat;
    internal GameObject driverInstance;

    void Update()
    {
        if (driverInstance != null && 
            driverSeat != null)
        {
            if (driverInstance.transform.parent != driverSeat)
            {
                driverInstance.transform.SetParent(driverSeat);
                driverInstance.transform.localPosition = Vector3.zero;
                driverInstance.transform.localRotation = Quaternion.identity;
            }
        }
    }
}
