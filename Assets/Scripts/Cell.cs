using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    //state of the cell 1 - on, 0 - off
    public int state = 0;

    //Position of the cell
    public int[] position;

    //The rule that was last applied to the cell
    public int currentRl = 0;

    //Reference to the cell's SpriteRenderer
    public SpriteRenderer sprRend;

    //Age of the tile
    public float age = 0;

    //Used for colour mode settings
    public int mode = 0;

    //Those colours are used when the rule colour mode is on
    Color[] col = new Color[]
    {
        Color.blue,
        Color.green,
        Color.black,
        Color.white,
        Color.red
    };


    // Creates the cell
    // pos - Position of the cell
    // newSpr - Sprite to be used by the cell
    public void CreateCell(int[] pos, Sprite newSpr)
    {
        state = 0;
        position = pos;
        // Set game objects position
        this.transform.position = new Vector2(position[0], position[1]);
        // Add the box collider used for the mouse raycast
        this.gameObject.AddComponent<BoxCollider>();
        // Add the SprieRenderer to the object for it to be seen in the scene
        this.gameObject.AddComponent<SpriteRenderer>();
        sprRend = this.gameObject.GetComponent<SpriteRenderer>();
        // Set cell's sprite
        SetSprite(newSpr);
    }

    // Set the cells state and the rule that was used on it
    // newStatus - New status of the cell
    // r - Rule that was applied ot the cell
    public void SetState(int newStatus, int r)
    {
        state = newStatus;
        currentRl = r;
        ChangeColour();
    }

    // Set the sprite to be used by the cell  and initial colour
    void SetSprite(Sprite newSpr)
    {
        sprRend.sprite = newSpr;
        sprRend.color = new Color(state, state, state);
    }

    // Switch the current state to the opposite
    public void SwitchState()
    {
        state = state == 0 ? 1 : 0;
        sprRend.color = new Color(state, state, state);
    }

    // Colouring modes
    // 0 - Colour of the cell is based on the rule applied
    // 1 - Colour based on age is aplied to cells when they are alive
    // 2 - Colour based on age is aplied to cells when they are alive, the intensity of the colour is bigger for the cells that are alive so they are more visible
    // 3 - Colour is based on the current state 1 - white, 0 - black
    public void ChangeColour()
    {
        switch (mode)

        {
            case 0:
                sprRend.color = new Color(state, state, state);
                if (currentRl == 4)
                {
                    sprRend.color = new Color(255, 0, 0);
                }
                break;
            case 1:
                if (state == 1)
                {
                    age += 0.03f;
                }
                sprRend.color = new Color(0, age, age);
                break;
            //case 2:
            //    if (state == 1)
            //    {
            //        age += 0.03f;
            //        sprRend.color = new Color(0, age + state, age + state);
            //    }
            //    else
            //    {
            //        //age -= 0.01f;
            //        sprRend.color = new Color(0, age, age);
            //    }
            //    break;
            case 3:
                sprRend.color = col[currentRl];
                break;
            case 4:
                sprRend.color = new Color(255, 0, 0);
                break;
        }
    }

    // Sets the value to be usef by the ChangeColour()
    // m - Mode to be applied
    public void SetMode(int m)
    {
        mode = m;
        ChangeColour();
    }
}
