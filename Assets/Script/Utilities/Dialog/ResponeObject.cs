using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResponeObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myText;

    [SerializeField] private int choiceValue;

    public void Setup(string newDialog,int myChoice)
    {
        myText.text = newDialog;
        choiceValue = myChoice;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
