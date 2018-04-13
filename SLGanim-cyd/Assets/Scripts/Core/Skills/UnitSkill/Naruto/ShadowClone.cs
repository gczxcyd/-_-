﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowClone : AdvancedClone
{
    protected override void SetIdentity(GameObject clone)
    {
        base.SetIdentity(clone);
        clone.GetComponent<CharacterStatus>().identity = "影分身";
    }
}
