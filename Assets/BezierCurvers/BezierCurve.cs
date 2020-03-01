using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by KadekSatriadi https://github.com/KadekSatriadi
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{
    public enum Order
    {
        Linear, Quadratic, Cubic
    }
    [Header("Bezier Curve settings")]
    public Order order;
    [Range(0, 1f)]
    public float t = 1f;

    [Header("Line settings")]
    [Range(0, 0.1f)]
    public float lineWidth = 0.01f;
    [Range(1, 500)]
    public int nsegments = 100;
    public Color color = Color.black;

    [Header("Guideline settings")]
    public bool showGuideLine = true;
    [Range(0, 0.007f)]
    public float guideLineWidth = 0.001f;
    public Color guideColor = Color.gray;

    [Header("Construction lines settings")]
    public bool showConstLine = true;
    [Range(0, 0.007f)]
    public float constLineWidth = 0.01f;
    public Color constColor1 = Color.yellow;
    public Color constColor2 = Color.blue;

    private LineRenderer line;
    private LineRenderer guideLine;
    private LineRenderer constrLine1;
    private LineRenderer constrLine2;
    private Transform p0, p1, p2, p3;

    private void Start()
    {
        //initate line
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.material = new Material(Shader.Find("Unlit/Color"));
       

        //initiate guide line
        GameObject gL = new GameObject("guideLine");
        gL.transform.SetParent(transform);
        guideLine = gL.AddComponent<LineRenderer>();
        guideLine.useWorldSpace = false;
        guideLine.material = new Material(Shader.Find("Unlit/Color"));

        //Construction lines
        GameObject gLC1 = new GameObject("constLine1");
        gLC1.transform.SetParent(transform);
        constrLine1 = gLC1.AddComponent<LineRenderer>();
        constrLine1.useWorldSpace = false;
        constrLine1.material = new Material(Shader.Find("Unlit/Color"));

        GameObject gLC2 = new GameObject("constLine2");
        gLC2.transform.SetParent(transform);
        constrLine2 = gLC2.AddComponent<LineRenderer>();
        constrLine2.useWorldSpace = false;
        constrLine2.material = new Material(Shader.Find("Unlit/Color"));

        //create control points
        p0 = CreateControlPoint("p0");
        p0.transform.localPosition -= transform.right * 0.5f;
        p1 = CreateControlPoint("p1");
        p2 = CreateControlPoint("p2");
        p2.transform.localPosition += transform.right * 0.5f;
        p3 = CreateControlPoint("p3");
        p3.transform.localPosition += transform.right * 1f;
    }

    private void Update()
    {
        switch (order)
        {
            case Order.Linear:
                CreateLinear();
                break;
            case Order.Quadratic:
                CreateQuadratic();
                break;
            case Order.Cubic:
                CreateCubic();
                break;
        }
        UpdateLine();
        if (!showGuideLine) showConstLine = false;
    }

    private void UpdateLine()
    {
        //line
        line.widthMultiplier = lineWidth;
        line.material.color = color;

        //guide line
        guideLine.widthMultiplier = guideLineWidth;
        guideLine.material.color = guideColor;
        guideLine.enabled = showGuideLine;

        //construction line
        constrLine1.widthMultiplier = constLineWidth;
        constrLine2.widthMultiplier = constLineWidth;
        constrLine1.material.color = constColor1;
        constrLine1.enabled = showConstLine;
        constrLine2.startColor = constColor2;
        constrLine2.material.color = constColor2;

    }

    private Transform CreateControlPoint(string name)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.name = name;
        g.transform.localScale = Vector3.one * 0.05f;
        g.transform.position = transform.position;
        g.transform.SetParent(transform);
        g.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Color"));
        g.GetComponent<MeshRenderer>().material.color = color;

        return g.transform;
    }

    private void SetVisibleControlPoint(Transform p, bool value)
    {
        p.GetComponent<MeshRenderer>().enabled = value;
        p.GetComponent<Collider>().enabled = value;
    }

    public void CreateCubic()
    {
        List<Vector3> points = BezierCurveFormula.GetCubic(p0.transform.localPosition, p1.transform.localPosition, p2.transform.localPosition, p3.transform.localPosition, t, nsegments);
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());

        SetVisibleControlPoint(p1, true);
        SetVisibleControlPoint(p2, true);

        //guide line
        guideLine.enabled = showGuideLine & true;
        guideLine.positionCount = 4;
        guideLine.SetPosition(0, p0.transform.localPosition);
        guideLine.SetPosition(1, p1.transform.localPosition);
        guideLine.SetPosition(2, p2.transform.localPosition);
        guideLine.SetPosition(3, p3.transform.localPosition);

        //construction line
        constrLine1.enabled = showConstLine & true;
        constrLine2.enabled = showConstLine & true;
        constrLine1.positionCount = 3;
        constrLine1.SetPosition(0, Vector3.Lerp(p0.localPosition, p1.localPosition, t));
        constrLine1.SetPosition(1, Vector3.Lerp(p1.localPosition, p2.localPosition, t));
        constrLine1.SetPosition(2, Vector3.Lerp(p2.localPosition, p3.localPosition, t));
        constrLine2.positionCount = 2;
        constrLine2.SetPosition(0, Vector3.Lerp(constrLine1.GetPosition(0), constrLine1.GetPosition(1), t));
        constrLine2.SetPosition(1, Vector3.Lerp(constrLine1.GetPosition(1), constrLine1.GetPosition(2), t));
    }

    public void CreateQuadratic()
    {
        List<Vector3> points = BezierCurveFormula.GetQuadratic(p0.transform.localPosition, p1.transform.localPosition, p3.transform.localPosition, t, nsegments);
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());

        SetVisibleControlPoint(p1, true);
        SetVisibleControlPoint(p2, false);

        //guide line
        guideLine.enabled = showGuideLine & true;
        guideLine.positionCount = 3;
        guideLine.SetPosition(0, p0.transform.localPosition);
        guideLine.SetPosition(1, p1.transform.localPosition);
        guideLine.SetPosition(2, p3.transform.localPosition);

        //construction line
        constrLine1.enabled = showConstLine & true;
        constrLine2.enabled = false;
        constrLine1.positionCount = 2;
        constrLine1.SetPosition(0, Vector3.Lerp(p0.localPosition, p1.localPosition, t));
        constrLine1.SetPosition(1, Vector3.Lerp(p1.localPosition, p3.localPosition, t));
       
    }

    public void CreateLinear()
    {
        List<Vector3> points = BezierCurveFormula.GetLinear(p0.transform.localPosition, p3.transform.localPosition, t, nsegments);
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());

        SetVisibleControlPoint(p1, false);
        SetVisibleControlPoint(p2, false);

        //guide line
        guideLine.positionCount = 0;
        constrLine1.positionCount = 0;
        constrLine2.positionCount = 0;
        guideLine.enabled = false;
        constrLine1.enabled = false;
        constrLine2.enabled = false;

    }

}


