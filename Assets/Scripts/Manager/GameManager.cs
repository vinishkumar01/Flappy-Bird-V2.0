using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//This script handles the game state and the score system
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameStates gamestates;

    [Header("UI")]
    [SerializeField] private GameObject _menuScreen;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _gameLoadScreen;
    [SerializeField] private GameObject _gameplayScreen;

    [SerializeField] private Image _dashImage;

    [Header("Score Text")]
    [SerializeField] private int _score;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [Header("Game Over Panel")]
    private int _panelScore;
    private int _panelBestScore;

    [SerializeField] private TextMeshProUGUI _panelScoreText;
    [SerializeField] private TextMeshProUGUI _panelBestScoreText;

    [SerializeField] private GameObject _bronzeMedal;
    [SerializeField] private GameObject _silverMedal;
    [SerializeField] private GameObject _GoldMedal;

    [SerializeField] private GameObject _newBestImage;

    //Required flags for the resetting
    [SerializeField] private GameObject _flappy;
    [SerializeField] private PipesAndBirdsSpawnSystem _pipesAndBirdsSpawnSystem;
    private InfiniteBackGroundLayer[] _infiniteBackGroundLayer;
    private bool _gameOverInitialized;

    // Flappy Dead
    public bool isFlappyDead = false;

    private void OnEnable()
    {
        _panelBestScore = PlayerPrefs.GetInt("BestScore", 0);
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //Get All the Background Layers
        _infiniteBackGroundLayer = FindObjectsOfType<InfiniteBackGroundLayer>();
    }

    private void StateControls()
    {
        switch(gamestates)
        {
            case GameStates.GameLoadScreen:

                _gameLoadScreen.SetActive(true);

                _flappy.SetActive(false); 
                _menuScreen.SetActive(false);
                _gameOverScreen.SetActive(false);
                break;

            case GameStates.GameIntro:

                _menuScreen.SetActive(true);

                _flappy.SetActive(true);
                _gameOverScreen.SetActive(false);
                _gameLoadScreen.SetActive(false);

                break;

            case GameStates.GameStarts:
                //Debug.Log("Game State has changed to gameStarts, this message is from Gamemanager");
                _menuScreen.SetActive(false);
                _gameplayScreen.SetActive(true);
                break ;

            case GameStates.GameOver:
                //Flappy dies show the current score and also have to show the HighScore
                _menuScreen.SetActive(false);
                _gameOverScreen.SetActive(true);
                _gameplayScreen.SetActive(false);

                if (!_gameOverInitialized)
                {
                    GameOver();
                    _gameOverInitialized = true;
                }
                
                break ;
        }
    }

    private void Update()
    {
        //
        ResetScore();

        StateControls();

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteKey("BestScore");
            PlayerPrefs.Save();

            Debug.Log("BestScore is cleared");
        }
    }

    public void HandleScore(int Score)
    {
        if (gamestates == GameStates.GameStarts)
        {
            _score += Score;
            _scoreText.text = _score.ToString();
        }
    }

    public void HandleDashImage(float fill)
    {
        _dashImage.fillAmount = fill;
    }

    private void GameOver()
    {
        if(gamestates == GameStates.GameOver)
        {
            //Store the score to the panel score value
            _panelScore = _score;

            int previousBestScore = _panelBestScore;

            Debug.Log($"Current Score : {_panelScore}");
            Debug.Log($"Best Score : {_panelBestScore}");

            AssignMedal(previousBestScore);

            // check if the panel score is greater than the best score, if yes then store the panel score as the best score
            if (_panelScore > _panelBestScore)
            {
                //Also enable the New image UI
                _newBestImage.SetActive(true);

                _panelBestScore = _panelScore;


                PlayerPrefs.SetInt("BestScore", _panelBestScore);
                PlayerPrefs.Save();
            }
            else
            {
                _newBestImage.SetActive(false);
            }

            //Here we will update the texts and the images of the gameOver panel(UI)
            _panelScoreText.text = _panelScore.ToString();
            _panelBestScoreText.text = _panelBestScore.ToString();
        }
    }

    private void AssignMedal(int previousBestScore)
    {
        _bronzeMedal.SetActive(false);
        _silverMedal.SetActive(false);
        _GoldMedal.SetActive(false);

        if(previousBestScore == 0)
        {
            _bronzeMedal.SetActive(false);
            _silverMedal.SetActive(false);
            _GoldMedal.SetActive(false);
            return;
        }

        //Get the ratio
        float ratio = (float)_panelScore / previousBestScore;

        if(ratio > 1f)
        {
            _GoldMedal.SetActive(true);
        }
        else if(ratio >= 0.5f)
        {
            _silverMedal.SetActive(true);
        }
        else if(ratio >= 0.2f)
        {
            _bronzeMedal.SetActive(true);
        }
    }

    public void OnPlayeButtonPressed()
    {
        gamestates = GameStates.GameIntro;
    }

    public void OnRetryButtonPressed()
    {
        _pipesAndBirdsSpawnSystem.ResetSpawner();
       
        //Reset BackgroundLayers
        foreach(var layer in _infiniteBackGroundLayer)
        {
            layer.ResetBackGround();
        }

        _flappy.GetComponent<Flappy>().ResetFlappiesPositionWhenPressedRetry();

        _gameOverInitialized = false;

        gamestates = GameStates.GameIntro;
    }

    public void OnQuitButtonPressed()
    {
        //Quit Application

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void ResetScore()
    {
        if(gamestates == GameStates.GameIntro)
        {
            //Reset Score to 0
            if(_score > 0)
            {
                _score = 0;
                _scoreText.text = _score.ToString();
            }
        }
    }
}
