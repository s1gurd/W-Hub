using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterControll : MonoBehaviour
{
    static Animator anim;
    public float speed = 10.0F;
    public float rotationSpeed = 100.0F;
     //Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        if (Input.GetButtonDown("Jump") && anim.GetBool("isSitting") == false)
        {
            anim.SetBool("isSitting", true);
        }
        else if (Input.GetButtonDown("Jump") && anim.GetBool("isSitting") == true)
        {
            anim.SetBool("isSitting", false);
        }

        if (Input.GetButtonDown("Fire1")) 
        {
            anim.SetTrigger("isWaving");
        }
        if (Input.GetButtonDown("Fire2"))
        {
            anim.SetTrigger("isThumbingUp");
        }
        if(translation != 0)
        {
            anim.SetBool("isWalking", true);
        }
        else 
        {
            anim.SetBool("isWalking", false);
        }
        
    }
}
