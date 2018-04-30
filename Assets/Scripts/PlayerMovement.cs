using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb2d;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * 50.0f;
        float y = Input.GetAxis("Vertical") * Time.deltaTime * 50.0f;

        Vector2 movement = new Vector2(x, y);

        rb2d.AddForce(movement);

    }
}
