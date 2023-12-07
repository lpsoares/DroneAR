using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRay : MonoBehaviour
{

    public Transform target1;
    public Transform target2;

    private int layerMask = 1 << 4;

    // Start is called before the first frame update
    void Start()
    {
        float delta = 0.1f;
        float radius = 0.5f;

        RaycastHit hitInfo, hitInfo2;
        if (Physics.Linecast(target1.position, target2.position, out hitInfo, layerMask)) {

            Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.green);
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = hitInfo.point + (delta * hitInfo.normal);
            sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            Physics.Linecast(target2.position, target1.position, out hitInfo2, layerMask);
            Debug.DrawRay(hitInfo2.point, hitInfo2.normal, Color.green);
            GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere2.transform.position = hitInfo2.point + (delta * hitInfo2.normal);
            sphere2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            GameObject sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //Vector3 vector3 = Vector3.Lerp(hitInfo.point, hitInfo2.point, 0.5f);
            Vector3 vector3 = hitInfo.collider.transform.position;
            Vector3 normal3 = (hitInfo.normal + hitInfo2.normal).normalized;
            sphere3.transform.position = vector3 + ( (radius+delta) * normal3 );
            sphere3.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            // if (hitInfo.rigidbody != null)
            // {
            //     Debug.Log("rigidbody");
            //     GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //     sphere.transform.position = hitInfo.point;
            // }
        }



    }

    
    
    // Update is called once per frame
    void Update() {



        
    }
}
