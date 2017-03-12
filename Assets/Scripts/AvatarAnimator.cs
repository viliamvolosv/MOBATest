using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AvatarAnimator : MonoBehaviour {

    Vector3 lastPosition;

    Animator animator;
    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
            animator.SetFloat("Forward", Mathf.Clamp((transform.position - lastPosition).magnitude *7 ,0,1), 0.1f, Time.deltaTime);
        
            
        lastPosition = transform.position;
    }
}
