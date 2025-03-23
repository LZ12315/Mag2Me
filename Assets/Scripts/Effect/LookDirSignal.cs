using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookDirSignal : MonoBehaviour
{
    [SerializeField] private PlayerController controller; 
    [SerializeField] private float radius;

    private void Update()
    {
        if(controller == null) return;

        Vector2 lookDir = controller.LookDir;
        Vector2 targetPosition = (Vector2)controller.transform.position + lookDir * radius;
        transform.position = targetPosition;

        Vector3 signalDir = (Vector3)targetPosition - controller.transform.position;
        float angle = Mathf.Atan2(signalDir.y, signalDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        //float angle = Vector2.Angle(Vector2.right, lookDir);
        //transform.RotateAround(controller.transform.position, new Vector3(0, 0, 1), angle);
    }
}
