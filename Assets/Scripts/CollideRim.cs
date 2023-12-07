using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CollideRim : MonoBehaviour
{

    Vector3 originalPos;

    public Material MaterialBase;
    public Material MaterialError;

    AudioSource audioSource;
    MeshRenderer mr;

    List<Vector3> playerPos = new List<Vector3>();

    bool record = false;

    void Start()
    {
        originalPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        audioSource = GetComponent<AudioSource>();
        mr = GetComponent<MeshRenderer>();
    }

    void Update() {
        
        if(record) {
            playerPos.Add(gameObject.transform.position);
        }

        // Cas cube is free falling
        if(gameObject.transform.position.y < 0.1) {
            ResetPosition();
            record = false;
            playerPos.Clear();
        }
    }

    void OnCollisionEnter(Collision collision) {

        // Case cube colides with rim
        if(collision.gameObject.tag == "Rim"){
            if (collision.relativeVelocity.magnitude > 2)
                audioSource.Play();
            mr.material = MaterialError;
            StartCoroutine(InvokeDelayed(3));
        }

        // Case cube colides with finish
        if(collision.gameObject.tag == "Finish"){
            record = false;
            
            //Color red = Color.red;
            Color tmp_color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            GameObject new_line = new GameObject("New Line");

            LineRenderer lineRenderer = new_line.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = tmp_color;
            lineRenderer.endColor = tmp_color;
            //lineRenderer.SetColors(tmp_color, tmp_color);
            
            //lineRenderer.SetWidth(0.003f, 0.003f);
            lineRenderer.startWidth = 0.003f;
            lineRenderer.endWidth = 0.003f;

            //lineRenderer.SetVertexCount(playerPos.Count);
            lineRenderer.positionCount = playerPos.Count;
            for (int i = 0; i < playerPos.Count; i++ )
                lineRenderer.SetPosition(i, playerPos[i]);
            
            // TESTAR TESTAR
            //lineRenderer.SetPositions(playerPos);
            
            //Add Components (not sure yet if all are needed)
            new_line.AddComponent<Rigidbody>();
            //new_line.AddComponent<MeshFilter>();
            new_line.AddComponent<BoxCollider>();
            //new_line.AddComponent<MeshRenderer>();
            new_line.AddComponent<XRGrabInteractable>();

            ResetPosition();

        }

    }

    // Case the Cube is out of the base
    void OnCollisionExit(Collision collision) {
        if(collision.gameObject.tag == "Respawn"){
            playerPos.Clear();
            //Destroy(GetComponent<LineRenderer>());
            record = true; // Start recording
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Rim")) { 
    //         // if (collision.relativeVelocity.magnitude > 2)
    //         //     audioSource.Play(); 
    //         mr.material = MaterialError;
    //         StartCoroutine(InvokeDelayed(3));
    //     }
    // }

    private IEnumerator InvokeDelayed(float delay) {
        yield return new WaitForSeconds(delay);
        mr.material = MaterialBase;
    }


    void ResetPosition() {
        // Release the object
        XRGrabInteractable interactable = GetComponent<XRGrabInteractable>();
        interactable.interactionManager.CancelInteractableSelection(interactable);
        //Use CancelInteractableSelection(IXRSelectInteractable) instead.'

        // Reset speed, rotation and position
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.transform.position = originalPos;
    }
}
