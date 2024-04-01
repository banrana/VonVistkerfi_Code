using SickscoreGames.HUDNavigationSystem;
using UnityEngine;

public class HUDDistance : MonoBehaviour
{
    public float radiusThreshold = 100f; // Đặt ngưỡng bán kính tại đây

    private HUDNavigationElement hudNavigationElement;

    private void Start()
    {
        hudNavigationElement = GetComponent<HUDNavigationElement>();

        // Kiểm tra xem HUDNavigationElement đã được gắn kịch bản hay chưa
        if (hudNavigationElement == null)
        {
            enabled = false; // Tắt script nếu không tìm thấy HUDNavigationElement
        }
    }

    private void Update()
    {
        // Kiểm tra xem đối tượng có ở trong vùng bán kính không
        bool isInRadius = IsInRadius();

        // Bật/tắt script HUDNavigationElement dựa trên kết quả kiểm tra
        hudNavigationElement.enabled = !isInRadius;
    }

    private bool IsInRadius()
    {
        // Lấy vị trí của HUDNavigationElement và vị trí của người chơi (hoặc đối tượng khác)
        Vector3 elementPosition = transform.position;
        // Lấy vị trí của người chơi
        Vector3 playerPosition = GetPlayerPosition();

        // Tính khoảng cách giữa hai điểm
        float distance = Vector3.Distance(elementPosition, playerPosition);

        // Kiểm tra xem khoảng cách có nằm trong vùng bán kính không
        return distance >= radiusThreshold;
    }

    private Vector3 GetPlayerPosition()
    {
        // Lấy vị trí của người chơi từ một nguồn nào đó (ví dụ: script Movement)
        Movement movementScript = FindObjectOfType<Movement>();
        if (movementScript != null)
        {
            return movementScript.transform.position;
        }

        return Vector3.zero;
    }

   /* private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radiusThreshold);
    }*/
}