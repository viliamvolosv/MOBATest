using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class FireController : MonoBehaviour {

    Animator animator;
    public Transform ball;
    public float cooldown = 2f;
    public float lastFireTime = 0f;
    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
            if((Time.realtimeSinceStartup - lastFireTime) >cooldown)
            {
                lastFireTime = Time.realtimeSinceStartup;
                Instantiate(ball,transform.position+transform.forward + transform.up, transform.rotation);
                animator.SetTrigger("Fire");
                SendFireEvent();
            }
	}

    void SendFireEvent()
    {
            Messages message = new Messages();
            message.type = Messages.Type.Fire;
            Fire state = new Fire();
            state.position = new Vector3Serializer();
            state.position.Fill(transform.position + transform.forward + transform.up);
            state.rotation = new QuaternionSerializer();
            state.rotation.Fill(transform.rotation);
            message.content = state;
            NetworkManager.Instance.SendGameMsg(message);
    }
}
