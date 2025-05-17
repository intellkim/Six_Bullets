using UnityEngine;

public class AllyPathController : MonoBehaviour
{
    public AllyMovementStep[] steps;         // 이동할 경로
    public float walkSpeed = 2f;             // 걷는 속도
    public AnimationCurve jumpCurve;         // 점프 곡선
    public float jumpDuration = 1f;          // 점프 연출 시간

    private int index = 0;
    private float jumpTimer = 0f;
    private bool isJumping = false;
    private Vector2 jumpStart, jumpEnd;

    void Update()
    {
        if (index >= steps.Length) return;

        var step = steps[index];

        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(jumpTimer / jumpDuration);
            Vector2 pos = Vector2.Lerp(jumpStart, jumpEnd, t);
            pos.y += jumpCurve.Evaluate(t);  // 곡선 적용
            transform.position = pos;

            if (t >= 1f)
            {
                isJumping = false;
                index++;
            }
            return;
        }

        if (step.moveType == MoveType.Walk)
        {
            transform.position = Vector2.MoveTowards(transform.position, step.targetPoint.position, walkSpeed * Time.deltaTime);
            
            // 방향 반전
            if (step.targetPoint.position.x - transform.position.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(step.targetPoint.position.x - transform.position.x), 1, 1);

            if (Vector2.Distance(transform.position, step.targetPoint.position) < 0.05f)
                index++;
        }
        else if (step.moveType == MoveType.Jump)
        {
            jumpStart = transform.position;
            jumpEnd = step.targetPoint.position;
            jumpTimer = 0f;
            isJumping = true;
        }
    }
}
