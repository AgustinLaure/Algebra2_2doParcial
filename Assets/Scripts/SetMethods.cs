using System;
using System.Collections.Generic;

public class SetMethods
{
    /// <summary>
    /// Determines whether all elements of a sequence satisfy a condition.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool All<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (TSource element in source)
        {
            if (!predicate(element))
            {
                return false;
            }
        }

        return true;
    }
    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Any<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (TSource element in source)
        {
            if (predicate(element))
            {
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Determines whether a sequence contains a specified element by using the default equality comparer.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool Contains<TSource>(IEnumerable<TSource> source, TSource item)
    {
        IEqualityComparer<TSource> comparer = EqualityComparer<TSource>.Default;

        foreach (TSource element in source)
        {
            //Equals compara referencias en caso de ser clases, o bits en caso de ser copias.
            if (comparer.Equals(element, item))
            {
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Determines whether a sequence contains a specified element by using a specified IEqualityComparer<T>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains<TSource>(IEnumerable<TSource> source, TSource item, IEqualityComparer<TSource> comparer)
    {
        foreach (TSource element in source)
        {
            if (comparer.Equals(element, item))
            {
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Returns distinct elements from a sequence by using the default equality comparer to compare values.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> Distinct<TSource>(IEnumerable<TSource> source)
    {
        IEqualityComparer<TSource> comparer = EqualityComparer<TSource>.Default;

        HashSet<TSource> hashSet = new HashSet<TSource>();

        foreach (TSource element in source)
        {
            if (hashSet.Add(element))
            {
                yield return element;
            }
        }
    }
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <summary>
    /// Returns distinct elements from a sequence by using a specified IEqualityComparer<T> to compare values.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> Distinct<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
    {
        HashSet<TSource> hashSet = new HashSet<TSource>(comparer);

        foreach (TSource element in source)
        {
            if (hashSet.Add(element))
            {
                yield return element;
            }
        }
    }
    /// <summary>
    /// Returns the element at a specified index in a sequence.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static TSource ElementAt<TSource>(IEnumerable<TSource> source, int index)
    {
        if (index < 0)
        {
            throw new IndexOutOfRangeException();
        }

        using (IEnumerator<TSource> enumerator = source.GetEnumerator())
        {
            for (int i = -1; i < index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    throw new IndexOutOfRangeException();
                }
            }

            return enumerator.Current;
        }
    }
    /// <summary>
    /// Produces the set difference of two sequences by using the default equality comparer to compare values.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source1"></param>
    /// <param name="source2"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> Except<TSource>(IEnumerable<TSource> source1, IEnumerable<TSource> source2)
    {
        IEqualityComparer<TSource> comparer = EqualityComparer<TSource>.Default;

        HashSet<TSource> hashSet = new HashSet<TSource>(source2);

        foreach (TSource element in source1)
        {
            if (hashSet.Add(element))
            {
                yield return element;
            }
        }
    }
    /// <summary>
    /// Produces the set difference of two sequences by using the specified IEqualityComparer<T> to compare values.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source1"></param>
    /// <param name="source2"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> Except<TSource>(IEnumerable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
    {
        HashSet<TSource> hashSet = new HashSet<TSource>(source2);

        foreach (TSource element in source1)
        {
            if (hashSet.Add(element))
            {
                yield return element;
            }
        }
    }
    /// <summary>
    /// Returns the first element in a sequence that satisfies a specified condition.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static TSource First<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (TSource element in source)
        {
            if (predicate(element))
            {
                return element;
            }
        }

        throw new ArgumentNullException();
    }
    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static TSource Last<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        TSource lastElement = default;

        foreach (TSource element in source)
        {
            if (predicate(element))
            {
                lastElement = element;
            }
        }

        throw new ArgumentNullException();
    }
    /// <summary>
    /// Produces the set intersection of two sequences by using the default equality comparer to compare values.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source1"></param>
    /// <param name="source2"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> Intersect<TSource>(IEnumerable<TSource> source1, IEnumerable<TSource> source2)
    {
        HashSet<TSource> hashSet = new HashSet<TSource>(source1);

        foreach (TSource element in source2)
        {
            if (hashSet.Remove(element))
            {
                yield return element;
            }
        }
    }
    /// <summary>
    /// Produces the set intersection of two sequences by using the specified IEqualityComparer<T> to compare values.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source1"></param>
    /// <param name="source2"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> Intersect<TSource>(IEnumerable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
    {
        HashSet<TSource> hashSet = new HashSet<TSource>(source1, comparer);

        foreach (TSource element in source2)
        {
            if (hashSet.Remove(element))
            {
                yield return element;
            }
        }
    }
    /// <summary>
    /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int Count<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        int i = 0;

        foreach (TSource element in source)
        {
            if (predicate(element))
            {
                i++;
            }
        }

        return i;
    }
    /// <summary>
    /// Determines whether two sequences are equal by comparing their elements by using a specified IEqualityComparer<T>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source1"></param>
    /// <param name="source2"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool SequenceEqual<TSource>(IEnumerable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
    {
        using IEnumerator<TSource> source1Enumerator = source1.GetEnumerator();
        using IEnumerator<TSource> source2Enumerator = source2.GetEnumerator();

        while (source1Enumerator.MoveNext())
        {
            if (!source2Enumerator.MoveNext())
            {
                return false;
            }

            if (!comparer.Equals(source1Enumerator.Current, source2Enumerator.Current))
            {
                return false;
            }
        }

        return !source2Enumerator.MoveNext();
    }
    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static TSource Single<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        TSource foundElement = default;
        int timesFound = 0;

        foreach (TSource element in source)
        {
            if (predicate(element))
            {
                if (timesFound >= 1)
                {
                    throw new ArgumentException();
                }

                timesFound++;
                foundElement = element;
            }
        }

        if (timesFound <= 0)
        {
            throw new ArgumentException();
        }

        return foundElement;
    }
    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> SkipWhile<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        bool wasPredicateFalse = false;

        foreach (TSource element in source)
        {
            if (wasPredicateFalse)
            {
                yield return element;
            }
            else
            {
                wasPredicateFalse = !predicate(element);

                if (wasPredicateFalse)
                {
                    yield return element;
                }
            }
        }
    }
    /// <summary>
    /// Produces the set union of two sequences by using the default equality comparer.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source1"></param>
    /// <param name="source2"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> Union<TSource>(IEnumerable<TSource> source1, IEnumerable<TSource> source2)
    {
        HashSet<TSource> hashSet = new HashSet<TSource>();

        foreach (TSource element in source1)
        {
            if (hashSet.Add(element))
            {
                yield return element;
            }
        }

        foreach (TSource element in source2)
        {
            if (hashSet.Add(element))
            {
                yield return element;
            }
        }
    }
    /// <summary>
    /// Produces the set union of two sequences by using a specified IEqualityComparer<T>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source1"></param>
    /// <param name="source2"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> Union<TSource>(IEnumerable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
    {
        HashSet<TSource> hashSet = new HashSet<TSource>(comparer);

        foreach (TSource element in source1)
        {
            if (hashSet.Add(element))
            {
                yield return element;
            }
        }

        foreach (TSource element in source2)
        {
            if (hashSet.Add(element))
            {
                yield return element;
            }
        }
    }
    /// <summary>
    /// Filters a sequence of values based on a predicate. Each element's index is used in the logic of the predicate function.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> Where<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
    {
        int i = 0;

        foreach (TSource element in source)
        {
            if (predicate(element, i))
            {
                yield return element;
            }

            i++;
        }
    }
}
