using System.Data.Common;
using _Scripts.Tools;
using UnityEngine;
using Unity.Profiling;

public class BulletInstance : MonoBehaviour
{
    private static readonly ProfilerMarker s_ApplyStateMarker = new ProfilerMarker("BulletSystem.BulletInstance.ApplyState");

    public SpriteRenderer spriteRenderer;
    public bool isActive = false;
    private MaterialPropertyBlock propertyBlock;

    private bool _hasCachedState;
    private Vector2 _lastPosition;
    private float _lastRotation;
    private Color _lastColor;
    private float _lastAlpha;
    private Sprite _lastSprite;
    private Vector2 _lastScale;
    private int _lastSortingOrder;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    public void ApplyState(BulletRuntimeState state)
    {
        using (s_ApplyStateMarker.Auto())
        {
            isActive = true;

            if (!_hasCachedState)
            {
                ApplyAll(state);
                _hasCachedState = true;
                return;
            }

            if (_lastPosition != state.position)
            {
                _lastPosition = state.position;
                transform.position = state.position;
            }

            if (_lastSortingOrder != state.id)
            {
                _lastSortingOrder = state.id;
                spriteRenderer.sortingOrder = state.id;
            }

            if (Mathf.Abs(_lastRotation - state.rotation) > 0.0001f)
            {
                _lastRotation = state.rotation;
                transform.rotation = Quaternion.Euler(0, 0, state.rotation);
            }

            bool materialDirty = _lastColor != state.color || Mathf.Abs(_lastAlpha - state.alpha) > 0.0001f;
            if (materialDirty)
            {
                _lastColor = state.color;
                _lastAlpha = state.alpha;
                propertyBlock.SetColor("_Color", state.color);
                propertyBlock.SetFloat("_Alpha", state.alpha);
                spriteRenderer.SetPropertyBlock(propertyBlock);
            }

            if (_lastSprite != state.sprite)
            {
                _lastSprite = state.sprite;
                spriteRenderer.sprite = state.sprite;
            }

            if (_lastScale != state.scale)
            {
                _lastScale = state.scale;
                transform.localScale = state.scale;
            }
        }
    }

    private void ApplyAll(BulletRuntimeState state)
    {
        _lastPosition = state.position;
        _lastRotation = state.rotation;
        _lastColor = state.color;
        _lastAlpha = state.alpha;
        _lastSprite = state.sprite;
        _lastScale = state.scale;
        _lastSortingOrder = state.id;

        transform.position = state.position;
        spriteRenderer.sortingOrder = state.id;
        transform.rotation = Quaternion.Euler(0, 0, state.rotation);
        propertyBlock.SetColor("_Color", state.color);
        propertyBlock.SetFloat("_Alpha", state.alpha);
        spriteRenderer.SetPropertyBlock(propertyBlock);
        spriteRenderer.sprite = state.sprite;
        transform.localScale = state.scale;
    }

    public void ResetState()
    {
        isActive = false;
        _hasCachedState = false;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        propertyBlock.SetColor("_Color", Color.gray);
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }


}
