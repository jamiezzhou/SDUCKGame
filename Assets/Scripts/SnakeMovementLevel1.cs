using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;


public class SnakeMovementLevel1 : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Transform> TailParts = new List<Transform>();


    public float minDistance = 5f;

    public float speed = 5;
    public float rotationSpeed = 180;

    public GameObject TailPrefab;
    public GameObject ApplePrefab;

    public AudioSource crunch;
    public AudioSource gameOver;
    public AudioSource gameWin;

    private float dis;
    private Transform curTailPart;
    private Transform prevTailPart;

    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    private int count;

    private string fileName;
    private float prevTime;

    void Start()
    {
        //Time.timeScale = 1;


        prevTime = 0;

        AddApple();

        count = 0;

        SetCountText();

        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);

        Directory.CreateDirectory(Application.streamingAssetsPath + "/Position_Logs/");
        CreateTextFile();

    }

    // Update is called once per frame
    void Update()
    {
        Move();

        //recording the position according to time
        if (Time.time - prevTime >= 0.5)
        {
            Debug.Log(Time.time + ", " + prevTime);
            float temp = Time.time;
            File.AppendAllText(fileName, Mathf.Round(temp * 100f) / 100f + "     "
            + Mathf.Round(TailParts[0].transform.position.x * 100f) / 100f
            + ", " + Mathf.Round(TailParts[0].transform.position.z * 100f) / 100f + "\n");
            prevTime = temp;
        }
    }

    public void CreateTextFile() {
        fileName = Application.streamingAssetsPath + "/Position_Logs/" + "PositionLevel1" + ".txt";

        if (!File.Exists(fileName)) {
            File.WriteAllText(fileName, "SDUCK LEVEL 1 POSITION LOG" + "\n");
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        if (count > 9) {
            EndGame("win");
        }
    }


    public void Move()
    {

        //determining the next position according to steering
        float curspeed = speed;

        if (Input.GetKey(KeyCode.Space)) {
            curspeed *= 2;
        }

        TailParts[0].Translate(TailParts[0].forward * curspeed * Time.smoothDeltaTime, Space.World);

        if (Input.GetAxis("Horizontal") != 0) {
            TailParts[0].Rotate(Vector3.up * rotationSpeed *  Time.deltaTime * Input.GetAxis("Horizontal"));
        }

        //determining the position for each tail node
        for (int i = 1; i < TailParts.Count; i++) {
            curTailPart = TailParts[i];
            prevTailPart = TailParts[i - 1];

            dis = Vector3.Distance(prevTailPart.position, curTailPart.position);

            Vector3 newpos = prevTailPart.position;

            newpos.y = TailParts[0].position.y;

            float T = Time.deltaTime * dis / minDistance * curspeed;

            if (T > 0.5f) {
                T = 0.5f;
            }

            curTailPart.position = Vector3.Slerp(curTailPart.position, newpos, T);
            curTailPart.rotation = Quaternion.Slerp(curTailPart.rotation, prevTailPart.rotation, T);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "apple") {
            Destroy(other.gameObject);
            crunch.Play();
            AddBodyPart();

            AddApple();
        }

        if (other.tag == "log" || other.tag == "walls") {
            EndGame("lose");
        }

        if (TailParts.Count > 2 && other.tag == "tail")
        {
            EndGame("lose");
        }
    }

    private void EndGame(string winOrLose)
    {
        speed = 0;
        rotationSpeed = 0;
        if (winOrLose.Equals("lose"))
        {
            loseTextObject.SetActive(true);
            gameOver.Play();
            StartCoroutine(WaitQuitMenu(3f));
        }
        if (winOrLose.Equals("win"))
        {
            winTextObject.SetActive(true);
            gameWin.Play();
            StartCoroutine(WaitNextMenu(3f));

        }
    }

    public IEnumerator WaitNextMenu(float t)
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public IEnumerator WaitQuitMenu(float t)
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene(3);
    }

    private void AddApple() {
        var randPos = new Vector3(Random.Range(-9f, 9f), 0.0f, Random.Range(-9f, 9f));  
        Instantiate(ApplePrefab, randPos, Quaternion.identity);
        ++count;
        SetCountText();
    }

    public void AddBodyPart() {
        Transform newPart = (Instantiate(TailPrefab, TailParts[TailParts.Count - 1].position, TailParts[TailParts.Count - 1].rotation) as GameObject).transform;
        newPart.SetParent(transform);
        TailParts.Add(newPart);
    }
}
