using DynamicBox.EventManagement;
using SOG.Meals;
using SOG.UI.PauseAndLoose;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOG.MealGenerator
{
  public class MealGenerator : MonoBehaviour
  {
    #region Variables

    [SerializeField] private GameObject[] Meals;

    [SerializeField] private float frequency;

    private List<GameObject> mealList;

    private List<GameObject> lostMealsList;

    private IEnumerator instantiateMeal;

    private int count = 10;

    private int id = 0;

    #endregion

    private IEnumerator InstantiateMeal()
    {
      for (int i = 0; i < Meals.Length; i++)
      {
        int score = Meals[i].gameObject.GetComponent<NormalMeals>().GetScoreForEat();
        if (score == 0)
        {
          for (int j = 0; j < 10; j++)
          {
            GameObject instantiateObject = Instantiate(Meals[i], new Vector3(0, 0, 0), Quaternion.identity, transform);
            instantiateObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            instantiateObject.gameObject.GetComponent<NormalMeals>().id = id;
            instantiateObject.SetActive(false);
            mealList.Add(instantiateObject);
            yield return new WaitForSeconds(0.05f);
            id++;
          }
        }
        else
        {
          for (int j = 0; j < (count / score); j++)
          {
            GameObject instantiateObject = Instantiate(Meals[i], new Vector3(0, 0, 0), Quaternion.identity, transform);
            instantiateObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            instantiateObject.gameObject.GetComponent<NormalMeals>().id = id;
            instantiateObject.SetActive(false);
            mealList.Add(instantiateObject);
            yield return new WaitForSeconds(0.05f);
            id++;
          }
        }
      }
    }

    private void GenerateMeal()
    {
      if (GameManager.Instance.gameState == GameStateEnum.PLAY)
      {
        if (mealList.Count == 0)
        {
          return;
        }
        GameObject meal = mealList[Random.Range(0, mealList.Count)];
        meal.transform.position = new Vector3(Random.Range(-10f, 10f), transform.position.y, 0);
        meal.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        meal.SetActive(true);
        for (int i = 0; i < mealList.Count; i++)
        {
          if(meal.gameObject.GetComponent<NormalMeals>().id == mealList[i].gameObject.GetComponent<NormalMeals>().id) mealList.RemoveAt(i);
        }
        lostMealsList.Add(meal);
        CheckLostMealList();
      }
    }

    private void CheckLostMealList()
    {
      for (int i = 0; i < lostMealsList.Count; i++)
      {
        if (lostMealsList[i].gameObject.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Static)
        {
          mealList.Add(lostMealsList[i]);
          lostMealsList.RemoveAt(i);
        }
      }
    }

    private void RestartState()
    {
      for (int i = 0; i < lostMealsList.Count; i++)
      {
        lostMealsList[i].SetActive(false);
        lostMealsList[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        mealList.Add(lostMealsList[i]);
        lostMealsList.RemoveAt(i);
      }
    }

    private void Start()
    {
      instantiateMeal = InstantiateMeal();
      mealList = new List<GameObject>();
      lostMealsList = new List<GameObject>();

      StartCoroutine(instantiateMeal);
      InvokeRepeating("GenerateMeal", 0.5f, frequency);

    }

    #region Unity Events
    private void OnEnable()
    {
      EventManager.Instance.AddListener<RestartButtonPressedEvent>(RestartButtonPressedEventHadnler);
      EventManager.Instance.AddListener<MainMenuButtonPressedEvent>(MainMenuButtonPressedEventHandler);
    }

    private void OnDisable()
    {
      EventManager.Instance.RemoveListener<RestartButtonPressedEvent>(RestartButtonPressedEventHadnler);
      EventManager.Instance.RemoveListener<MainMenuButtonPressedEvent>(MainMenuButtonPressedEventHandler);

    }

    #endregion

    #region Handlers

    private void RestartButtonPressedEventHadnler(RestartButtonPressedEvent eventDetails)
    {
      RestartState();
      RestartState();
    }

    private void MainMenuButtonPressedEventHandler(MainMenuButtonPressedEvent eventDetails)
    {
      RestartState();
    }

    #endregion



  }
}

