using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;
//using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public struct Step
    {
        public Vector3 position;
        public Quaternion rotation;
    }
    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> collection;
    public LinkedList<Step> dataEntries = new LinkedList<Step>();
    public float move;
    public AudioClip touchwall;
    public AudioClip pass;
    public AudioClip move_s;
    public AudioClip failed;
    AudioSource aud;
    private CircleCollider2D circleCollider;
    public float Leftlimit;
    //public float Bottomlimit;
    public float Rightlimit;
    public float Uplimit;
    public float win_ponit;
    public int return_limit;
    public bool gameisgonging = false;
    public GameObject passObject;
    public GameObject FailedText;
    GameObject returntext;
    GameObject three_ranking;
    public GameRuler GameRuler;
    // Start is called before the first frame update
    void Start()
    {
        RecordStep();
        aud = GetComponent<AudioSource>();
        circleCollider = GetComponent<CircleCollider2D>();
        this.returntext = GameObject.Find("return");
        client = new MongoClient("mongodb+srv://neois:25MvIn0Ua0suFonf@cluster0.emd9aib.mongodb.net/?retryWrites=true&w=majority");
        database = client.GetDatabase("test");
        collection = database.GetCollection<BsonDocument>("maze");
          
    }

    // Update is called once per frame
    void Update()
    {
        if (gameisgonging)
        {
            if (return_limit < 1 && CheckCollision(transform.right) && CheckCollision(transform.forward))
            {
                gameisgonging = false;
                FailedText.SetActive(true);
                this.aud.PlayOneShot(this.failed);
            }
            if (transform.position.y < win_ponit)
            {
                gameisgonging = false;
                this.aud.PlayOneShot(this.pass);
                passObject.SetActive(true); // play "pass" 
                Animation anim = passObject.GetComponent<Animation>(); // get Animation of pass  
                if (anim != null)
                {
                    anim.Play(); // Play animation
                }
                //GameRuler.Stoptimer();
                var count = collection.CountDocuments(new BsonDocument());
                this.three_ranking = GameObject.Find("rank");
                Debug.Log("this.three_ranking" + this.three_ranking.GetComponent<TextMeshProUGUI>().text);
                this.three_ranking.GetComponent<TextMeshProUGUI>().text = "1~3位:\n";
                string record_text = GameObject.Find("timer").GetComponent<TextMeshProUGUI>().text;
                int record = int.Parse(record_text.Replace("s", ""));
                var data = new BsonDocument {
                {"ID",count+1 },
                {"Record",record },
                };
                collection.InsertOne(data);
                var sortDefinition = Builders<BsonDocument>.Sort.Ascending("Record");
                var topThreeScores = collection.Find(new BsonDocument()).Sort(sortDefinition).Limit(4).ToList();
                foreach (var doc in topThreeScores)
                {
                    Debug.Log("Number of scores retrieved: " + topThreeScores.Count);
                    if (doc.Contains("Record") && doc.Contains("ID"))
                    {
                        Debug.Log("Contains");
                        int scoreValue = doc["Record"].AsInt32;
                        long scoreid = doc["ID"].AsInt64;
                        var addrank = "ID:" + scoreid.ToString() + "  Record:" + scoreValue.ToString() + "s" + "\n";
                        this.three_ranking.GetComponent<TextMeshProUGUI>().text += addrank;
                    };
                    
                }
                this.three_ranking.GetComponent<TextMeshProUGUI>().text += "your result: \n ID:" + (count + 1).ToString() + "  Record:" + record.ToString() + "s" + "\n";
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (CheckCollision(transform.forward))
                {
                    this.aud.PlayOneShot(this.touchwall);
                }
                else
                {
                    //Debug.Log("move: " + move);
                    transform.Translate(0.0f, move, 0.0f);
                    RecordStep();
                    this.aud.PlayOneShot(this.move_s);
                }

            }
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                if (CheckCollision(transform.right))
                {
                    this.aud.PlayOneShot(this.touchwall);
                }
                else
                {
                    transform.Rotate(0, 0, -90);
                    //UnityEngine.Debug.Log("Rotate ");
                    transform.Translate(0.0f, move, 0.0f);
                    RecordStep();
                    this.aud.PlayOneShot(this.move_s);
                }
            }
            if (Input.GetKeyUp(KeyCode.Space) && return_limit > 0)
            {
                if (dataEntries.Count > 1)
                {
                    dataEntries.RemoveLast();
                    transform.position = dataEntries.Last.Value.position;
                    transform.rotation = dataEntries.Last.Value.rotation;
                    return_limit -= 1;
                    this.returntext.GetComponent<TextMeshProUGUI>().text = "戻せる回数:" + return_limit.ToString();
                    //Debug.Log("Last position: " + dataEntries.Last.Value.position);
                }
            }
        };
        
    }

    private bool CheckCollision(Vector3 direction)
    {
        int obstacleLayerMask = LayerMask.GetMask("Default");
        float radius = circleCollider.radius;
        float dis = move + radius;
        //Debug.Log("radius :" + radius );
        //Debug.Log("player position:" + transform.position);
        Vector3 startPoint = transform.position;
        Vector3 endPoint = startPoint + transform.up * dis ;

         if (direction == transform.right)
        {
            endPoint = startPoint + transform.right * dis;
        }
        //Debug.Log("startPoint :" + startPoint);
        //Debug.Log("endPoint :" + endPoint);
        if (endPoint.x > Leftlimit || endPoint.x < Rightlimit || endPoint.y > Uplimit)
        {
            return true;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(endPoint, radius, obstacleLayerMask);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)  // Exclude self from collision check
            {
                Debug.Log("Collision ahead:" + collider.gameObject.name);
                return true;
            }
        }

        return false;
    }
    private void RecordStep()
    {
        
        Step entry = new Step
        {
            position = transform.position,
            rotation = transform.rotation
        };
        dataEntries.AddLast(entry);
    }
}
