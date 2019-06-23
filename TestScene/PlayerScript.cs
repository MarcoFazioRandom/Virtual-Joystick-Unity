using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	public JoystickScript joystickMove;
	public JoystickScript joystickLook;

	public float speed = 10f;
	
    void Update()
    {
		if (joystickMove && joystickMove.IsWorking)
			transform.position += (Vector3)joystickMove.Output * speed * Time.deltaTime;
			//transform.Translate(joystickMove.Output * speed * Time.deltaTime);

		if (joystickLook && joystickLook.IsWorking)
		{
			transform.rotation = Quaternion.FromToRotation(Vector3.up, joystickLook.Output);

			//float angle = Vector2.SignedAngle(Vector3.up, joystickLook.Output);
			//transform.localRotation = Quaternion.Euler(0f, 0f, angle);
			
			//transform.localRotation = Quaternion.LookRotation(joystickLook.Output);
		}
	}
}
