using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[RequireComponent(typeof(Animator))]
public class AnimatorManager : MonoBehaviour
{
    private Animator _animator;

    [Tooltip("If true, allows the sprite to be flipped horizontally")]
    [SerializeField]
    private bool _canFlipX = true;

    [Tooltip("If true, allows the sprite to be flipped vertically")]
    [SerializeField]
    private bool _canFlipY = true;
    SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        if (_animator == null)
        {
            Debug.LogError("NO ANIMATOR IS ATTACHED");
            return;
        }

    }
    private void Update()
    {
        if(_rb != null)
        {
            Vector2 velocity = _rb.velocity;
            _animator.SetFloat("XSpeed", velocity.x);
            _animator.SetFloat("YSpeed", velocity.y);
        }
      
    }
    private void RotateSpriteXLeft()
    {
        if (!_canFlipX) return;
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = true;
        }
    }
    private void RotateSpriteXRight()
    {
        if (!_canFlipX) return;
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = false;
        }
    }

    public void RotateSpriteY()
    {
        if (!_canFlipY) return;
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipY = !_spriteRenderer.flipY;
        }
    }

    public void Destroy()
    {
        if (_animator == null || !_animator.enabled) return;
        _animator.SetTrigger("Die");
        StartCoroutine(DestroyAfterAnimation());
        this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }
    private IEnumerator DestroyAfterAnimation()
    {
        // Wait for the "Die" animation to finish
        float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);
        // Destroy the object after the animation completes
        Destroy(gameObject);
    }
    public void Damage()
    {
        if (_animator == null || !_animator.enabled) return;
        _animator.SetTrigger("Damage");
    }

    public void ChangeState()
    {
        if (_animator == null || !_animator.enabled) return;
        _animator.SetTrigger("ChangeState");
        _animator.SetBool("Left", false);
        _animator.SetBool("Right", false);
        _animator.SetBool("Up", false);
        _animator.SetBool("Down", false);
        _animator.SetBool("Follow", false);
    }

    public void SpawnEvent()
    {
        if (_animator == null || !_animator.enabled) return;
        _animator.SetTrigger("Spawn");
    }

    private void LeftDirection()
    {
        if (_animator == null || !_animator.enabled) return;
        _animator.SetBool("Left", true);
        _animator.SetBool("Right", false);
    }

    private void RightDirection()
    {
        if (_animator == null || !_animator.enabled) return;
        _animator.SetBool("Left", false);
        _animator.SetBool("Right", true);
    }

    public void DownDirection()
    {
        if (_animator == null || !_animator.enabled) return;
        _animator.SetBool("Up", false);
        _animator.SetBool("Down", true);
    }

    public void UpDirection()
    {
        if (_animator == null || !_animator.enabled) return;
        _animator.SetBool("Down", false);
        _animator.SetBool("Up", true);
    }

    public void XLeftChangeAndFlip()
    {
        if (_animator == null || !_animator.enabled) return;
       
       RotateSpriteXLeft();
       LeftDirection();
    }

    public void XRightChangeAndFlip()
    {
        if (_animator == null || !_animator.enabled) return;
        RotateSpriteXRight();
        RightDirection();
    }
    public void ChangeSpeedY(float speed)
    {
        if (_animator == null || !_animator.enabled) return;
        _animator.SetFloat("YSpeed", speed);
    }

    public void ChangeSpeedRotation(float speed)
    {
        if (_animator == null || !_animator.enabled) return;
        _animator.SetFloat("RotationSpeed", speed);
    }

    public void Follow()
    {
        if (_animator == null || !_animator.enabled) return;
        _animator.SetBool("Follow", true);
    }

}
