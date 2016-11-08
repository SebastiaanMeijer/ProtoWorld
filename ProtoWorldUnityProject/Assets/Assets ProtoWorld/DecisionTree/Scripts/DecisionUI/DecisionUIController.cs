/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * DECISION TREE MODULE
 * DecisionUIController.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DecisionUIController : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private ScenarioController scenarioController;
    private RectTransform questionBox;
    private List<GameObject> choiceButtons;
    private TimeController timeController;
    private CameraControl cameraControl;
    private LandmarkController[] listOfLandmarkPoints;

    private Stack<SimQuestion> previousQuestions;
    private SimQuestion currentQuestion;

    public float zoomFactor = 4.0f;
    public GameObject backToStartQuestionCanvas;
    public Button pauseButton;

    void Awake()
    {
        timeController = FindObjectOfType<TimeController>();
        scenarioController = FindObjectOfType<ScenarioController>();
        questionBox = this.GetComponent<RectTransform>();
        choiceButtons = new List<GameObject>();
        listOfLandmarkPoints = FindObjectsOfType<LandmarkController>();
        cameraControl = FindObjectOfType<CameraControl>();
        previousQuestions = new Stack<SimQuestion>();
    }

    public void ShowQuestionCanvas(SimQuestion question)
    {
        // Log the question
        Debug.Log("Display question " + question.id + ": " + question.text);
        log.Info("Display question " + question.id + ": " + question.text);

        // Fill the question canvas info
        this.transform.FindChild("QuestionId").GetComponent<Text>().text = "Question " + question.id;
        this.transform.FindChild("QuestionText").GetComponent<Text>().text = question.text;

        // Set the height of the box depending on the number of choices
        questionBox.sizeDelta = new Vector2(questionBox.sizeDelta.x, 125 + 75 * question.choices.Length);

        // Disable all the choice buttons
        foreach (GameObject G in choiceButtons)
            G.SetActive(false);

        // Set the choice buttons
        Transform choiceTemplate = this.transform.FindChild("ChoiceTemplate");
        for (int i = 0; i < question.choices.Length; i++)
        {
            // Instantiate a new choice button
            if (i >= choiceButtons.Count)
                choiceButtons.Add(Instantiate(choiceTemplate.gameObject) as GameObject);

            // Get the script component of the choice button            
            RectTransform buttonBox = choiceButtons[i].GetComponent<RectTransform>();
            Text buttonText = choiceButtons[i].transform.FindChild("ChoiceText").GetComponent<Text>();
            choiceButtons[i].GetComponent<ChoiceButtonController>().SetChoiceNumber(i);

            // Position the button according to its parent object
            choiceButtons[i].transform.SetParent(this.transform, false);
            buttonBox.localPosition = new Vector3(0, -150 - 75 * i, 0);

            // Change the button text
            buttonText.text = question.choices[i].text;

            //Set the button active
            choiceButtons[i].SetActive(true);
        }

        // Show the canvas
        this.GetComponent<FadingElementUI>().fadeInCanvas();

        // Pause the game until the question is answer
        if (timeController != null)
        {
            timeController.RequestPauseGame();
            timeController.BlockPauseButton(true);
        }

        // If there is a focus id in the question, move the camera to the focus point
        if (question.landmark != null)
        {
            // Find the point
            foreach (LandmarkController L in listOfLandmarkPoints)
            {
                if (L.landmarkId == question.landmark)
                {
                    cameraControl.BlockPlayerControls(5);
                    cameraControl.FocusOnHotPoint(L.transform, zoomFactor);
                    break;
                }
            }
        }

        // Update current question
        currentQuestion = question;
    }

    public void AnswerQuestion(int choice)
    {
        // Log the question
        Debug.Log("Answer to question " + currentQuestion.id + ": " + choice + " -> " + currentQuestion.choices[choice].text);
        log.Info("Answer to question " + currentQuestion.id + ": " + choice + " -> " + currentQuestion.choices[choice].text);

        // Hide the canvas
        this.GetComponent<FadingElementUI>().fadeOutCanvas();

        // Answer the question
        if (currentQuestion != null)
        {
            currentQuestion.ExecuteChoice(choice);
            //scenarioController.AnswerQuestion(currentQuestion.id, choice);

            // Add the question to the previous questions
            previousQuestions.Push(currentQuestion);
            currentQuestion = null;
        }

        // Unblock the camera
        cameraControl.UnblockPlayerControls();

        // Resume the game
        if (timeController != null)
        {
            timeController.RequestResumeGame();
            timeController.BlockPauseButton(false);
        }
    }

    public void BackToPreviousEvent(bool backToBeginning = false)
    {
        // Log the question
        Debug.Log("Back to Previous Event clicked");
        log.Info("Back to Previous Event clicked");

        if (previousQuestions.Count == 0 || backToBeginning)
        {
            // Flush the Stack
            previousQuestions.Clear();

            this.GetComponent<FadingElementUI>().fadeOutCanvas();
            cameraControl.FocusOnOverviewPoint();
            timeController.gameTime = 1f;

            // Unblock the camera
            cameraControl.UnblockPlayerControls();

            // Resume the game
            if (timeController != null)
                timeController.RequestResumeGame();

            // Load the first scenario
            scenarioController.LoadFirstScenario();
        }
        else if (previousQuestions.Count > 0)
        {
            // Go to previous question on the stack
            SimQuestion q = previousQuestions.Pop();
            timeController.gameTime = q.time;
            ShowQuestionCanvas(q);
        }
    }

    public void ShowBackToStartQuestion()
    {
        if (backToStartQuestionCanvas != null)
        {
            backToStartQuestionCanvas.GetComponent<FadingElementUI>().fadeInCanvas();
            backToStartQuestionCanvas.transform.FindChild("YesButton").GetComponent<Button>().interactable = true;
            backToStartQuestionCanvas.transform.FindChild("NoButton").GetComponent<Button>().interactable = true;
        }
    }

    public void BackToBeginningTrigger(bool isGoingBack)
    {
        backToStartQuestionCanvas.GetComponent<FadingElementUI>().fadeOutCanvas();
        backToStartQuestionCanvas.transform.FindChild("YesButton").GetComponent<Button>().interactable = false;
        backToStartQuestionCanvas.transform.FindChild("NoButton").GetComponent<Button>().interactable = false;

        if (isGoingBack)
        {
            BackToPreviousEvent(true);
        }
    }
}
