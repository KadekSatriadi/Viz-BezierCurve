using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BezierCurveInput : MonoBehaviour
{

    public Color hoverColor;
    public Color selectColor;
    public BezierCurve curve;
    public Text titleText;
    public Slider tSlider;
    public Text tText;
    public Text desText;

    private enum Status
    {
        Null, Drag, Hover
    }
    private Status status = Status.Null;
    private Transform hitTransform;
    private Color prevColor;
    // Update is called once per frame

    private void Start()
    {
        tSlider.onValueChanged.AddListener(delegate { UpdateT(); });
    }
    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(status == Status.Null)
            {
                hitTransform = hit.transform;
                prevColor = hitTransform.GetComponent<Renderer>().material.color;
                status = Status.Hover;
            }
        }
        else
        {
            if (status != Status.Drag)
            {
                status = Status.Null;
                if (hitTransform) hitTransform.GetComponent<Renderer>().material.color = prevColor;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit2))
            {
                hitTransform = hit2.transform;
                status = Status.Drag;
            }            
        }

        if (Input.GetMouseButtonUp(0))
        {
            status = Status.Null;
            if(hitTransform) hitTransform.GetComponent<Renderer>().material.color = prevColor;
        }

        if (status == Status.Drag)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position = hitTransform.parent.InverseTransformPoint(position);
            hitTransform.localPosition = new Vector3(position.x, position.y, 0);
            hitTransform.GetComponent<Renderer>().material.color = selectColor;

        }

        if(status == Status.Null)
        {
            if(hitTransform) hitTransform.GetComponent<Renderer>().material.color = prevColor;
        }

        if (status == Status.Hover)
        {
            hitTransform.GetComponent<Renderer>().material.color = hoverColor;
        }


        titleText.text = curve.order.ToString();
        tText.text = "t = " + curve.t.ToString("F2");
        curve.t = tSlider.value;

        switch (curve.order)
        {
            case BezierCurve.Order.Cubic:
                desText.text = "Cubic bezier curve has two control points.";
                break;
            case BezierCurve.Order.Quadratic:
                desText.text = "Quadratic bezier curve has a single control point.";
                break;
            case BezierCurve.Order.Linear:
                desText.text = "Zero control point. Yes, it's a straight line.";
                break;
        }
    }

    private void UpdateT()
    {
        curve.t = tSlider.value;
    }
}
