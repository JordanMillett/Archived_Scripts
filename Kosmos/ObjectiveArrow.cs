using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveArrow : MonoBehaviour
{
    public bool Visible = true;
    public Vector3 Destination;

    public GameObject Arrow;
    public GameObject Target;
    RectTransform RT;

    void Start()
    {
        RT = GetComponent<RectTransform>();
    }

    void OnDisable()
    {
        Target.SetActive(false);
    }

    void Update()
    {
        if(Visible)
            Point(Destination);
        else
        {
            Arrow.SetActive(false);
            Target.SetActive(false);
        }
    }

    void Point(Vector3 Pos)
    {
        Pos = new Vector3(Pos.x, this.transform.position.y, Pos.z);

        float Distance = Vector3.Distance(Pos, this.transform.position);

        if(Distance > UniversalConstants.CullDistance)
        {
            Target.SetActive(true);
            Arrow.SetActive(true);
            Vector3 Direction = Pos - this.transform.position;
            Vector3 RotationVector = new Vector3(0f, 0f, Vector3.SignedAngle(Direction, Vector3.right, Vector3.up));
            RT.localEulerAngles = RotationVector;
            Target.transform.position = Pos;
        }else
        {
            Target.SetActive(true);
            Arrow.SetActive(false);

            Target.transform.position = Pos;
        }

    }
}
