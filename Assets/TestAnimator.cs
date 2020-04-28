using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimator : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
      
        var animatorInfo = animator.GetCurrentAnimatorStateInfo (0);//必须放在update里
        Debug.Log($"animatorInfo.normalizedTime:{animatorInfo.normalizedTime}");
        if ((animatorInfo.normalizedTime >= 0.7f) && (animatorInfo.IsName("Base Layer.open")))//normalizedTime: 范围0 -- 1,  0是动作开始，1是动作结束
        {
            animator.SetBool("Opening",false);
        }

  
        if(Input.GetKey(KeyCode.A))
            animator.SetBool("Opening",true);
       // else
          //  animator.SetBool("Opening",false);
        

    }
}
