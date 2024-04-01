using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    // Generator hình dạng sẽ được sử dụng để tính toán độ cao của điểm trên hành tinh.
    ShapeGenerator shapeGenerator;

    // Mesh của mặt đất.
    Mesh mesh;

    // Độ phân giải của mesh.
    int resolution;

    // Hướng "lên" của mặt đất.
    Vector3 localUp;

    // Hai trục vuông góc với nhau trên mặt đất.
    Vector3 axisA;
    Vector3 axisB;

    // Constructor, được gọi khi khởi tạo một mặt của hành tinh.
    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        // Tính toán hai trục vuông góc với hướng "lên".
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    // Phương thức này tạo mesh cho mặt đất.
    public void ConstructMesh()
    {
        // Khởi tạo mảng chứa tất cả các điểm trên mặt đất.
        Vector3[] vertices = new Vector3[resolution * resolution];

        // Mảng chứa tất cả các tọa độ UV (được sử dụng để ánh xạ ảnh lên mesh).
        Vector2[] uv = new Vector2[resolution * resolution];

        // Mảng chứa thông tin về các tam giác trong mesh.
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        // Biến đếm tam giác.
        int triIndex = 0;

        // Lặp qua từng điểm trên mặt đất.
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;

                // Tính toán tỷ lệ theo phần trăm dọc và ngang trên mặt đất.
                Vector2 percent = new Vector2(x, y) / (resolution - 1);

                // Tính toán vị trí trên cube đơn vị.
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;

                // Chuyển vị trí từ cube đơn vị sang trên hình cầu đơn vị.
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                // Tính toán độ cao của điểm trên hành tinh sử dụng generator hình dạng.
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                // Tính toán tọa độ UV để ánh xạ ảnh lên mesh.
                uv[i] = new Vector2(percent.x, percent.y);

                // Kiểm tra xem có tạo tam giác không.
                if (x != resolution - 1 && y != resolution - 1)
                {
                    // Tạo hai tam giác cho mỗi ô vuông trên mặt đất.
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;

                    triIndex += 6;
                }
            }
        }

        // Xóa mesh hiện tại.
        mesh.Clear();

        // Gán dữ liệu mới cho mesh.
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals(); // Tính toán lại các pháp tuyến cho mỗi điểm trên mesh.
    }
}
