using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    private List<GravityAttractor> planets = new List<GravityAttractor>();
    private Rigidbody rb;
    private bool isGrounded = false; // Biến kiểm tra xem vật thể có đang đứng trên bề mặt không

    void Awake()
    {
        // Tìm tất cả các hành tinh có GravityAttractor và thêm vào danh sách planets.
        GravityAttractor[] planetArray = FindObjectsOfType<GravityAttractor>();
        planets.AddRange(planetArray);
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        GravityAttractor nearestPlanet = GetNearestPlanet();

        if (nearestPlanet != null)
        {
            nearestPlanet.Attract(rb);

            // Tính toán lực hút về hành tinh gần nhất và áp dụng lên vật thể.
            Vector3 gravityUp = (nearestPlanet.transform.position - transform.position).normalized;
            Vector3 bodyUp = transform.up;
            rb.AddForce(gravityUp * nearestPlanet.gravity, ForceMode.Acceleration);
            // Kiểm tra xem vật thể có đang đứng trên bề mặt không.
            if (isGrounded)
            {
                // Nếu đang đứng trên bề mặt, hủy lực nảy và đặt biến isGrounded thành false.
                //rb.velocity = Vector3.zero;
                isGrounded = false;
            }
            else
            {
                // Nếu không đứng trên bề mặt, áp dụng lực trọng lực.
                rb.AddForce(gravityUp * nearestPlanet.gravity);
            }
        }
    }

    GravityAttractor GetNearestPlanet()
    {
        GravityAttractor nearestPlanet = null;
        float nearestDistance = float.MaxValue;
        Vector3 position = transform.position;

        foreach (GravityAttractor planet in planets)
        {
            float distanceToPlanet = Vector3.Distance(position, planet.transform.position);

            if (distanceToPlanet < nearestDistance)
            {
                nearestDistance = distanceToPlanet;
                nearestPlanet = planet;
            }
        }

        return nearestPlanet;
    }

    public Vector3 GetUpDirection()
    {
        // Hàm trả về hướng trọng lực làm cho những thứ có thể di chuyển đứng trên bề mặt hành tinh.
        return (transform.position - GetNearestPlanet().transform.position).normalized;
    }
}
