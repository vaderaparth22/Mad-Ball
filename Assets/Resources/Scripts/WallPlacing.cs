using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlacing : MonoBehaviour
{
    public float xPosMultiplier = 0.5f;
    public float yPositionMultiplier = 0.5f;

    public Transform leftWall;
    public Transform rightWall;
    public Transform topWall;
    public Transform bottomWall;

    public float GetWallsHeight => topWall.transform.position.y;
    public float GetWallsWidth => rightWall.transform.position.x;

    public void InitializeWallPositions()
    {
        Vector2 screenToWorld = GetScreenToWorldWidthHeight();

        leftWall.transform.position = new Vector2(-screenToWorld.x + xPosMultiplier, 0);
        rightWall.transform.position = new Vector2(screenToWorld.x - xPosMultiplier, 0);

        //Vector2 viewPortPos = Camera.main.WorldToViewportPoint
        topWall.transform.position = new Vector2(0, screenToWorld.y - yPositionMultiplier);
        bottomWall.transform.position = new Vector2(0, -screenToWorld.y + yPositionMultiplier);
    }

    public Vector2 GetScreenToWorldWidthHeight()
    {
        Vector2 screenToWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        return screenToWorld;
    }
}