public static class BezierCurveFormula
{
    public static List<Vector3> GetLinear(Vector3 p0, Vector3 p1, float t, int nsegments)
    {
        List<Vector3> points = new List<Vector3>();
        float inc = 1f/nsegments;
        float i = 0f;
        while(i < t)
        {
            Vector3 pt = p0 + i * (p1 - p0);
            points.Add(pt);
            i += inc;
        }

        return points;
    }

    public static List<Vector3> GetQuadratic(Vector3 p0, Vector3 p1, Vector3 p2, float t, int nsegments)
    {
        List<Vector3> points = new List<Vector3>();
        float inc = 1f / nsegments;
        float i = 0f;
        while (i <= t)
        {
            Vector3 pt = p1 + (1f - i) * (1f - i) * (p0 - p1) + i * i * (p2 - p1);
            points.Add(pt);
            i += inc;
        }

        return points;
    }

    public static List<Vector3> GetCubic(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, int nsegments)
    {
        List<Vector3> points = new List<Vector3>();
        float inc = 1f / nsegments;
        float i = 0f;
        while (i <= t)
        {
            Vector3 pt = Mathf.Pow((1f - i), 3)  * p0 + 3f * (1f-i) * (1f-i) * i * p1 + 3f * (1f-i) * i * i * p2 + i * i * i * p3;
            points.Add(pt);
            i += inc;
        }

        return points;
    }
}
