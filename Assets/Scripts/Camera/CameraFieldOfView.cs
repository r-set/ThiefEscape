using UnityEngine;

public class CameraFieldOfView : MonoBehaviour
{
    [SerializeField] private float _coneHeight = 3f;
    [SerializeField] private float _coneRadius = 2f;
    [SerializeField] private int _segments = 12;

    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _playerLayer;

    [SerializeField] private MeshFilter _coneMeshFilter;
    [SerializeField] private GameObject _circleObject;

    [SerializeField] private Color _searchViewMeshColor;
    [SerializeField] private Color _alarmViewMeshColor;
    private Mesh _coneMesh;

    private void Start()
    {
        CapsuleCollider capsuleCollider = _circleObject.AddComponent<CapsuleCollider>();
        capsuleCollider.isTrigger = true;
        capsuleCollider.height = 4f;
        capsuleCollider.radius = 0.5f;

        _coneMesh = new Mesh();
        _coneMesh.name = "Cone Mesh";
        _coneMeshFilter.mesh = _coneMesh;

        CreateConeMesh(_coneHeight, _coneRadius);
        _coneMeshFilter.GetComponent<Renderer>().material.color = _searchViewMeshColor;
    }

    private void CreateConeMesh(float height, float radius)
    {
        float angleStep = 360f / _segments;

        Vector3[] vertices = new Vector3[_segments + 2];
        int[] triangles = new int[_segments * 3 * 2];

        vertices[0] = Vector3.up * height;
        vertices[1] = Vector3.zero;

        for (int i = 0; i < _segments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            Vector3 direction = new Vector3(x, 0, z).normalized;
            Ray ray = new Ray(Vector3.up * height, direction);

            if (Physics.Raycast(ray, out RaycastHit hit, height, _obstacleMask))
            {
                vertices[i + 2] = hit.point;
            }
            else
            {
                vertices[i + 2] = new Vector3(x, 0, z);
            }
        }

        for (int i = 0; i < _segments; i++)
        {
            int current = i + 2;
            int next = (i + 1) % _segments + 2;

            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = current;
            triangles[i * 3 + 2] = next;
        }

        for (int i = 0; i < _segments; i++)
        {
            int current = i + 2;
            int next = (i + 1) % _segments + 2;

            triangles[_segments * 3 + i * 3] = 1;
            triangles[_segments * 3 + i * 3 + 1] = next;
            triangles[_segments * 3 + i * 3 + 2] = current;
        }

        _coneMesh.Clear();
        _coneMesh.vertices = vertices;
        _coneMesh.triangles = triangles;
        _coneMesh.RecalculateNormals();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited by: " + other.gameObject.name);
    }
}
