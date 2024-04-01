using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode
{
    Free,       // Chế độ camera tự do
    TopDown     // Chế độ camera từ trên xuống
}

public class ThirdPersonCamera : MonoBehaviour
{
    public float sensitivity = 1.0f; // Độ nhạy của camera
    [Header("Free Camera")]
    public float distance = 5.0f;    // Khoảng cách của camera trong chế độ tự do
    public float minDistance = 2.0f; // Khoảng cách tối thiểu
    public float maxDistance = 10.0f;// Khoảng cách tối đa
    [Header("Top Down Camera")]
    public float topDownDistance = 10.0f;  // Khoảng cách của camera từ trên xuống
    public float topDownMax = 16.0f;        // Khoảng cách tối đa từ trên xuống
    public float topDownMin = 5.0f;         // Khoảng cách tối thiểu từ trên xuống

    public Transform player;    // Đối tượng người chơi

    private float rotation = 0.0f; // Góc quay của camera

    private float initialDistance = 5.0f; // Khoảng cách ban đầu của camera
    private float targetDistance = 5.0f;  // Khoảng cách mục tiêu của camera
    private bool isTransitioning = false;  // Đang chuyển đổi chế độ
    private float transitionSpeed = 10.0f; // Tốc độ chuyển đổi

    private CameraMode currentMode = CameraMode.Free; // Chế độ camera hiện tại

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; // Khóa con trỏ chuột
        initialDistance = distance; // Lưu khoảng cách ban đầu
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchCameraMode(); // Chuyển đổi chế độ camera khi nhấn Tab
        }

        if (isTransitioning)
        {
            // Lerp để chuyển đổi mượt mà
            distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * transitionSpeed);
            transform.position = Vector3.Lerp(transform.position, player.position - transform.forward * distance, Time.deltaTime * transitionSpeed);

            if (Mathf.Abs(distance - targetDistance) < 0.01f && Vector3.Distance(transform.position, player.position - transform.forward * distance) < 0.01f)
            {
                isTransitioning = false; // Kết thúc quá trình chuyển đổi
            }
        }
        else
        {
            // Logic xử lý chế độ tự do
            if (currentMode == CameraMode.Free)
            {
                rotation -= mouseY * sensitivity;
                rotation = Mathf.Clamp(rotation, -30, 60);

                if (mouseY < 0)
                {
                    targetDistance += Mathf.Abs(mouseY) * sensitivity;
                }
                else
                {
                    targetDistance -= mouseY * sensitivity;
                }

                if (rotation >= 0)
                {
                    targetDistance = initialDistance;
                }

                if (rotation > 30)
                {
                    targetDistance = maxDistance;
                }

                targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
            }
            // Logic xử lý chế độ từ trên xuống
            else if (currentMode == CameraMode.TopDown)
            {
                // Cho phép cuộn để điều chỉnh khoảng cách
                topDownDistance -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 5.0f;
                topDownDistance = Mathf.Clamp(topDownDistance, topDownMin, topDownMax);

                rotation = Mathf.Clamp(rotation, 80, 80);
                targetDistance = topDownDistance;
            }

            // Lerp để di chuyển mượt mà
            distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * 6f);

            transform.localRotation = Quaternion.Euler(rotation, 0, 0);
            transform.position = player.position - transform.forward * distance;
            player.Rotate(Vector3.up * mouseX * sensitivity);
        }
    }

    void SwitchCameraMode()
    {
        if (currentMode == CameraMode.Free)
        {
            currentMode = CameraMode.TopDown;
            targetDistance = topDownDistance; // Đặt khoảng cách cho chế độ từ trên xuống
        }
        else
        {
            currentMode = CameraMode.Free;
            targetDistance = initialDistance; // Đặt khoảng cách cho chế độ người thứ ba
            rotation = 0.0f; // Đặt góc quay về 0
        }
    }
}
