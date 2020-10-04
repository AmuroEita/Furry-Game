using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    [SerializeField] float horizentalFactor = 1f;
    [SerializeField] float verticalFactor = 0.1f;
    [SerializeField] float zoomFactor = 1;

    new Camera camera;

	private void Awake()
	{
        camera = GetComponent<Camera>();
	}

	void Update()
    {
        var scrollInput = Input.mouseScrollDelta.y * zoomFactor;
        var size = camera.orthographicSize += scrollInput;


        var moveInput = new Vector3(
            Input.GetAxis("Horizontal") * horizentalFactor * size,
            0,
            Input.GetAxis("Vertical")) * verticalFactor * size;
        transform.Translate(moveInput, Space.World);
    }
}
