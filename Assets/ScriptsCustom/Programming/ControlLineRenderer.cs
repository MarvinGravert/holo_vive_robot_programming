using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private List<Waypoint>  wpoints;
    public Color lineColor = Color.red;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
    }

    public void SetupLineRenderer(List<Waypoint> wpoints)
    {
        lineRenderer.positionCount = wpoints.Count;
        this.wpoints = wpoints;

    }

    
    private void Update()
    {
        
        for (int i =0; i< wpoints.Count; i++)
        {
            lineRenderer.SetPosition(i, wpoints[i].obj.transform.position);
            
        }
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }
}
