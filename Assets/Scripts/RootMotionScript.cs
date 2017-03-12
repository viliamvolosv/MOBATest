using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class RootMotionScript : MonoBehaviour
{
    float m_TurnAmount;

    void OnAnimatorMove()
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            Vector3 newPosition = transform.position;
            newPosition+= transform.forward * animator.GetFloat("Forward") * Time.deltaTime *5f;
            transform.position = newPosition;
        }
    }
}
