using System;
using System.Collections;
using System.Collections.Generic;
using Meta.WitAi;
using UnityEngine;

public class RayController : MonoBehaviour {

    public Transform anchor;
    public AudioClip sound;

    public float distance = 1.0f;

    public GameObject sphere;
    public GameObject drone;

    public GameObject colisor;

    private LineRenderer lineRenderer;

    Vector3 origin_drone;

    // To draw the track (not the ray)
    public GameObject track;


    // Vector with points in the space
    List<Vector3> points;
    List<Quaternion> rotations;

    List<Color> colors;

    public float duration = 3.0f;

	private float progress;

    private int count = 0;

    bool play;

    //private float total_distance = 0;

    private GameObject tmp_sphere = null;

    Quaternion tmp_rot_control;
    Quaternion tmp_rot_obj;
    AudioSource audioData;

    float direction;
    Vector3 tmp_pos;
    Quaternion tmp_rot;

    void Reset() {
        direction = 1.0f;
        play = false;
        tmp_sphere = null;
        points = new List<Vector3>();
        rotations = new List<Quaternion>();
        colors = new List<Color>();
    }

    void Start()
    {
        progress = -1.0f;
        origin_drone = drone.transform.position;
        tmp_pos = origin_drone;
        lineRenderer = GetComponent<LineRenderer>();

        audioData = drone.GetComponent<AudioSource>();
        audioData.Play(0);
        audioData.Pause();

        Reset();
    }

    void CreateMark(Vector3 new_pos) {

        tmp_sphere = Instantiate(sphere, new_pos, Quaternion.identity);
        tmp_sphere.GetComponent<MarkColor>().colisor = colisor;
        
        points.Add(new_pos);
        rotations.Add(Quaternion.identity);

        colors.Add(tmp_sphere.GetComponent<MarkColor>().GetColor());

        if(points.Count > 1) {
            // Segment
            GameObject tmp_track = Instantiate(track);
            LineRenderer trackRenderer = tmp_track.GetComponent<LineRenderer>();
            trackRenderer.startWidth = 0.01f;
            trackRenderer.endWidth = 0.01f;
            trackRenderer.SetPosition(0, points[points.Count-1]);
            trackRenderer.SetPosition(1, points[points.Count-2]);

            // Color for the segment
            Gradient gradient = new Gradient();
            GradientAlphaKey[] grad_alphas = new GradientAlphaKey[2];
            grad_alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
            grad_alphas[1] = new GradientAlphaKey(1.0f, 1.0f); //opaque
            GradientColorKey[] grad_colors;
            grad_colors = new GradientColorKey[2];
            grad_colors[0] = new GradientColorKey(colors[points.Count-1], 0.0f);
            grad_colors[1] = new GradientColorKey(colors[points.Count-2], 1.0f);
            gradient.SetKeys(grad_colors, grad_alphas);
            trackRenderer.colorGradient = gradient;
        }
    }

    Vector3[] TesteRay(Vector3 target1) {

        int layerMask = 1 << 4;

        float delta = 0.2f;
        float radius = 0.5f;

        if(points.Count > 0) {

            Vector3 target2 = points[points.Count-1];

            RaycastHit hitInfo, hitInfo2;
            if (Physics.Linecast(target1, target2, out hitInfo, layerMask)) {

                Vector3[] new_points = new Vector3[3];

                //Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.green);
                //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                new_points[2] = hitInfo.point + (delta * hitInfo.normal);
                //sphere.transform.position = hitInfo.point + (delta * hitInfo.normal);
                //sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                Physics.Linecast(target2, target1, out hitInfo2, layerMask);
                //Debug.DrawRay(hitInfo2.point, hitInfo2.normal, Color.green);
                //GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                new_points[0] = hitInfo2.point + (delta * hitInfo2.normal);
                //sphere2.transform.position = hitInfo2.point + (delta * hitInfo2.normal);
                //sphere2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                //GameObject sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //Vector3 vector3 = Vector3.Lerp(hitInfo.point, hitInfo2.point, 0.5f);
                Vector3 vector3 = hitInfo.collider.transform.position;
                Vector3 normal3 = (hitInfo.normal + hitInfo2.normal).normalized;
                new_points[1] = vector3 + ( (radius+delta) * normal3 );
                //sphere3.transform.position = vector3 + ( (radius+delta) * normal3 );
                //sphere3.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                return new_points;
            }
        }

        return null;

    }

