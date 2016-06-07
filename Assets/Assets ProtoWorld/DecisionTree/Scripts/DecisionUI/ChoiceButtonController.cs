using UnityEngine;
using System.Collections;

public class ChoiceButtonController : MonoBehaviour 
{
    private int choiceNumber = 0;
    private DecisionUIController decisionUI;

    void Awake()
    {
        decisionUI = FindObjectOfType<DecisionUIController>();
    }

    public void SetChoiceNumber(int i)
    {
        this.choiceNumber = i;
    }

    public void SelectThisChoice()
    {
        decisionUI.AnswerQuestion(choiceNumber);
    }
}
