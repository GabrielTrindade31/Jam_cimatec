using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform powerCore;
    public string     playerTag = "Player";
    public float      followSpeed = 5f;

    Transform      player;
    PlayerStats    stats;

    void LateUpdate()
    {
        if (player == null)
        {
            var go = GameObject.FindWithTag(playerTag);
            if (go != null)
            {
                player = go.transform;
                stats  = go.GetComponent<PlayerStats>();
            }
        }

        Vector3 target;
        if (player != null && stats != null && !stats.isDead)
            target = player.position;
        else
            target = powerCore.position;

        target.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);
    }
}
