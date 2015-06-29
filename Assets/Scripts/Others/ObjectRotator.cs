using UnityEngine;
using System.Collections;

public class ObjectRotator : MonoBehaviour 
{
    public bool X;
    public bool Y;
    public bool Z;

    public float anglePerSecond;

    void Update()
    {
		if (GetComponents<Rigidbody> ().Length != 0) {
			if (X) {
				GetComponent<Rigidbody>().AddTorque (anglePerSecond, 0, 0, ForceMode.Impulse);
			}
			if (Y) {
				GetComponent<Rigidbody>().AddTorque (0, anglePerSecond, 0, ForceMode.Impulse);
			}
			if (Z) {
				GetComponent<Rigidbody>().AddTorque (0, 0, anglePerSecond, ForceMode.Impulse);
			}
		} else {
			if (X) {
				transform.Rotate (anglePerSecond * Time.deltaTime, 0, 0);
			}
			if (Y) {
				transform.Rotate (0, anglePerSecond * Time.deltaTime, 0);
			}
			if (Z) {
				transform.Rotate (0, 0, anglePerSecond * Time.deltaTime);
			}
		}
    }
}
