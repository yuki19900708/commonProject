using UnityEngine;

public class CircularMotion : MonoBehaviour
{
    public Transform target;
    public Transform center;
    public float radius = 30;
    public bool localOrWorld = true;
    public float speed = 300;
    public float angle = 0;

    private void Start()
    {
        UpdatePosition(angle);
    }

    private void Update()
    {
        angle += Time.deltaTime * speed;
        UpdatePosition(angle);
    }

    private void UpdatePosition(float angle)
    {
        this.angle = angle;
        angle %= 360;

        float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        if (localOrWorld)
        {
            Vector3 newPos = center.TransformPoint(new Vector3(x, y, 0));
            newPos.z = target.position.z;
            target.position = newPos;
        }
        else
        {
            Vector3 newPos = center.position + new Vector3(x, y, 0);
            target.position = newPos;
        }
    }
}
