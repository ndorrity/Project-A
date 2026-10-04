using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointNumber = 1;
    public bool isFinishLine = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.name.Contains("RaceCar"))
        {
            if (isFinishLine)
            {
                RaceTrackManager.Instance.FinishLineCrossed();
            }
            else
            {
                RaceTrackManager.Instance.CheckpointHit(checkpointNumber);
            }
        }
    }
}
