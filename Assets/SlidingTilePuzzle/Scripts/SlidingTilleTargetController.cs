using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;


namespace Enneas.SlidingTile
{
    // THE FOLLOWING SCRIPT HANDLES THE SLIDING TILE GAME TARGETS
    // IN V.01 THE GAME HAS TWO SLIDING TILE GAMES: POSTAGE, COINS, WW2 AND SURGICAL
    // IT TAKES AS AN INPUT THE NAME OF THE GAMEOBJECT THE SCRIPT IS ATTACHED TO
    // TO AND PERFORMS THE CORRESPONDING ACTIONS ACCORDING TO THAT
    public class SlidingTilleTargetController : MonoBehaviour, IRestart
    {
        private GameObject achievementManagerObject;

        /// <summary>
        /// Requires: Achievement Manager script
        /// </summary>
        //private AchievementManager achievementManager;

        //[SerializeField] private GameObject budgeBeltManagerObject;
        //private achievementsManaget budgeBeltManager;


        [SerializeField] private GameObject StartPanel;

        /// <summary>
        /// Requires: GameHunting script
        /// </summary>
        //private GameHunting gameHunting;
        private bool sceneActivated;
        private bool Completed = false;


        /// <summary>
        /// Requires: Fading Component script
        /// </summary>
        //private FadingComponent fm;
        /////////////////////////////////////
        ///
        private string achievementName;
        [SerializeField] int budgeIndex;

        /// <summary>
        /// Requires: BagdeCaseHandler  script
        /// </summary>
        //private BagdeCaseHandler bagdeCaseHandler;
        //private int budgeIndex = -1;
        private string sceneName;
        private int prefabIndex = 2;


        /// <summary>
        /// Requires: ScoreSystem  script
        /// </summary>
        //Score
        //private ScoreSystem scoreSystem;
        public int score;
        public int movesToComplete;

        [SerializeField] private string achievementKeyVitro = "interiorDesigner";
        [SerializeField] private string achievementKeyPlakaki = "assistantCaretaker";

        [SerializeField] private string sceneVitro = "sliding _sliding_tile_vitro";
        [SerializeField] private string scenePlakaki = "sliding _sliding_tile";

        [SerializeField] private GameObject endScreen;

        private bool gameWon;

        [SerializeField] private GameObject autosolvePane;


        [SerializeField] private GameObject solvedPuzzle;

        [SerializeField] private GameObject solveMeBtn;

        private bool autoWin;


        void Awake()
        {
            sceneActivated = false;
        }

        void Start()
        {

            ////Initialize the Achievement Manager;
            //achievementManagerObject = GameObject.Find("Achievement Manager");
            ////budgeBeltManagerObject = GameObject.Find("BadgeCase");

            //bagdeCaseHandler = FindObjectOfType<BagdeCaseHandler>();
            ////Initialize the Score System;
            //scoreSystem = FindObjectOfType<ScoreSystem>();

            //gameHunting = FindObjectOfType<GameHunting>().GetComponent<GameHunting>();
            //fm = GetComponent<FadingComponent>();


            Completed = false;

            string gameObjectName = transform.root.gameObject.name;

            Debug.Log(gameObjectName);


            //achievementManager = achievementManagerObject.GetComponent<AchievementManager>();

            if (gameObjectName == "pf_sliding_tile_plakaki" || gameObjectName == "pf_sliding_tile_plakaki(Clone)")
            {
                //achievementName = "achievementSurgicalIntern";
                //budgeIndex = 4;
                prefabIndex = 3;
                sceneName = scenePlakaki;
                achievementName = achievementKeyPlakaki;

                Debug.Log(sceneName);

            }
            else
            {
                prefabIndex = 4;
                sceneName = sceneVitro;
                achievementName = achievementKeyVitro;
                Debug.Log(sceneName);
            }
        }

