using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    // Số đơn vị res (độ phân giải) cho mỗi mặt hành tinh.
    [Range(2, 256)]
    public int resolution = 10;

    // Tự động cập nhật khi thay đổi các cài đặt hay không.
    public bool autoUpdate = true;

    // Định nghĩa loại hiển thị cho từng mặt của hành tinh.
    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
    public FaceRenderMask faceRenderMask;

    // Cài đặt hình dạng và màu sắc cho hành tinh.
    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    // Các biến ẩn, dùng để tự động thu gọn các phần cài đặt trong Inspector.
    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    // Generator hình dạng của hành tinh.
    ShapeGenerator shapeGenerator;

    // Danh sách MeshFilter và TerrainFace cho từng mặt của hành tinh.
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    // Hàm khởi tạo.
    void Initialize()
    {
        // Tạo generator hình dạng.
        shapeGenerator = new ShapeGenerator(shapeSettings);

        // Khởi tạo danh sách MeshFilter và TerrainFace.
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        // Mảng hướng vector cho mỗi mặt của hành tinh.
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        // Lặp qua từng mặt của hành tinh.
        for (int i = 0; i < 6; i++)
        {
            // Nếu MeshFilter chưa được khởi tạo, tạo mới.
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                // Thêm MeshRenderer và thiết lập vật liệu.
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            // Khởi tạo TerrainFace cho mỗi mặt và kiểm tra hiển thị.
            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    // Tạo hành tinh.
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();

        // Thiết lập mesh collider sau khi tạo mesh.
        SetupMeshColliders();
    }

    // Gọi khi có cập nhật các cài đặt hình dạng.
    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    // Gọi khi có cập nhật các cài đặt màu sắc.
    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    // Tạo mesh cho từng mặt của hành tinh.
    void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }
    }

    // Thiết lập màu sắc cho hành tinh.
    void GenerateColours()
    {
        foreach (MeshFilter m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colourSettings.planetColour;
        }
    }

    // Thiết lập mesh collider cho hành tinh.
    void SetupMeshColliders()
    {
        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.gameObject.activeSelf)
            {
                MeshCollider meshCollider = meshFilter.gameObject.GetComponent<MeshCollider>();

                // Nếu không có mesh collider, tạo mới.
                if (meshCollider == null)
                {
                    meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                }

                // Thiết lập mesh cho collider.
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
        }
    }
}
