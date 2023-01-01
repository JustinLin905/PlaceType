using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomSentence : MonoBehaviour
{
    public TextAsset wordsList;
    string[] allWords;


    // Function which generates a list of random words in the English language of length n
    public string GenerateSentence(int n)
    {
        string[] words = new string[n];
        for (int i = 0; i < n; i++)
        {
            words[i] = RandomWord();
        }

        // Join using spaces, remove last index to remove the final space
        string sentence = string.Join(" ", words);
        sentence = sentence.Remove(sentence.Length - 1);

        return sentence;
    }

    // Function to generate a random word from the English language
    public string RandomWord()
    {
        if (wordsList != null)
        {
            allWords = (wordsList.text.Split('\n'));
            int length = allWords.Length;

            return allWords[Random.Range(0, length)];
        }

        return "";
    }
}