        private void OnEnable()
        {
            transform.root.GetComponent<CanvasGroup>().alpha = 0f;
            transform.root.GetComponent<CanvasGroup>().DOFade(1f, 1);
            ST_PuzzleDisplay.OnAutoSolve += AutoSolve;
            //TODO: Before push to product, change the scene name from jason -> scene_ar;

            //Scene scene_ar = SceneManager.GetSceneByName("scene_ar");
            Scene scene_ar = SceneManager.GetSceneByName("jason");

            Scene sliding_tile_scene = SceneManager.GetSceneByName(sceneName);

            /*Debug.Log(sliding_tile_scene.name); 
            Debug.Log(scene_ar.name);
            */

            //fm = GetComponent<FadingComponent>();


            if (scene_ar.isLoaded && !sliding_tile_scene.isLoaded)
            {
                //gameHunting.ScanCanvas.SetActive(false);
                sceneActivated = false;
            }
        }

        private void OnDisable()
        {
            ST_PuzzleDisplay.OnAutoSolve -= AutoSolve;
        }

        private void AutoSolve()
        {
            if (autosolvePane != null)
            {
                autosolvePane.SetActive(true);
                autosolvePane.GetComponent<CanvasGroup>().DOFade(1.0f, 1.5f);

            }

        }

        public void EnableSolveMeBtn()
        {
            solveMeBtn.SetActive(true);
        }

        public void SolvePuzzle()
        {
            autosolvePane.SetActive(false);
            solvedPuzzle.SetActive(true);


            GameObject slidingTileGame = GameObject.Find("/SlidingTile_3by3");
            foreach (Transform child in slidingTileGame.transform)
            {
                child.gameObject.SetActive(false);
            }

            StartCoroutine(AutoWin());
        }

        IEnumerator AutoWin()
        {
            //We need to destroy the tiles;


            yield return new WaitForSeconds(2f);

            GameObject slidingTileGame = GameObject.Find("/SlidingTile_3by3");
            ST_PuzzleDisplay puzzleScript = slidingTileGame.GetComponent<ST_PuzzleDisplay>();

            puzzleScript.AutoSolve();

            autoWin = true;



        }


        // Update is called once per frame
        void Update()
        {

            GameObject slidingTileGame = GameObject.Find("/SlidingTile_3by3");
            if (slidingTileGame != null)
            {
                ST_PuzzleDisplay puzzleScript = slidingTileGame.GetComponent<ST_PuzzleDisplay>();

                if (puzzleScript != null)
                {
                    if (puzzleScript.Complete)
                    {

                        Debug.Log("WON");
                        //We get how many moves took us to complete the puzzle;
                        //StartPanel.SetActive(false);
                        score = puzzleScript.movesToComplete;


                        CompleteGame(puzzleScript.Complete);
                    }
                }
            }

            /*if (sceneActivated == false)
            {
                StartCoroutine(SwitchGameScene());
                sceneActivated = true;
            }*/


        }
        public void AddLoadedScene()
        {

            transform.root.GetComponent<CanvasGroup>().DOFade(0f, 1);

            if (sceneActivated == false)
            {
                StartCoroutine(SwitchGameScene());
                sceneActivated = true;
            }
        }

        public void DisableStartPanel()
        {
            StartCoroutine(DisableStartPanelDelay());
        }

        IEnumerator DisableStartPanelDelay()
        {
            yield return new WaitForSeconds(2f);
            StartPanel.SetActive(false);

            ResetGame();
        }

        public void CompleteGame(bool isGameWon)
        {
            // game won
            Completed = true;

            QuitCame(isGameWon);

            gameWon = isGameWon;

            //How many moves it took us to complete the game;
            Debug.Log("Moves to complete: " + score);

            //We call our function to send the calculated score to the score system class;
            CalculateScore();

            Debug.Log("achievementName: " + achievementName);

            //Activate EndScreen
            ActivateEndScreen();
        }

        public void ActivateEndScreen()
        {
            //We need to set the alpha value of sliding gameobject back to 1;
            transform.root.GetComponent<CanvasGroup>().DOFade(1f, 1);
            StartPanel.SetActive(false);
            //Enable EndGameScreen;
            endScreen.SetActive(true);

        }

        //When the user presses continue in the end game screen we show the rewards;
        public void Achievement()
        {
            //bagdeCaseHandler.UnlockBudge(budgeIndex);


            transform.root.GetComponent<CanvasGroup>().DOFade(1f, 1);
            transform.root.gameObject.SetActive(false);
            bool won = gameWon;


            //achievementManager.Unlock(achievementName);

            //budgeBeltManager.UnlockBudge(budgeIndex);

            //gameHunting.FinishCurrentGame(gameHunting.FindPrefabIndex(transform.parent.name), won);

            transform.root.gameObject.SetActive(false);
        }

