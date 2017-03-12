using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AvatarAnimator : MonoBehaviour {

    Vector3 lastPosition;
    public float dumptime = 0.1f;
    public float f;
    Animator animator;
    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        f = Mathf.Clamp((transform.position - lastPosition).magnitude * 5f, 0, 1);
        //float f = (transform.position - lastPosition).magnitude;
        f = f < 0.2 ? 0 : f;
        animator.SetFloat("Forward", f, dumptime, Time.deltaTime);
        lastPosition = transform.position;
    }
}
