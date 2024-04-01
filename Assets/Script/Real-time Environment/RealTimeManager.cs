using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeManager : MonoBehaviour
{
    public float lightRotationSpeed = 0.5f; // Tốc độ quay của Directional Light
    public Light directionalLight; // Đối tượng Light

    void Start()
    {
        // Lấy tham chiếu đến Directional Light trong Scene (cần thêm một đèn Directional Light vào Scene)
        directionalLight = GameObject.FindObjectOfType<Light>();
    }

    void Update()
    {
        // Quay Directional Light
        if (directionalLight != null)
        {
            // Tính góc quay mới dựa trên thời gian
            float lightRotation = Time.time * lightRotationSpeed;

            // Đặt góc quay cho Directional Light
            directionalLight.transform.rotation = Quaternion.Euler(new Vector3(lightRotation, 0, 0));
        }
    }
}
