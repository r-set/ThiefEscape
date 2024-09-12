using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicemanFieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)] public float viewAngle;

    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstracleMask;

    public List<Transform> visibleTarget;
    [SerializeField] private float _meshResolution;
    [SerializeField] private MeshFilter _viewMeshFilter;
    private Mesh _viewMesh;

    [SerializeField]  private Color _baseViewMeshColor;
    [SerializeField] private Color _yellowViewMeshColor;
    [SerializeField] private Color _redViewMeshColor;

    [SerializeField] private GameObject _redObject;
    [SerializeField] private GameObject _yellowObject;

    private void Start()
    {
        _viewMesh = new Mesh();
        _viewMesh.name = "View Mesh";
        _viewMeshFilter.mesh = _viewMesh;

        _baseViewMeshColor = _viewMeshFilter.GetComponent<Renderer>().material.color;

        StartCoroutine(nameof(FindTargetWithDelay), 0.2);
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            GetVisibleTarget();
        }
    }

    private void Update()
    {
        DrawFieldOfView();
    }

    private void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * _meshResolution);
        float stepAngle = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngle * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateBounds();
    }

    public void GetVisibleTarget()
    {
        visibleTarget.Clear();
        bool targetVisible = false;
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, _targetMask);
        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstracleMask))
                {
                    visibleTarget.Add(target);
                    targetVisible = true;
                    break;
                }
            }
        }


        if (targetVisible)
        {
            _viewMeshFilter.GetComponent<Renderer>().material.color = _redViewMeshColor;

            _redObject.SetActive(true);
        }
        else
        {
            _viewMeshFilter.GetComponent<Renderer>().material.color = _baseViewMeshColor;

            _redObject.SetActive(false);
        }
    }

    public Vector3 DirectionFormAngle(float angleInDegrees, bool isGlobalAngles)
    {
        if (!isGlobalAngles)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirectionFormAngle(globalAngle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, _obstracleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }
}
