using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public bool invertAxis = false;
    private Vector3 direction;
    private CharacterController controller;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * (invertAxis ? -1 : 1);
        float z = Input.GetAxis("Vertical") * (invertAxis ? -1 : 1);

        direction = new Vector3(x, 0, z).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime);
        }

        controller.Move(direction * speed * Time.deltaTime);

        // Send input to animator
        animator.SetFloat("MoveX", x);
        animator.SetFloat("MoveZ", z);
    }
}
