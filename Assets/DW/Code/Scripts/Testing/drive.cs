using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drive : MonoBehaviour
{
    Rigidbody rb;
    ConfigurableJoint cj;
    public float range = 1f;
    public float power = 20000f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cj = GetComponent<ConfigurableJoint>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool grounded = true;
        Vector3 down = Vector3.down;
        Vector3 position = transform.position;

        RaycastHit hit;
        Color rayCol = Color.red;
        grounded = UnityEngine.Physics.Raycast(position, down, out hit, range);

        if (grounded) {
            Vector3 forward = -Vector3.Cross(transform.forward, hit.normal);

            Debug.DrawRay(position, forward, Color.magenta);

            rayCol = Color.green;

            if (Input.GetKey(KeyCode.W)) {
                rb.AddForce(forward * power);
            }

            if (Input.GetKey(KeyCode.S)) {
                rb.AddForce(forward * -power);
            }
        }

        cj.targetRotation = new Quaternion(0f, -Input.GetAxis("Horizontal"), 0f, 1f);


        Debug.DrawLine(position, position + (down * range), rayCol);
    }
}
