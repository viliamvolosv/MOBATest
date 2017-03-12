using UnityEngine;
using System.Collections;
using UnityEngine.UI;


[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour {

    Animator animator;
    int health = 100;
    public Slider slider;
    // Use this for initialization
    void Start () {
        slider.minValue = 0;
        slider.maxValue = health;
        slider.value = health;
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Damage(int damage)
    {
        health -= damage;
        slider.value = health;
        if(health<=0)
            animator.SetTrigger("Death");
    }
}
