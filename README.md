# Carrom2D

## Overview

Carrom2D is a single-player game developed in Unity, optimized for portrait screen orientation, and scalable on all devices. The game replicates the popular tabletop game of Carrom, where the player aims to pocket all the pucks using a striker.

## Game Elements
The following game elements are incorporated in the Carrom2D:
- **Striker**: The player can drag the striker behind to charge the power of the shot and release it in the desired direction.
- **Pucks**: There are 8 black pucks, 8 white pucks, and 1 red queen. The objective is to pocket all the pucks using the striker.

## Game Mechanics
The game mechanics of Carrom2D include:

- **Power**: The player can charge the power of the shot by dragging the striker behind and release it in the desired direction to hit the pucks.
- **Physics**: The game follows general physics rules, ensuring realistic puck movement and collisions.
- **Boundaries**: The striker and pucks bounce off the boundaries of the board to maintain the gameplay within the designated area. 
- **Scoring**: The game incorporates functional pockets on the game board. When a puck is pocketed, the score in the UI panel increases by 1. Pocketing the red queen adds 2 points to the score.
- **Timer**: The game includes a 2-minute timer. When the timer runs out, a "Game Over" banner is displayed, indicating the end of the game.



## AI Functionality

Carrom Game features a single-player mode with AI functionality. The AI bot is capable of taking simple shots to challenge the player's skills.

## Game Demo

https://github.com/Acrolyte/Carrom2D/assets/59145196/470a8b67-4a48-4952-ba9e-cd3d8506195b

## Game Logic

- PlayManager acts as a center of execution of game logic. It contains references of almost every other gameobject and initializes the gameplay.
```
    public Turn currentTurn = Turn.User;
    public GameObject PlayerStriker, CompStriker, scoreManager;

    [SerializeField]
    private GameObject pucksGO;

    public Rigidbody2D[] pucks;

    public static PlayManager instance;
```
- The PlayerStriker and CompStriker takes input from user and AI consecutively. 
- Player logic is defined in PositioningScript.cs which takes Canvas based slider for positioning the striker and Raycast for calculating the board striker angle and power level.
```
if (Input.GetMouseButton(0))
        {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);

            if (hit.collider)
            {
                //Debug.Log($"Raycast with {hit.collider.name}");
                if (hit.collider.name == "PlayerStriker")
                { 
                    StrikerAim = true;
                    helperCollider.SetActive(true);
                }
                if (StrikerAim)
                {
                    StrikerBg.LookAt(hit.point);
                }

                float ScaleValue = Vector2.Distance(transform.position, hit.point);
                ScaleValue = ScaleValue > 1.5f ? 1.5f : ScaleValue;
                StrikerBg.localScale = new Vector3(ScaleValue, ScaleValue, ScaleValue);
                
                //Debug.Log(hit.transform.name);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            rb.AddForce(new Vector3(circles.position.x - transform.position.x, circles.position.y - transform.position.y, 0) * 2000);
            
            sliderHider.SetActive(true);
            StrikerBg.localScale = Vector3.zero;
            if (rb.velocity.magnitude < 0.2f && StrikerAim)
            {
                helperCollider.SetActive(false);
                gameObject.name = "Unavailable";
                Invoke(nameof(GiveTurn), 3);
            }
            StrikerAim = false;
        }
```
- AI's input is defined in ComputerAim.cs and is generated for a random hitpoint.
```
Vector2 hitpoint = new Vector2(Random.Range(minX,maxX), Random.Range(minY,maxY));
```
- Along with that initial position of computer striker, angle and forces is calculated and delayed using Invoke() method for better user-experience.
- PlayManager also contains the logic for switching the turns according state of the board.
```
public void SwitchTurn(bool flag)
    {
        //Debug.Log("Switched");
        bool steadyState = true;
        float total_velocity = 0;
        while (steadyState)
        {
            foreach(var p in pucks)
            {
                total_velocity += p.velocity.magnitude;
            }
            if (total_velocity < 0.2f) 
                steadyState = false;
        }

        PlayerStriker.SetActive(flag);
        CompStriker.SetActive(!flag);
        if (currentTurn == Turn.User) currentTurn = Turn.Comp;
        else currentTurn = Turn.User;
    }
```
- When a puck reaches with a low velocity near holes, the DestroyPucks.cs deals with the removal of that puck gameobject and from PlayManager reference.
```
if ((collision.gameObject.name != "PlayerStriker" && collision.gameObject.name != "CompStriker")  && vel < 0.2f)
        {
            int value = collision.gameObject.GetComponent<Puck_Value>().Value;
            
            PlayManager.instance.pucks = PlayManager.instance.pucks.Where(val => val.name != collision.gameObject.name).ToArray();

            GetComponent<AudioSource>().Play();

            Destroy(collision.gameObject);
            PlayManager.instance.scoreManager.GetComponent<ScoreManager>().AddScore(value);
        }
```
- Also ScoreManager.cs deals with the calculation of score based on 'Turn' enum.
```
public void AddScore(int value)
    {
        Turn check = PlayManager.instance.currentTurn;
        if (check == Turn.User)
        {
            playerScore += value;

            playerScoreText.text = "You : " + playerScore;
        }
        else
        {
            compScore += value;

            compScoreText.text = "Comp : " + compScore;
        }

    }
```
- Lastly, TimeHandler.cs calculates the timer value and shows GameOver scene when value reaches zero.
```
        if (stoptime > 0)
        {
            stoptime -= Time.deltaTime;
        }
        else
        {
            SceneManager.LoadScene("GameOver");
        }
```
