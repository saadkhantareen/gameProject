using UnityEngine;

public class MobileMoveButtons : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    public void PressUp()    { MoveInput = new Vector2(MoveInput.x, 1f); }
    public void ReleaseUp()  { if (MoveInput.y > 0) MoveInput = new Vector2(MoveInput.x, 0f); }

    public void PressDown()  { MoveInput = new Vector2(MoveInput.x, -1f); }
    public void ReleaseDown(){ if (MoveInput.y < 0) MoveInput = new Vector2(MoveInput.x, 0f); }

    public void PressLeft()  { MoveInput = new Vector2(-1f, MoveInput.y); }
    public void ReleaseLeft(){ if (MoveInput.x < 0) MoveInput = new Vector2(0f, MoveInput.y); }

    public void PressRight() { MoveInput = new Vector2(1f, MoveInput.y); }
    public void ReleaseRight(){ if (MoveInput.x > 0) MoveInput = new Vector2(0f, MoveInput.y); }
}