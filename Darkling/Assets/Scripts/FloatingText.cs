using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {

    private Text text;

    public float moveSpeed;

    //private Vector2[] moveDirs;
    private Vector2 myMoveDir;

    private bool canMove = false;

    private void Start()
    {
        /*
         * // Random movement
        moveDirs = new Vector2[]
        {
            transform.up,
            (transform.up + transform.right),
            (transform.up + -transform.right)
        };

        myMoveDir = moveDirs[Random.Range(0, moveDirs.Length)];
        */

        // Upwards movement
        myMoveDir = transform.up;

    }

    private void Update()
    {
        if (canMove)
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + myMoveDir, moveSpeed * Time.deltaTime);
    }

    public void SetFloatingText(string textString, int fontSize, Color textColor)
    {
        text = GetComponentInChildren<Text>();
        text.fontSize = fontSize;
        text.color = textColor;
        text.text = textString;
        canMove = true;
    }



}
