using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector2[] directions = new Vector2[]
    {
        Vector2.right, Vector2.up, Vector2.left, Vector2.down, Vector2.zero
    };

    [Header("Inscribed: Enemy")]
    public float maxHealth = 1;

    [Header("Dynamic: Enemy")]
    public float health;

    protected Animator anim;
    protected Rigidbody2D rigid;
    protected SpriteRenderer sRend;

    protected virtual void Awake()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sRend = GetComponent<SpriteRenderer>();
    }
}

