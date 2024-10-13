using UnityEngine;

public class CameraAlarm : MonoBehaviour
{
    [SerializeField] private MeshFilter _coneMeshFilter;
    [SerializeField] private SpriteRenderer _circleSpriteRenderer;

    [SerializeField] private Color _searchViewMeshColor;
    [SerializeField] private Color _alarmViewMeshColor;

    private void Start()
    {
        _coneMeshFilter.GetComponent<Renderer>().material.color = _searchViewMeshColor;
        _circleSpriteRenderer.GetComponent<Renderer>().material.color = _searchViewMeshColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        _coneMeshFilter.GetComponent<Renderer>().material.color = _alarmViewMeshColor;
        _circleSpriteRenderer.GetComponent<Renderer>().material.color = _alarmViewMeshColor;
    }

    private void OnTriggerExit(Collider other)
    {
        _coneMeshFilter.GetComponent<Renderer>().material.color = _searchViewMeshColor;
        _circleSpriteRenderer.GetComponent<Renderer>().material.color = _searchViewMeshColor;
    }
}