    void Update() {

        // Pointer line (controller)
        Ray ray = new Ray(anchor.position, anchor.forward);
        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, ray.origin + (ray.direction * distance));

        // Rotate last point
        if(points.Count>0 && tmp_sphere != null) {
            // if(OVRInput.GetDown(OVRInput.RawButton.RThumbstickUp)) tmp_sphere.transform.Rotate(0.0f, 0.0f, -10.0f);
            // if(OVRInput.GetDown(OVRInput.RawButton.RThumbstickDown)) tmp_sphere.transform.Rotate(0.0f, 0.0f, 10.0f);
            // if(OVRInput.GetDown(OVRInput.RawButton.RThumbstickLeft)) tmp_sphere.transform.Rotate(10.0f, 0.0f, 0.0f);
            // if(OVRInput.GetDown(OVRInput.RawButton.RThumbstickRight)) tmp_sphere.transform.Rotate(-10.0f, 0.0f, 0.0f);

            if(OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)) {
                tmp_rot_control = Quaternion.Inverse(OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch));
                tmp_rot_obj = tmp_sphere.transform.rotation;
            }
            if(OVRInput.Get(OVRInput.RawButton.RHandTrigger)) {
                tmp_sphere.transform.rotation = tmp_rot_obj * (OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * tmp_rot_control);
            }

            rotations[points.Count-1] = tmp_sphere.transform.rotation;
        }

        // Restart
        if(OVRInput.GetDown(OVRInput.RawButton.RThumbstick)) {
            foreach (var gameObj in GameObject.FindGameObjectsWithTag("Respawn"))
                Destroy(gameObj);
            Reset();
        }

        if(OVRInput.GetDown(OVRInput.RawButton.B)) {
            play = !play;
            direction = 1.0f;
        }

        if(play && points.Count>1) {
            if (progress > (float)(points.Count-1)) {
                direction = -1.0f;
            } 
            if(progress<0 && direction<0) {
                direction = 1.0f;
            }
            progress += direction * Time.deltaTime / duration;
            if(progress<0) {  //first iteration
                drone.transform.localPosition = Vector3.Lerp(origin_drone, points[0], 1.0f + progress);
            } else {
                count = (int)progress;
                if(count < points.Count-1) {
                    drone.transform.localPosition = Vector3.Lerp(points[count], points[count+1], (progress-(float)count));
                    drone.transform.rotation = Quaternion.Lerp(rotations[count], rotations[count+1], (progress-(float)count));
                }
            }

            // Rotate fan
            foreach (var gameObj in GameObject.FindGameObjectsWithTag("Rim"))
                gameObj.transform.Rotate(0.0f, 0.0f, 30.0f);
            audioData.UnPause();
        } else {
            if(progress>=0.0f) {
                tmp_pos = drone.transform.localPosition;
                tmp_rot = drone.transform.rotation;
                progress = 0.0f;
            }
            if(progress>-1.0f) {
                progress -= Time.deltaTime / duration;
                drone.transform.localPosition = Vector3.Lerp(tmp_pos, origin_drone, -progress);
                drone.transform.rotation = Quaternion.Lerp(tmp_rot, Quaternion.identity, -progress);
            } else {
                audioData.Pause();
            }
            
        }


        //if(OVRInput.GetDown(OVRInput.RawButton.A)){
        // Create a sphere
        if(OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger)){

            // Distance from the colider
            Vector3 new_pos = ray.origin + (ray.direction * distance);
            float d = (new_pos - colisor.transform.position).magnitude;

            if(d<0.5) {return;} // Inside the colisor
            

            Vector3[] new_points = TesteRay(new_pos);
            if(new_points != null) {
                CreateMark(new_points[0]);
                CreateMark(new_points[1]);
                CreateMark(new_points[2]);
            }

            CreateMark(new_pos);
           
        }

    }
}