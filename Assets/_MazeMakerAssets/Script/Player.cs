using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    public bool isRunning = false;
    private int totalBrick = 0;
    private int Gold = 0;
    private List<GameObject> bricks = new List<GameObject>();
    [SerializeField] TextMeshProUGUI textMeshPro;

    [SerializeField] Transform player;
    [SerializeField] Transform brickParent;

    [SerializeField] Transform up;
    [SerializeField] Transform down;
    [SerializeField] Transform left;
    [SerializeField] Transform right;
    [SerializeField] Transform centre;

    [SerializeField] LayerMask unBrickLayers;
    [SerializeField] LayerMask brickLayers;
    [SerializeField] LayerMask pushLayers;
    [SerializeField] LayerMask whiteLayers;
    [SerializeField] LayerMask finalLayers;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public enum MoveState
    {
        Up,
        Down,
        Left,
        Right
    }
    


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (CheckRaycast(MoveState.Up) && !isRunning)
            {
                StartCoroutine(MovePlayer(MoveState.Up));
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (CheckRaycast(MoveState.Down) && !isRunning)
            {
                StartCoroutine(MovePlayer(MoveState.Down));
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (CheckRaycast(MoveState.Left) && !isRunning)
            {
                StartCoroutine(MovePlayer(MoveState.Left));
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (CheckRaycast(MoveState.Right) && !isRunning)
            {
                StartCoroutine(MovePlayer(MoveState.Right));
            }
        }

    }

    IEnumerator MovePlayer(MoveState state)
    {
        isRunning = true;
        
        if (CheckRaycast(state))
        {
            RaycastHit hit;
            if (Physics.Raycast(centre.position, Vector3.down, out hit, 5f, brickLayers))
            {
                totalBrick++;
                hit.collider.gameObject.transform.SetParent(brickParent);
                hit.collider.gameObject.GetComponent<Collider>().enabled = false;

                hit.collider.transform.localPosition = new Vector3(0f, 0.25f * (totalBrick - 1), 0f);
                player.localPosition = new Vector3(0f, 0.25f * (totalBrick - 1), 0f);

                bricks.Add(hit.collider.gameObject);
            }

            RaycastHit hitWhite;
            if (Physics.Raycast(centre.position, Vector3.down, out hitWhite, 5f, whiteLayers))
            {
                if (bricks.Count > 0)
                {
                    bricks[totalBrick - 1].transform.SetParent(hitWhite.collider.gameObject.transform);
                    bricks[totalBrick - 1].transform.localPosition = new Vector3(0f, 0f, -0.4f);

                    bricks.Remove(bricks[totalBrick - 1]);
                    hitWhite.collider.gameObject.GetComponent<Collider>().enabled = false;

                    player.localPosition = new Vector3(0f, 0.25f * (totalBrick - 1), 0f);
                    totalBrick--;
                }        
            }

            RaycastHit hitFinish;
            if (Physics.Raycast(centre.position, Vector3.down, out hitWhite, 5f, finalLayers))
            {
                Debug.Log("done");
                player.transform.localPosition = new Vector3(2.05f, -0.49f, -0.73f);
                brickParent.localPosition = new Vector3(2.05f, -0.49f, 1.12f);
            }


            MovePlayerDirection(state);
            yield return new WaitForSeconds(0.05f);
            StartCoroutine(MovePlayer(state));
        }
        else
        {
            isRunning = false;
            if (CheckPush())
            {
                Debug.Log("hit");
                MoveState newMoveState = FindPushDirection(state);
                StartCoroutine (MovePlayer(newMoveState));
            }
        }
    }
    private MoveState FindPushDirection(MoveState currentState)
    {
        if (CheckRaycast(MoveState.Up) && currentState != MoveState.Down)
        {
            return MoveState.Up;
        }
        else if (CheckRaycast(MoveState.Down) && currentState != MoveState.Up)
        {
            return MoveState.Down;
        }
        else if (CheckRaycast(MoveState.Left) && currentState != MoveState.Right)
        {
            return MoveState.Left;
        }
        else if (CheckRaycast(MoveState.Right) && currentState != MoveState.Left)
        {
            return MoveState.Right;
        }
        else return currentState;
    }

    private void MovePlayerDirection(MoveState state)
    {
        switch (state)
        {
            case MoveState.Up:
                transform.position += new Vector3(1f, 0f, 0f);
                break;
            case MoveState.Down:
                transform.position += new Vector3(-1f, 0f, 0f);
                break;
            case MoveState.Left:
                transform.position += new Vector3(0f, 0f, 1f);
                break;
            case MoveState.Right:
                transform.position += new Vector3(0f, 0f, -1f);
                break;
        }
    }

    //Raycast bang Layer

    bool CheckPush()
    {
        RaycastHit hit;
        if (Physics.Raycast(centre.position, Vector3.down, out hit, 5f, pushLayers))
        {
            return true;
        }
        else return false;
    }

    bool CheckRaycast(MoveState state)
    {
        if (state == MoveState.Up)
        {
            return Physics.Raycast(up.position, Vector3.down, 1f, unBrickLayers);
        }
        else if (state == MoveState.Down)
        {
            return Physics.Raycast(down.position, Vector3.down, 1f, unBrickLayers);
        }
        else if (state == MoveState.Left)
        {
            return Physics.Raycast(left.position, Vector3.down, 1f, unBrickLayers);
        }
        else if (state == MoveState.Right)
        {
            return Physics.Raycast(right.position, Vector3.down, 1f, unBrickLayers);
        }
        return false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Diamond")
        {
            Gold++;
            textMeshPro.text = Gold.ToString();
            Destroy(other.gameObject);
        }
    }

    //Raycast bang Tag
    //bool CheckRaycast(Transform dicr)
    //{
    //    RaycastHit hit;
    //    Ray ray = new Ray(dicr.position, Vector3.down);

    //    if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Wall"))
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        return true;
    //    }
    //}
}
