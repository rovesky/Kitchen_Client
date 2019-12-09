using System.Collections;
using System.Collections.Generic;
using FootStone.ECS;
using UnityEngine;

public class CharacterSkinController : MonoBehaviour
{
    Animator animator;
    Renderer[] characterMaterials;

    public Texture2D[] albedoList;
    [ColorUsage(true,true)]
    public Color[] eyeColors;
    public enum EyePosition { normal, happy, angry, dead}
    public EyePosition eyeState;
    private int lastIndex;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterMaterials = GetComponentsInChildren<Renderer>();

        FSLog.Info($"characterMaterials:{characterMaterials.Length}");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeMaterialSettings(0);
            ChangeEyeOffset(EyePosition.normal);
            ChangeAnimatorIdle("normal");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeMaterialSettings(1);
            ChangeEyeOffset(EyePosition.angry);
            ChangeAnimatorIdle("angry");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeMaterialSettings(2);
            ChangeEyeOffset(EyePosition.happy);
            ChangeAnimatorIdle("happy");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeMaterialSettings(3);
            ChangeEyeOffset(EyePosition.dead);
            ChangeAnimatorIdle("dead");
        }

     
    }

    void ChangeAnimatorIdle(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    public void ChangeMaterialSettings(int index)
    {
        if (lastIndex == index)
            return;

        if (characterMaterials == null)
            characterMaterials = GetComponentsInChildren<Renderer>();

        FSLog.Info($"ChangeMaterialSettings:{characterMaterials.Length},{index}");
        for (int i = 0; i < characterMaterials.Length; i++)
        {
            if (characterMaterials[i].transform.CompareTag("PlayerEyes"))
                characterMaterials[i].material.SetColor("_EmissionColor", eyeColors[index]);
            else
                characterMaterials[i].material.SetTexture("_MainTex", albedoList[index]);
        }

        lastIndex = index;
    }

    void ChangeEyeOffset(EyePosition pos)
    {
        Vector2 offset = Vector2.zero;

        switch (pos)
        {
            case EyePosition.normal:
                offset = new Vector2(0, 0);
                break;
            case EyePosition.happy:
                offset = new Vector2(.33f, 0);
                break;
            case EyePosition.angry:
                offset = new Vector2(.66f, 0);
                break;
            case EyePosition.dead:
                offset = new Vector2(.33f, .66f);
                break;
            default:
                break;
        }

        for (int i = 0; i < characterMaterials.Length; i++)
        {
            if (characterMaterials[i].transform.CompareTag("PlayerEyes"))
                characterMaterials[i].material.SetTextureOffset("_MainTex", offset);
        }
    }
}
