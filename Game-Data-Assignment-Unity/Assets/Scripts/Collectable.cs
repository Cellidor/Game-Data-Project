using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour {

    //Items to be serialized.

    public float lerpTime = 0;
    public bool invertMove;
    public bool collected = false;
    public float animationTime;
    public Vector3 currentPosition;

    public Animator objAnimator;
    private AudioSource aSource;
    private SpriteRenderer dataRender;
    private BoxCollider2D dataCol;
    public AudioClip collectSound;

    private Vector3 startPos;
    private Vector3 endPos;


    public void Awake()
    {
        objAnimator = gameObject.GetComponent<Animator>();
        aSource = gameObject.GetComponent<AudioSource>();
        dataRender = gameObject.GetComponent<SpriteRenderer>();
        dataCol = gameObject.GetComponent<BoxCollider2D>();
    }
    public void Start()
    {
        startPos = gameObject.transform.position;
        endPos = new Vector3(startPos.x, startPos.y + 0.2f, startPos.z);
    }


    void Update()
    {
        if (collected == true)
        {
            dataRender.enabled = false;
            dataCol.enabled = false;
        }
        else
        {
            transform.position = Vector3.Lerp(startPos, endPos, lerpTime);
            animationTime = objAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            if (!invertMove)
            {
                if (lerpTime >= 1)
                {
                    invertMove = !invertMove;
                }
                lerpTime += Time.deltaTime;
            }
            else
            {
                if (lerpTime <= 0)
                {
                    invertMove = !invertMove;
                }
                lerpTime -= Time.deltaTime;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collected = true;
            aSource.PlayOneShot(collectSound);
        }
    }
}
