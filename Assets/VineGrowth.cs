using UnityEngine;
using System.Collections;

public class VineGrowth : MonoBehaviour {

#region PrivateFields
    [SerializeField]
    int vineLength;
    [SerializeField]
    float createMargin, growthDistance, verticalGrowthMargin;
    [SerializeField]
    Transform vineChunk, lastChunk;
    [SerializeField]
    AnchorGeneration anchorScript;
    [SerializeField]
    float timeToDestroy;
    [SerializeField]
    Vector3 explosionForce;
#endregion

 #region PublicProperties
    public Transform Player {get; set;}
    public PlayerVineGrow playerVine { get; set; }
#endregion

#region UnityFunctions
void Start()
{
        lastChunk = transform.GetChild(0);
        anchorScript = GetComponent<AnchorGeneration>();
        anchorScript.StartLocation = transform.position;
        playerVine.CurrentVines.Add(gameObject);
}

void Update()
{
    if (vineLength>0)
        CheckPlayerPosition();
}
    #endregion

    #region CustomFunctions
    public void StopGrowing()
    {
        vineLength = 0;
        playerVine.GrowingVine = false;
        anchorScript.CreateAnchors();
    }

    public void DestroyVine()
    {
        playerVine.CurrentVines.Remove(gameObject);
        foreach (Transform chunk in transform)
        {
            if (chunk.tag != "Enemy")
            {
                StartCoroutine("FallApart", chunk);
            }
            else
            {
                chunk.GetComponent<AnchorHitDetection>().OtherUsed = true;
                StartCoroutine("FallApart", chunk);
            }
        }
        Invoke("DestroySelf", timeToDestroy * 2f);
    }

    IEnumerator FallApart(Transform chunk)
    {
        chunk.GetComponent<Collider>().isTrigger = true;
        Rigidbody rb;
        if (chunk.transform.tag != "Enemy")
        {
            rb = chunk.gameObject.AddComponent<Rigidbody>();
        }
        else
        {
            rb = chunk.GetComponent<Rigidbody>();
        }

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(new Vector3(explosionForce.x * RandomMagnitude(), explosionForce.y * RandomMagnitude(), explosionForce.z * RandomMagnitude()), ForceMode.Impulse);
        yield return new WaitForSeconds(timeToDestroy + RandomMagnitude());
        if (chunk!= null)
            Destroy(chunk.gameObject);
        yield return null;
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    float RandomMagnitude()
    {
        return Random.Range(-1f, 1f);
    }

    void CheckPlayerPosition()
    {
        if (Vector3.Distance(Player.position, lastChunk.position) > growthDistance)
        {
            Vector3 difference = Player.position - lastChunk.position;
            Vector3 absDif = new Vector3(Mathf.Abs(difference.x), Mathf.Abs(difference.y), Mathf.Abs(difference.z));
            if ((absDif.y > absDif.x) && (absDif.y > absDif.z))
            {
                if (difference.y > verticalGrowthMargin)
                    MakeChunk(0);
                else if (difference.y < 0)
                    StopGrowing();

            }
            else if ((absDif.x > absDif.y) && (absDif.x > absDif.z))
            {
                //farther side to side
                if (difference.x < -growthDistance) //player is to the left
                    MakeChunk(1);
                else if (difference.x > growthDistance) //player is to the right
                    MakeChunk(2);
            }
            else if ((absDif.z > absDif.x) && (absDif.z > absDif.y))
            {
                //farther back or forward
                if (difference.z > growthDistance) //player is to the front
                    MakeChunk(3);
                else if (difference.z < -growthDistance) //player is to the back
                    MakeChunk(4);
            }
        }
    }

    void MakeChunk(int dir)
    {
        Vector3 offset = Vector3.zero;
        switch (dir)
        {
            case 0:
                offset = Vector3.up * createMargin;
                break;
            case 1:
                offset = Vector3.left * createMargin;
                break;
            case 2:
                offset = Vector3.right * createMargin;
                break;
            case 3:
                offset = Vector3.forward * createMargin;
                break;
            case 4:
                offset = Vector3.back * createMargin;
                break;
        }
        Transform newChunk = (Transform)Instantiate(vineChunk, lastChunk.position + offset, Quaternion.identity);
        newChunk.parent = transform;
        lastChunk = newChunk;
        vineLength--;
    }
#endregion
}