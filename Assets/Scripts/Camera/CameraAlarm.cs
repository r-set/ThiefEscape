using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAlarm : MonoBehaviour
{
    [SerializeField] private MeshFilter _coneMeshFilter;
    [SerializeField] private MeshFilter _circleMeshFilter;

    [SerializeField] private Color _searchViewMeshColor;
    [SerializeField] private Color _alarmViewMeshColor;

    private void OnTriggerEnter(Collider other)
    {
        _coneMeshFilter.GetComponent<Renderer>().material.color = _alarmViewMeshColor;
        _circleMeshFilter.GetComponent<Renderer>().material.color = _alarmViewMeshColor;
    }

    private void OnTriggerExit(Collider other)
    {
        _coneMeshFilter.GetComponent<Renderer>().material.color = _searchViewMeshColor;
        _circleMeshFilter.GetComponent<Renderer>().material.color = _searchViewMeshColor;
    }
}
