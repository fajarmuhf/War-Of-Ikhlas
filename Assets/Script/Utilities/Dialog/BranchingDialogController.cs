using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class BranchingDialogController : MonoBehaviour
{
    [SerializeField] private GameObject dialogPrefab;
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] public TextAssetValue dialogValue;
    [SerializeField] private Story myStory;
    [SerializeField] private GameObject dialogHolder;
    [SerializeField] private GameObject choiceHolder;
    [SerializeField] private ScrollRect dialogScroll;
    [SerializeField] public GameObject npcInteract;
    [SerializeField] public GameObject playerInteract;
    // Start is called before the first frame update
    private void OnEnable()
    {
        DeleteOldDialog();
        SetStory();
        RefreshView();
    }

    public void SetStory()
    {
        if (dialogValue.value)
        {
            myStory = new Story(dialogValue.value.text);
        }
        else
        {
            Debug.Log("Something wrong with dialog asset");
        }
        myStory.variablesState["name"] = "Fajar";
        myStory.variablesState["introQuest"] = npcInteract.GetComponent<NPCController>().introQuest;
        /* Set Varible Name */
        myStory.BindExternalFunction("takeQuestNow", (string nama, string reward) => {
            //_audioController.Play(name);
            playerInteract.GetComponent<Player>().takeQuest(Int32.Parse(nama));
        });
        myStory.BindExternalFunction("giveRewardQuest", (string name, string reward) => {
            //_audioController.Play(name);
            Debug.Log("Quest berhasil diselesaikan level " + name);
        });

        int jumlahOwnerQuest = 0;
        if(playerInteract.GetComponent<Player>().playerQuest.myQuest.Count > 0)
        {
            for(int i = 0; i < playerInteract.GetComponent<Player>().playerQuest.myQuest.Count; i++)
            {
                if(playerInteract.GetComponent<Player>().playerQuest.myQuest[i].ownerQuest == npcInteract)
                {
                    jumlahOwnerQuest++;
                }
            }
        }
        if(jumlahOwnerQuest > 0)
        {
            myStory.variablesState["takeQuest"] = 1;
        }
        else
        {
            myStory.variablesState["takeQuest"] = 0;
        }
    }

    public void RefreshView()
    {
        while (myStory.canContinue)
        {
            MakeNewDialog(myStory.Continue());
        }

        if (myStory.currentChoices.Count > 0)
        {
            MakeNewChoice();
            StartCoroutine(ScrollCo());
        }
        else
        {
            gameObject.SetActive(false);
            Player.localPlayer.doneChat();
        }
    }

    IEnumerator ScrollCo()
    {
        yield return null;
        dialogScroll.verticalNormalizedPosition = 0f;
    }

    public void MakeNewDialog(string newDialog)
    {
        DialogObject newDialogObject = Instantiate(dialogPrefab, dialogHolder.transform).GetComponent<DialogObject>();
        newDialogObject.Setup(newDialog);
    }

    void MakeNewChoice()
    {
        for (int i = 0; i < choiceHolder.transform.childCount; i++)
        {
            Destroy(choiceHolder.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < myStory.currentChoices.Count; i++)
        {
            MakeNewResponse(myStory.currentChoices[i].text,i);
        }
    }

    public void MakeNewResponse(string newDialog, int choiceValue)
    {
        npcInteract.GetComponent<NPCController>().introQuest = 0;
        ResponeObject newResponeObject =
            Instantiate(choicePrefab, choiceHolder.transform).GetComponent<ResponeObject>();
        newResponeObject.Setup(newDialog, choiceValue);
        Button responseButton = newResponeObject.GetComponent<Button>();
        if (responseButton)
        {
            responseButton.onClick.AddListener(delegate { ChooseChoice(choiceValue);});
        }
    }

    public void ChooseChoice(int choice)
    {
        myStory.ChooseChoiceIndex(choice);
        RefreshView();
    }

    public void DeleteOldDialog()
    {
        for (int i = 0; i < dialogHolder.transform.childCount; i++)
        {
            Destroy(dialogHolder.transform.GetChild(i).gameObject);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
