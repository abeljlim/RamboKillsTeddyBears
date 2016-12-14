using UnityEngine;
using System.Collections;

public class animator : MonoBehaviour {

    private Animator anim;

	// Use this for initialization
	void Start () {

        anim = GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            anim.SetBool("walk", true);
        } else
        {
            anim.SetBool("walk", false);
        }
	}
}
