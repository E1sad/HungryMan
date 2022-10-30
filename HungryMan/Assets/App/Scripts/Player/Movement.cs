using DynamicBox.EventManagement;
using SOG.UI.PauseAndLoose;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOG.Player
{
  public class Movement : MonoBehaviour
  {
    [SerializeField] private float movementSpeed;

    [SerializeField] private Rigidbody2D playerRb;


    private void PlayerMovement()
    {
      float InputHorizontal = Input.GetAxis("Horizontal");

      playerRb.velocity = ((Vector2.right * InputHorizontal * movementSpeed) + (Vector2.up * playerRb.velocity.y)) * Time.deltaTime;
      
      if (Input.touchCount > 0)
      {
        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        if (touchPosition.x > 1)
        {
          playerRb.velocity = ((Vector2.right * 1 * movementSpeed) + (Vector2.up * playerRb.velocity.y)) * Time.deltaTime;  
        }
        if (touchPosition.x < -1)
        {
          playerRb.velocity = ((Vector2.right * -1 * movementSpeed) + (Vector2.up * playerRb.velocity.y)) * Time.deltaTime;
        }

        transform.localScale = new Vector3(Mathf.Sign(touchPosition.x), 1, 1);
      
      }

      if (InputHorizontal != 0)
      {
        transform.localScale = new Vector3(Mathf.Sign(InputHorizontal),1,1);
      }

    }

    private void Update()
    {
      if (GameManager.Instance.gameState == GameStateEnum.PLAY)
      {
        PlayerMovement();
      }
    }

    #region Unity Events
    private void OnEnable()
    {
      EventManager.Instance.AddListener<RestartButtonPressedEvent>(RestartButtonPressedEventHadnler);
    }

    private void OnDisable()
    {
      EventManager.Instance.RemoveListener<RestartButtonPressedEvent>(RestartButtonPressedEventHadnler);

    }

    #endregion

    #region Handlers

    private void RestartButtonPressedEventHadnler(RestartButtonPressedEvent eventDetails)
    {
      gameObject.transform.position = new Vector3(0, -4.5f, 0);
    }

    #endregion


  }
}
