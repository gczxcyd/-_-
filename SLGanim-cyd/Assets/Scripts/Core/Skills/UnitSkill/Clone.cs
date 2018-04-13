﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//分身术
public class Clone : UnitSkill
{
    GameObject judgeUI;
    bool switchPosition;
    GameObject clone;
    public override bool Init(Transform character)
    {
        switchPosition = false;
        return base.Init(character);
    }

    public override void SetLevel(int level)
    {
        skillRange = 1 + level;
        
    }

    public override bool Filter(Skill sender)
    {
        if (UnitManager.GetInstance().units.Find(u => u.GetComponent<CharacterStatus>()
            && (u.GetComponent<CharacterStatus>().characterIdentity == CharacterStatus.CharacterIdentity.clone || u.GetComponent<CharacterStatus>().characterIdentity == CharacterStatus.CharacterIdentity.advanceClone)
                && u.GetComponent<CharacterStatus>().playerNumber == sender.character.GetComponent<CharacterStatus>().playerNumber
                    && u.GetComponent<CharacterStatus>().roleEName == sender.character.GetComponent<CharacterStatus>().roleEName) != null)
        {
            //DebugLogPanel.GetInstance().Log("已有分身在场！");
            return false;
        }
        return base.Filter(sender);
    }
    
    public override void Effect()
    {
        base.Effect();

        clone = GameObject.Instantiate(character.gameObject);
        RoundManager.GetInstance().Invoke(() => {
            FXManager.GetInstance().SmokeSpawn(character.position, character.rotation, null);
        }, 0.6f);

        render = character.Find("Render").gameObject;
        RoundManager.GetInstance().Invoke(() => { render.SetActive(false); }, 0.8f);
        animator.speed = 0f;

        RoundManager.GetInstance().Invoke(() => {
            FXManager.GetInstance().SmokeSpawn(focus, character.rotation, null);
            FXManager.GetInstance().SmokeSpawn(character.position, character.rotation, null);
            animator.speed = 1f;
        }, 1.4f);
        RoundManager.GetInstance().Invoke(() => {
            if (switchPosition)
            {
                clone.transform.position = character.position;
                character.position = focus;
            }
            else
            {
                clone.transform.position = focus;
            }

            SetIdentity(clone);

            UnitManager.GetInstance().AddUnit(clone.GetComponent<Unit>());
            clone.GetComponent<Unit>().Buffs.Add(new DirectionBuff());
            clone.GetComponent<Animator>().Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            clone.GetComponent<Animator>().SetInteger("Skill", 0);
            character.GetComponent<Unit>().UnitEnded += SetCloneEnd;
            render.SetActive(true);
        }, 1.6f);


        //var clone = GameObject.Instantiate(character.gameObject);
        //if (switchPosition)
        //{
        //    clone.transform.position = character.position;
        //    character.position = focus;
        //}
        //else
        //{
        //    clone.transform.position = focus;
        //}
        //clone.GetComponent<CharacterStatus>().characterIdentity = CharacterStatus.CharacterIdentity.clone;

        //UnitManager.GetInstance().AddUnit(clone.GetComponent<Unit>());
        //clone.GetComponent<Unit>().Buffs.Add(new DirectionBuff());
        //clone.GetComponent<Animator>().Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        //clone.GetComponent<Animator>().SetInteger("Skill", 0);
        //clone.GetComponent<Unit>().OnUnitEnd();
    }

    void SetCloneEnd(object sender,EventArgs e)
    {
        clone.GetComponent<Unit>().OnUnitEnd();
        character.GetComponent<Unit>().UnitEnded -= SetCloneEnd;
    }

    protected virtual void SetIdentity(GameObject clone)
    {
        clone.GetComponent<CharacterStatus>().SetClone(character);
    }

    protected override void InitSkill()
    {
        base.InitSkill();
    }

    protected void SwitchPosition()
    {
        switchPosition = true;
    }

    protected override void ShowConfirm()
    {
        var go = (GameObject)Resources.Load("Prefabs/UI/Judge");
        judgeUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        judgeUI.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(SwitchPosition);
        judgeUI.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(base.ShowConfirm);
        judgeUI.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(DestroyUI);
        judgeUI.transform.Find("No").GetComponent<Button>().onClick.AddListener(base.ShowConfirm);
        judgeUI.transform.Find("No").GetComponent<Button>().onClick.AddListener(DestroyUI);
        judgeUI.transform.Find("Text").GetComponent<Text>().text = "改变本体和分身的位置吗？";
    }

    protected void DestroyUI()
    {
        if(judgeUI)
            UnityEngine.Object.Destroy(judgeUI);
    }

    public override bool Check()
    {
        var list = Detect.DetectObject(focus);
        foreach (var p in list)
        {
            if (p.GetComponent<Unit>())
            {
                return false;
            }
        }
        return true;
    }

    protected override void ResetSelf()
    {
        DestroyUI();
        base.ResetSelf();
    }

    public override void Reset()
    {
        DestroyUI();
        base.Reset();
    }
}
