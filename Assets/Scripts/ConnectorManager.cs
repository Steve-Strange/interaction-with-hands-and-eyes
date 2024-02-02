using UnityEngine;

public class ConnectorManager : MonoBehaviour
{
    public GameObject cube1;
    public GameObject cube2;
    public GameObject cube3;
    public GameObject cube4;

    private LineRenderer lineRenderer;
    public Color lineColor = Color.red; // 设置默认颜色
    public float lineWidth = 0.1f; // 设置默认宽度

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // 设置LineRenderer的颜色
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    void Update()
    {
        // 获取方块的位置
        Vector3[] positions = new Vector3[4];
        positions[0] = cube1.transform.position;
        positions[1] = cube2.transform.position;
        positions[2] = cube3.transform.position;
        positions[3] = cube4.transform.position;

        // 设置LineRenderer的位置
        lineRenderer.positionCount = 4;
        lineRenderer.SetPositions(positions);
    }
}
