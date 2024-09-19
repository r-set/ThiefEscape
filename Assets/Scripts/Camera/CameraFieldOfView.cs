using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFieldOfView : MonoBehaviour
{
    public float coneHeight = 3f;
    public float coneRadius = 2f;

    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();
    [SerializeField] private MeshFilter _coneMeshFilter;
    private Mesh _coneMesh;

    [SerializeField] private Color _baseViewMeshColor;
    [SerializeField] private Color _redViewMeshColor;

    private void Start()
    {
        _coneMesh = new Mesh();
        _coneMesh.name = "Cone Mesh";
        _coneMeshFilter.mesh = _coneMesh;

        CreateConeMesh(coneHeight, coneRadius);

        StartCoroutine(FindTargetWithDelay(0.2f));
    }

    private void CreateConeMesh(float height, float radius)
    {
        int segments = Mathf.Max(3, Mathf.RoundToInt(radius * 10f));
        float angleStep = 360f / segments;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 12];

        vertices[0] = Vector3.up * height;
        vertices[1] = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[i + 2] = new Vector3(x, 0, z);
        }

        for (int i = 0; i < segments; i++)
        {
            int current = i + 2;
            int next = (i + 1) % segments + 2;

            if (i * 3 + 2 < triangles.Length)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = current;
                triangles[i * 3 + 2] = next;
            }
        }

        for (int i = 0; i < segments; i++)
        {
            int current = i + 2;
            int next = (i + 1) % segments + 2;

            if (segments * 3 + i * 3 + 2 < triangles.Length)
            {
                triangles[segments * 3 + i * 3] = 1;
                triangles[segments * 3 + i * 3 + 1] = next;
                triangles[segments * 3 + i * 3 + 2] = current;
            }
        }

        _coneMesh.Clear();
        _coneMesh.vertices = vertices;
        _coneMesh.triangles = triangles;
        _coneMesh.RecalculateNormals();
    }

    private IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            GetVisibleTargets();
        }
    }

    private void GetVisibleTargets()
    {
        visibleTargets.Clear();
        bool targetVisible = false;

        Collider[] targetsInView = Physics.OverlapSphere(transform.position, coneRadius, _targetMask);

        for (int i = 0; i < targetsInView.Length; i++)
        {
            Transform target = targetsInView[i].transform;
            if (IsTargetInCone(target))
            {
                visibleTargets.Add(target);
                targetVisible = true;
                break;
            }
        }

        _coneMeshFilter.GetComponent<Renderer>().material.color = targetVisible ? _redViewMeshColor : _baseViewMeshColor;
    }

    private bool IsTargetInCone(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        directionToTarget.y = 0;

        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget.normalized);

        return distanceToTarget <= coneRadius && angleToTarget <= Mathf.Atan2(coneRadius, coneHeight) * Mathf.Rad2Deg;
    }
}
