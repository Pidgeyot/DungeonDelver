using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMove : MonoBehaviour {
    private IFacingMover mover;

    void Awake() {
        mover = GetComponent<IFacingMover>();
        if (mover == null) {
            Debug.LogError("Cannot find IFacingMover on " + gameObject.name);
        }
    }

    void FixedUpdate() {                                                   
        if (!mover.moving) return; 
        int facing = mover.GetFacing();

        Vector2 posIR = mover.posInRoom;
        Vector2 posIRGrid = mover.GetGridPosInRoom();
  
        float delta = 0; 
        if (facing == 0 || facing == 2) {
            delta = posIRGrid.y - posIR.y;
        } else {
            delta = posIRGrid.x - posIR.x;
        }
        if (delta == 0) return;

        float gridAlignSpeed = mover.GetSpeed() * Time.fixedDeltaTime; 
        gridAlignSpeed = Mathf.Min(gridAlignSpeed, Mathf.Abs(delta));
        if (delta < 0) gridAlignSpeed = -gridAlignSpeed;

        if (facing == 0 || facing == 2) { 
            posIR.y += gridAlignSpeed;
        } else {
            posIR.x += gridAlignSpeed;
        }

        mover.posInRoom = posIR; 
    }

}