        //Calculation to transfor the total moves into a 1-4 score;
        //We break the moves to complete into 4 parts, each having 20 moves difference;
        private void CalculateScore()
        {
            if (score < 20)
            {
                score = 4;
            }
            else if (score < 40)
            {
                score = 3;
            }
            else if (score < 60)
            {
                score = 2;
            }
            else
                score = 1;

            //scoreSystem.AddMinigameScore(score);
        }

        IEnumerator SwitchGameScene()
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("Current Scene name: " + sceneName);


            if (sceneName == scenePlakaki)
            {
                SceneManager.LoadScene(scenePlakaki, LoadSceneMode.Additive);

            }
            else
            {
                Debug.Log("Scene name: " + sceneName);

                SceneManager.LoadScene(sceneVitro, LoadSceneMode.Additive);

            }
        }

        public void QuitButton()
        {
            //Destroy the scene;
            Scene targetScene = SceneManager.GetSceneByName(sceneName);


            //gameHunting.FinishCurrentGame(gameHunting.FindPrefabIndex(this.name), false);

            SceneManager.UnloadSceneAsync(targetScene);

            //Destroy the gameobject;
            var gameobjectToDestroy = GameObject.Find("pf_sliding_tile_plakaki");


            if (gameobjectToDestroy != null)
            {
                gameobjectToDestroy.SetActive(false);

            }
            else
            {
                gameobjectToDestroy = GameObject.Find("pf_sliding_tile_vitro");



                gameobjectToDestroy.SetActive(false);


            }

        }




        public void PauseGame()
        {
            StartCoroutine(PauseGameCo());
        }

        IEnumerator PauseGameCo()
        {
            yield return new WaitForSeconds(1.0f);
            Time.timeScale = 0.0f;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1.0f;
        }

        public void QuitCame(bool isGameWon)
        {
            StartCoroutine(QuitGameCo(isGameWon));
        }

        IEnumerator QuitGameCo(bool isGameWon)
        {
            Scene targetScene = SceneManager.GetSceneByName(sceneName);


            //StartCoroutine(ResetAlpha());

            if (Completed == false)
            {
                //gameHunting.FinishCurrentGame(gameHunting.FindPrefabIndex(transform.root.name), isGameWon);

                Debug.Log("isGameWon: " + isGameWon);
                yield return new WaitForSeconds(2f);
            }
            if (targetScene.isLoaded)
            {
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
                while (!asyncUnload.isDone)
                {
                    yield return null;
                }
            }
            //We deactivate the sliding prefab;

        }

        private IEnumerator ResetAlpha()
        {
            //We find our sliding prefab;
            GameObject sliding = GameObject.Find("pf_sliding_tile_plakaki");
            GameObject sliding2 = GameObject.Find("pf_sliding_tile_vitro");

            //Deactivate it;
            sliding.SetActive(false);
            sliding2.SetActive(false);

            //Now since fadingcomponent.OnDisable gets called automatically, we need to reset its alpha
            //because otherwise it goes to 1 (rather than 0);
            yield return new WaitForSeconds(2.0f);


            //sliding.GetComponent<FadingComponent>().Disable();
            //sliding2.GetComponent<FadingComponent>().Disable();
        }

        public void ResetGame()
        {
            score = 0;
            movesToComplete = 0;
            gameWon = false;
            Completed = false;
            autoWin = false;
            sceneActivated = false;


            if (StartPanel != null)
            {
                StartPanel.SetActive(true); // Reactivate start panel if necessary
            }

            if (endScreen != null)
            {
                endScreen.SetActive(false); // Deactivate end screen
            }

            if (solvedPuzzle != null)
            {
                solvedPuzzle.SetActive(false); // Deactivate solved puzzle panel
            }

            if (solveMeBtn != null)
            {
                solveMeBtn.SetActive(false); // Deactivate solve me button
            }

            if (autosolvePane != null)
            {
                autosolvePane.SetActive(false); // Deactivate autosolve panel
            }

        }
    }

}
