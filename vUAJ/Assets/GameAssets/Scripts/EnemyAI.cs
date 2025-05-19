using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 1f; 
    public LayerMask ground;
    public LayerMask wall;

    private Rigidbody2D _rigidbody; 
    public Collider2D triggerCollider;
        
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.Instance.GameIsPaused) return;
        _rigidbody.linearVelocity = new Vector2(moveSpeed, _rigidbody.linearVelocity.y);
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.GameIsPaused) return;
        if (!triggerCollider.IsTouchingLayers(ground) || triggerCollider.IsTouchingLayers(wall))
        {
            Flip();
        }
    }
        
    private void Flip()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        moveSpeed *= -1;
    }
}
