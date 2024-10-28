using System.Collections.Generic;
using UnityEngine;

public class TriangleData
{
    public List<Vector3> Corners { get; private set; }

    public TriangleData(List<Vector3> corners)
    {
        Corners = corners;
    }
}
