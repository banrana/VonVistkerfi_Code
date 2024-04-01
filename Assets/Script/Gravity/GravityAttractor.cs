using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    public float gravity = 9.8f; //gia tốc trọng lực
    public float gravitationalRadius = 100f; // Bán kính vùng trọng lực.

    private void OnDrawGizmos()
    {
        // Vẽ vùng trọng lực để dễ quan sát trong Scene view.
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, gravitationalRadius);
    }

    public void Attract(Rigidbody body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 localUp = body.transform.up;

        // Kiểm tra xem đối tượng có nằm trong vùng trọng lực hay không.
        float distanceToCenter = Vector3.Distance(body.position, transform.position);
        if (distanceToCenter <= gravitationalRadius)
        {
            // Tính lực trọng lực.
            float gravityStrength = (1 - (distanceToCenter / gravitationalRadius)) * gravity;
            Vector3 gravityForce = gravityUp * gravityStrength;

            // Áp dụng lực trọng lực lên đối tượng.
            body.AddForce(gravityForce);

            // Căn chỉnh trục "lên" của đối tượng với tâm của hành tinh.
            Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
            body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 8.0f * Time.deltaTime);
        }
    }
}