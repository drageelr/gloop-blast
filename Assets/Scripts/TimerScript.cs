using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;
using System.Collections;

public class TimerScript : MonoBehaviour {

    public int timeinseconds;
	private int minutes;
	private int seconds;
    public int yellowtime;
    public GameObject Timer;
    public GameObject pauseObj;
    public Slime slimescript;
    public bool run;
    public bool running;

    private Text Text;
    private Stopwatch sw;
    

	// Use this for initialization
	void Start () {
        Text = Timer.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        //Text.color = new Color(0, 255, 0);
        //Text.color = new Color(246, 230, 103);
        //Text.color = new Color(255, 0, 0);
        if (run)
        {
            run = false;
            sw = new Stopwatch();
            sw.Start();
            Text.color = new Color(255, 255, 255);
			DisplayTime ();
			//Text.text = timeinseconds + "s";
            running = true;
        }
        if (running)
        {
            if (pauseObj.tag == "Paused")
            {
                running = false;
            }
            else
            {
                if (sw.ElapsedMilliseconds >= 1000)
                {
                    sw.Reset();
                    sw.Start();
                    timeinseconds = timeinseconds - 1;
                    if (timeinseconds == yellowtime)
                    {
                        Text.color = new Color(255, 225, 0);
                        slimescript.halfscore = true;
                    }
                    else if (timeinseconds == 0)
                    {
                        Text.color = new Color(255, 0, 0);
                        running = false;
                        sw.Stop();
                    }
					DisplayTime();
					//Text.text = timeinseconds + "s";
                }
            }

        }

        }
	void DisplayTime(){
		minutes =  (timeinseconds / 60);
		seconds =  (timeinseconds -  minutes * 60);

		if (minutes < 10) {
			Text.text = "0" + minutes;
		} 
		else{
			Text.text = "" + minutes;
		}

		if (seconds < 10) {
			Text.text = Text.text + ":0" + seconds;
		} else {
			Text.text = Text.text + ":" + seconds;
		}
	} 
}

