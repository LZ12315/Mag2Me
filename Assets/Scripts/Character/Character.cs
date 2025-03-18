using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float Mass;

    Vector2 stress;
    Vector2 acceleration;
    Vector2 velocity;

    private void FixedUpdate()
    {
        acceleration = stress / Mass;
        velocity += acceleration * Time.fixedDeltaTime;
        transform.Translate(velocity * Time.fixedDeltaTime);

        stress = new Vector2(0, 0);
        acceleration = new Vector2(0, 0);
    }

    public void AddForce(Vector2 forceValue)
    {
        stress += forceValue;
    }
}
