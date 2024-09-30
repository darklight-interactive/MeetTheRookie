using UnityEngine;

public class MTRSceneBounds : MonoBehaviour
{
    public float leftBound = -10f;
    public float rightBound = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector3(leftBound, -5, transform.position.z), new Vector3(leftBound, 5, transform.position.z));
        Gizmos.DrawLine(new Vector3(rightBound, -5, transform.position.z), new Vector3(rightBound, 5, transform.position.z));
    }
}
