using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(InRoom) )]
public class Dray : MonoBehaviour, IFacingMover {
    static public IFacingMover IFM;
    public enum eMode { idle, move, attack, roomTrans }

    [Header("Inscribed")]
    public float speed = 5;
    public float            attackDuration = 0.25f;
    public float            attackDelay = 0.5f;
    public float            roomTransDelay = 0.5f;

    [Header("Dynamic")]
    public int dirHeld = -1; 
    public int              facing = 1;    
    public eMode            mode = eMode.idle;  
   
    private float           timeAtkDone = 0;  
    private float           timeAtkNext = 0;
    private float           roomTransDone = 0;   
    private Vector2         roomTransPos;

    private Rigidbody2D rigid;
    private Animator        anim;
    private InRoom          inRm;

    private Vector2[] directions = new Vector2[] {
        Vector2.right, Vector2.up, Vector2.left, Vector2.down 
    };

    private KeyCode[] keys = new KeyCode[] {       
        KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow,       
        KeyCode.D,          KeyCode.W,       KeyCode.A,         KeyCode.S 
    };

    void Awake() {
        IFM = this;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inRm = GetComponent<InRoom>();
    }

    void Update() {
        if ( mode == eMode.roomTrans ) {             
            rigid.velocity = Vector3.zero;             
            anim.speed = 0;            
            posInRoom = roomTransPos;           
            if (Time.time < roomTransDone) return;           
            mode = eMode.idle;         
        }

        if (mode == eMode.attack && Time.time >= timeAtkDone) { 
            mode = eMode.idle;
        }

        if (mode == eMode.idle || mode == eMode.move) {
            dirHeld = -1;
            for (int i = 0; i < keys.Length; i++) {
                if (Input.GetKey(keys[i])) dirHeld = i % 4;
            }

            if (dirHeld == -1) { 
                mode = eMode.idle;
            } else {
                facing = dirHeld; 
                mode = eMode.move;
            }

            if (Input.GetKeyDown(KeyCode.Z) && Time.time >= timeAtkNext) { 
                mode = eMode.attack;
                timeAtkDone = Time.time + attackDuration;
                timeAtkNext = Time.time + attackDelay;
            }
        }
        Vector2 vel = Vector2.zero;
        switch (mode) {
            case eMode.attack: 
                anim.Play("Dray_Attack_" + facing);
                anim.speed = 0;
                break;

            case eMode.idle: 
                anim.Play("Dray_Walk_" + facing);
                anim.speed = 0;
                break;

            case eMode.move: 
                vel = directions[dirHeld];
                anim.Play("Dray_Walk_" + facing);
                anim.speed = 1;
                break;
        }
        rigid.velocity = vel * speed;
    }
    
    void LateUpdate() {
        Vector2 gridPosIR = GetGridPosInRoom(0.25f);

        int doorNum;
        for (doorNum = 0; doorNum < 4; doorNum++) {                           
            if (gridPosIR == InRoom.DOORS[doorNum]) {
                break;                                                     
            }
        }

        if (doorNum > 3 || doorNum != facing) return;                      

        Vector2 rm = roomNum;
        switch (doorNum) { 
            case 0:
                rm.x += 1;
                break;
            case 1:
                rm.y += 1;
                break;
            case 2:
                rm.x -= 1;
                break;
            case 3:
                rm.y -= 1;
                break;
        }

        if (0 <= rm.x && rm.x <= InRoom.MAX_RM_X) {                          
            if (0 <= rm.y && rm.y <= InRoom.MAX_RM_Y) {
                roomNum = rm;
                roomTransPos = InRoom.DOORS[(doorNum + 2) % 4];             
                posInRoom = roomTransPos;
                mode = eMode.roomTrans;                                      
                roomTransDone = Time.time + roomTransDelay;
            }
        }
    }

    //———————————————————— Implementation of IFacingMover ————————————————————
        public int GetFacing() { return facing; }   
        public float GetSpeed() { return speed; }

        public bool moving { get { return (mode == eMode.move); } }

        public float gridMult { get { return inRm.gridMult; } }

        public bool isInRoom { get { return inRm.isInRoom; } }

        public Vector2 roomNum
        {
            get { return inRm.roomNum; }
            set { inRm.roomNum = value; }
        }

        public Vector2 posInRoom
        {
            get { return inRm.posInRoom; }
            set { inRm.posInRoom = value; }
        }

        public Vector2 GetGridPosInRoom(float mult = -1) {
            return inRm.GetGridPosInRoom(mult);
        }
}