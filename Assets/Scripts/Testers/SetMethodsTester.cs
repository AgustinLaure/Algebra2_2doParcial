using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMethodsTester : MonoBehaviour
{
    [SerializeField] private int[] sequence1;
    [SerializeField] private int[] sequence2;

    [Header("All")]
    [SerializeField] private int multiple;
    [Header("Any")]
    [SerializeField] private int number;
    [Header("Contains")]
    [SerializeField] private int number2;
    [Header("ElementAt")]
    [SerializeField] private int index;
    [Header("First")]
    [SerializeField] private int firstMultiple;
    [Header("Last")]
    [SerializeField] private int lastMultiple;
    [Header("Count")]
    [SerializeField] private int countMultiple;
    [Header("Single")]
    [SerializeField] private int singleMultiple;
    [Header("SkipWhile")]
    [SerializeField] private int skipWhileMultiple;
    [Header("Where")]
    [SerializeField] private int whereIndexMultiple;
    [SerializeField] private int whereNumberMultiple;

    private void Start()
    {
        string numbers = "[";
        for (int i = 0; i < sequence1.Length; i++)
        {
            numbers += sequence1[i];
            if (i != sequence1.Length - 1)
            {
                numbers += ", ";
            }
        }
        numbers += "]";

        EqualityComparer<int> intComparer = EqualityComparer<int>.Default;

        Debug.Log("Sequence: " + numbers);

        //All
        Debug.Log("All are multiple of " + multiple + ": " + SetMethods.All(sequence1, (n) => IsMultiple(n, multiple)));
        //Any
        Debug.Log("Is there any " + number + ": " + SetMethods.Any(sequence1, (n) => IsExactNumber(n, number)));

        //Contains
        Debug.Log("Contains " + number2 + ": " + SetMethods.Contains(sequence1, number2));
        Debug.Log("Contains int comparer " + number2 + ": " + SetMethods.Contains(sequence1, number2, intComparer));

        //Distinct
        List<int> distincts = new List<int>(SetMethods.Distinct(sequence1));
        List<int> distinctsComparer = new List<int>(SetMethods.Distinct(sequence1));
        Debug.Log("Distincts: " + GetNumbers(distincts));
        Debug.Log("Distincts int comparer: " + GetNumbers(distinctsComparer));

        //ElementAt
        Debug.Log("ElementAt" + index + ": " + SetMethods.ElementAt(sequence1, index));

        //Except
        Debug.Log("Except: " + GetNumbers(new List<int>(SetMethods.Except(sequence1, sequence2))));
        Debug.Log("Except int comparer: " + GetNumbers(new List<int>(SetMethods.Except(sequence1, sequence2, intComparer))));

        //First
        Debug.Log("First to be multiple of " + firstMultiple + ": " + SetMethods.First(sequence1, (n) => IsMultiple(n, firstMultiple)));

        //Last
        Debug.Log("Last to be multiple of " + lastMultiple + ": " + SetMethods.Last(sequence1, (n) => IsMultiple(n, lastMultiple)));

        //Intersect
        Debug.Log("Intersect " + lastMultiple + ": " + GetNumbers(new List<int>(SetMethods.Intersect(sequence1, sequence2))));
        Debug.Log("Intersect int comparer " + lastMultiple + ": " + GetNumbers(new List<int>(SetMethods.Intersect(sequence1, sequence2, intComparer))));

        //Count
        Debug.Log("Sequence1 multiple count of " + countMultiple + ": " + SetMethods.Count(sequence1, (n) => IsMultiple(n, countMultiple)));

        //SequenceEqual
        Debug.Log("Sequence1 and Sequence2 equality: " + SetMethods.SequenceEqual(sequence1, sequence2, intComparer));

        //Single
        Debug.Log("Single element that satisfies being multiple of " + singleMultiple + ": " + SetMethods.Single(sequence1, (n) => IsMultiple(n, singleMultiple)));

        //SkipWhile
        Debug.Log("Skip while there isnt a multiple of " + skipWhileMultiple + ": " + GetNumbers(new List<int>(SetMethods.SkipWhile(sequence1, (n) => IsMultiple(n, skipWhileMultiple)))));

        //Union
        Debug.Log("Union " + GetNumbers(new List<int>(SetMethods.Union(sequence1, sequence2))));
        Debug.Log("Union int comparere" + GetNumbers(new List<int>(SetMethods.Union(sequence1, sequence2, intComparer))));

        //Where
        Debug.Log("Where index" + whereIndexMultiple + " equals " + whereNumberMultiple + ": " + GetNumbers(new List<int>(SetMethods.Where(sequence1, (n, index) => IsMultiple(n, whereNumberMultiple) && IsMultiple(index, whereIndexMultiple)))));
    }

    private string GetNumbers<T>(List<T> list)
    {
        string numbers = "[";
        for (int i = 0; i < list.Count; i++)
        {
            numbers += list[i];
            if (i != list.Count - 1)
            {
                numbers += ", ";
            }
        }
        numbers += "]";

        return numbers;
    }

    private bool IsMultiple(int evaluated, int number)
    {
        return evaluated % number == 0;
    }

    private bool IsExactNumber(int evaluate, int number)
    {
        return number == evaluate;
    }
}
