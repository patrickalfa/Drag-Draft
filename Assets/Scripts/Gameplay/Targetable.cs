﻿using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum TARGET_TYPE
{
    HERO,
    ENEMY,
    BOARD,
    AREA_HERO,
    AREA_ENEMY,
    NONE
}

public class Targetable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("Sorting order while raised when targeting.")]
    /// <summary>
    /// Sorting order while raised when targeting
    /// </summary>
    public int raisedSortingOrder;
    [Tooltip("Color of the line drawn when targeting.")]
    /// <summary>
    /// Color of the line drawn when targeting
    /// </summary>
    public Color lineColor;
    [Tooltip("Range of the targeting.")]
    /// <summary>
    /// Range of the targeting
    /// </summary>
    public float range;
    [Tooltip("Type of the targeting method of the card")]
    /// <summary>
    /// Type of the targeting method of the card
    /// </summary>
    public TARGET_TYPE targetType;

    protected Transform _transform;
    protected Transform _sprite;
    protected GameObject _shadow;

    protected GameObject _targetObj;
    protected Vector2 _targetPos;
    protected GameObject[] _targetMultiple;

    protected float _zDistanceToCamera;
    protected int _sortingOrder;

    protected bool dragging = false;

    ///////////////////////////////////////////////////////////////////////////////

    #region Interface Implementations

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        _zDistanceToCamera = Mathf.Abs(_transform.position.z - Camera.main.transform.position.z);

        dragging = true;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount > 1)
            return;

        _targetPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                            Input.mousePosition.y, _zDistanceToCamera));
        DragTo(_targetPos);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;

        if (targetType == TARGET_TYPE.HERO || targetType == TARGET_TYPE.ENEMY)
        {
            if (_targetObj != null)
                Action();
        }
        else if (targetType == TARGET_TYPE.AREA_HERO || targetType == TARGET_TYPE.AREA_ENEMY)
        {
            if (_targetMultiple != null)
                Action();
        }
        else
            Action();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        _sprite.GetComponent<SpriteRenderer>().sortingOrder = raisedSortingOrder;
        _shadow.GetComponent<SpriteRenderer>().sortingOrder = raisedSortingOrder - 2;

        _sprite.DOComplete();
        _sprite.DOBlendableScaleBy(Vector3.one * .1f, .1f);
        _sprite.DOLocalMove((Vector3.up + Vector3.right) * .1f, .1f);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        _sprite.GetComponent<SpriteRenderer>().sortingOrder = _sortingOrder;
        _shadow.GetComponent<SpriteRenderer>().sortingOrder = _sortingOrder - 1;

        _sprite.DOComplete();
        _sprite.DOBlendableScaleBy(Vector3.one * -.1f, .1f);
        _sprite.DOLocalMove(Vector3.zero, .1f);
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////

    protected virtual void Start()
    {
        _transform = transform;
        _sprite = _transform.Find("Sprite");
        _sortingOrder = _sprite.GetComponent<SpriteRenderer>().sortingOrder;
        CreateShadow();
    }

    protected virtual void Update()
    {
        if (dragging)
        {
            DrawLine();

            switch (targetType)
            {
                case TARGET_TYPE.HERO:
                    CheckTargetHero(); break;
                case TARGET_TYPE.ENEMY:
                    CheckTargetEnemy(); break;
                case TARGET_TYPE.BOARD:
                    CheckTargetBoard(); break;
                case TARGET_TYPE.AREA_HERO:
                    CheckTargetAreaHero(); break;
                case TARGET_TYPE.AREA_ENEMY:
                    CheckTargetAreaEnemy(); break;
            }
        }
        else
        {
            _targetObj = null;
            _targetPos = Vector2.zero;
            _targetMultiple = null;
        }
    }

    protected virtual void OnDestroy()
    {
        Destroy(_shadow);
    }

    ///////////////////////////////////////////////////////////////////////////////

    protected virtual void DragTo(Vector3 newPos)
    {
        if (Vector3.Distance(_transform.position, newPos) > range)
        {
            Vector3 norm = (newPos - _transform.position).normalized;
            _targetPos = _transform.position + (norm * range);
        }
    }

    protected virtual void CreateShadow()
    {
        _shadow = Instantiate(_sprite.gameObject, _sprite.position, Quaternion.identity, _transform);
        _shadow.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, .5f);
        _shadow.GetComponent<SpriteRenderer>().sortingOrder = _sortingOrder - 1;
        _shadow.name = "Shadow";
    }

    protected virtual void DrawLine()
    {
        LineManager.instance.size = .1f;
        LineManager.instance.delta = .25f;
        LineManager.instance.color = lineColor;
        LineManager.instance.sortingOrder = raisedSortingOrder;
        LineManager.instance.DrawDottedLine(_transform.position, _targetPos);

        TargetManager.instance.size = .25f;
        TargetManager.instance.color = lineColor;
        TargetManager.instance.sortingOrder = raisedSortingOrder;
        TargetManager.instance.shape = TARGET_SHAPE.CIRCLE;
        TargetManager.instance.DrawMarker(_targetPos);
    }

    protected virtual void Action()
    {
        Destroy(this);
    }

    // CHECKS //////////////////////////////

    private void CheckTargetHero()
    {
        Collider2D col = Physics2D.OverlapCircle(_targetPos, .1f, LayerMask.GetMask("Hero"));

        if (col)
        {
            _targetObj = col.gameObject;

            TargetManager.instance.size = 1.25f;
            TargetManager.instance.sortingOrder = -1;
            TargetManager.instance.shape = TARGET_SHAPE.CIRCLE;
            TargetManager.instance.color = new Color(lineColor.r, lineColor.g, lineColor.b, .5f);
            TargetManager.instance.DrawMarker(_targetObj.transform.position);
        }
        else
            _targetObj = null;
    }

    private void CheckTargetEnemy()
    {
        Collider2D col = Physics2D.OverlapCircle(_targetPos, .1f, LayerMask.GetMask("Enemy"));

        if (col)
        {
            _targetObj = col.gameObject;

            TargetManager.instance.size = 1.25f;
            TargetManager.instance.sortingOrder = -1;
            TargetManager.instance.shape = TARGET_SHAPE.CIRCLE;
            TargetManager.instance.color = new Color(lineColor.r, lineColor.g, lineColor.b, .5f);
            TargetManager.instance.DrawMarker(_targetObj.transform.position);
        }
        else
            _targetObj = null;
    }

    private void CheckTargetBoard()
    {
    }

    private void CheckTargetAreaHero()
    {
    }

    private void CheckTargetAreaEnemy()
    {
    }
}