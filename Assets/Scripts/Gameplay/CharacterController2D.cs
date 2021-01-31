using System;
using System.Collections;
using System.Collections.Generic;
using RPGM.Gameplay;
using UnityEngine;
using UnityEngine.U2D;

namespace RPGM.Gameplay
{
    /// <summary>
    /// A simple controller for animating a 4 directional sprite using Physics.
    /// </summary>
    public class CharacterController2D : MonoBehaviour
    {
        public float speed = 1;
        public float acceleration = 2;
        public Vector3 nextMoveCommand;
        public Animator animator;
        public bool flipX = false;

        new Rigidbody2D rigidbody2D;
        SpriteRenderer spriteRenderer;
        PixelPerfectCamera pixelPerfectCamera;

        public enum State
        {
            Idle, Moving, Attack
        }

        public enum Direction
        {
            North, East, West, South
        }

        public State state = State.Idle;
        public Direction dir = Direction.North;
        public bool attackBool = false;
        Vector3 start, end;
        Vector2 currentVelocity;
        float startTime;
        float distance;
        float velocity;

        const string IDLE_S = "Idle-D";
        const string IDLE_N = "Idle-U";
        const string IDLE_E = "Idle-R";
        const string IDLE_W = "Idle-L";
        const string WALK_N = "Walk-U";
        const string WALK_S = "Walk-D";
        const string WALK_E = "Walk-R";
        const string WALK_W = "Walk-L";
        const string ATTACK_N = "Attack-U";
        const string ATTACK_S = "Attack-D";
        const string ATTACK_E = "Attack-R";
        const string ATTACK_W = "Attack-L";


        void IdleState()
        {
            if (attackBool){
                state = State.Attack;
            }
            else if (nextMoveCommand != Vector3.zero)
            {
                start = transform.position;
                end = start + nextMoveCommand;
                distance = (end - start).magnitude;
                velocity = 0;
                UpdateAnimator(nextMoveCommand);
                nextMoveCommand = Vector3.zero;
                state = State.Moving;
            }
            else
            {
                velocity = 0;
                rigidbody2D.velocity = Vector3.zero;
                state = State.Idle;
            }
        }

        void MoveState()
        {
            if (attackBool)
            {
                state = State.Attack;
            }
            else
            {
                velocity = Mathf.Clamp01(velocity + Time.deltaTime * acceleration);
                rigidbody2D.velocity = Vector2.SmoothDamp(rigidbody2D.velocity, nextMoveCommand * speed, ref currentVelocity, acceleration, speed);
                
                if (nextMoveCommand.y > 0){
                    dir = Direction.North;
                }
                else if (nextMoveCommand.y < 0){
                    dir = Direction.South;
                    }
                else if (nextMoveCommand.x > 0){
                    dir = Direction.East;
                    }
                else if (nextMoveCommand.x < 0){
                    dir = Direction.West;
                    }
                else{
                    dir = dir;
                    state = State.Idle;
                }

                UpdateAnimator(nextMoveCommand);
            }
        }

        void AttackState() 
        {
            rigidbody2D.velocity = Vector2.zero;
            UpdateAnimator(nextMoveCommand);
            StartCoroutine(attackHold());
        }

        IEnumerator attackHold()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            attackBool = false;
            state = State.Idle;   
            UpdateAnimator(nextMoveCommand);

        }

        void UpdateAnimator(Vector3 direction)
        {
            if (animator)
            {
                switch(dir)
                {
                    case Direction.North:
                        if (state == State.Idle)
                        {
                            animator.PlayInFixedTime(IDLE_N);
                        }
                        else if (state == State.Moving)
                        {
                            animator.PlayInFixedTime(WALK_N);
                        }
                        else if (state == State.Attack)
                        {
                            animator.PlayInFixedTime(ATTACK_N);
                        }
                        break;
                    case Direction.South:
                        if (state == State.Idle)
                        {
                            animator.PlayInFixedTime(IDLE_S);
                            break;
                        }
                        else if (state == State.Moving)
                        {
                            animator.PlayInFixedTime(WALK_S);
                        }
                        else if (state == State.Attack)
                        {
                            animator.PlayInFixedTime(ATTACK_S);
                        }
                        break;
                    case Direction.East:
                        if (state == State.Idle)
                        {
                            animator.PlayInFixedTime(IDLE_E);
                        }
                        else if (state == State.Moving)
                        {
                            animator.PlayInFixedTime(WALK_E);
                        }
                        else if (state == State.Attack)
                        {
                            animator.PlayInFixedTime(ATTACK_E);
                        }
                        break;
                    case Direction.West:
                        if (state == State.Idle)
                        {
                            animator.PlayInFixedTime(IDLE_W);
                        }
                        else if (state == State.Moving)
                        {
                            animator.PlayInFixedTime(WALK_W);
                        }
                        else if (state == State.Attack)
                        {
                            animator.PlayInFixedTime(ATTACK_W);
                        }
                        break;

                }
            }
        }
        
        void Update()
        {
            switch (state)
            {
                case State.Idle:
                    IdleState();
                    break;
                case State.Moving:
                    MoveState();
                    break;
                case State.Attack:
                    AttackState();
                    break;
            }
        }

        void LateUpdate()
        {
            if (pixelPerfectCamera != null)
            {
                transform.position = pixelPerfectCamera.RoundToPixel(transform.position);
            }
        }

        void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            pixelPerfectCamera = GameObject.FindObjectOfType<PixelPerfectCamera>();
        }
    }
}