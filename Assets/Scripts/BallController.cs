using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{

    Vector3 startPosition;
    public float flightDistance = 30f;
    public float speed = 5f;
    public int damage = 20;

    public AudioClip startSound;
    public AudioClip endSound;
    public Rigidbody rb;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        AudioSource.PlayClipAtPoint(startSound, transform.position);
        rb.AddRelativeForce(Vector3.forward * speed , ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - startPosition).magnitude >= flightDistance)
            Destroy(gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
            player.Damage(damage);
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        AudioSource.PlayClipAtPoint(endSound, transform.position);
    }
}
