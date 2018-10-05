﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageFreeze : Targetable
{
    protected Hero _hero;

    protected override void Start()
    {
        base.Start();

        _hero = GetComponent<Hero>();
        raisedSortingOrder = 9;
        targetType = TARGET_TYPE.ENEMY;
        temporary = true;
        range = 3f;
        lineColor = new Color(.3f, .38f, 72f, 1f);

        GameManager.instance.SpotlightHero(_hero, true);
    }

    protected override void Update()
    {
        base.Update();

        if (!dragging)
            HighlightHero();
    }

    protected override void Action()
    {
        GameManager.instance.SpotlightHero(_hero, false);
        GameManager.instance.currentState = GAME_STATE.PLANNING;

        if (!_targetObj.GetComponent<StatusEnemyFrozen>())
            _targetObj.AddComponent<StatusEnemyFrozen>();

        base.Action();
    }

    protected void HighlightHero()
    {
        TargetManager.instance.size = 1.25f;
        TargetManager.instance.color = new Color(lineColor.r, lineColor.g, lineColor.b, .75f);
        TargetManager.instance.sortingOrder = -1;
        TargetManager.instance.DrawMarker(_transform.position);
    }
}