using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyAgent : MonoBehaviour {
    private Animator animator;
    private Rigidbody2D body;
    private DashTrail dashTrail;
    private SpriteRenderer spriteRenderer;

    private float speedLimit;
    private bool moving = false;
    private bool dashing = false;

    private Vector2 previousDirection = new Vector2(0, 0);
    private Vector2 zeroVector = new Vector2(0, 0);



    /**************************
     *          Init          *
     **************************/
    private void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        dashTrail = GetComponent<DashTrail>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }



    /**************************
     *        Update          *
     **************************/
    private void UpdateSortingLayer()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt((transform.position.y + -0.1f) / 0.05f);
    }

    private void Update()
    {
        UpdateSortingLayer();
    }

    private void FixedUpdate()
    {

    }
}
