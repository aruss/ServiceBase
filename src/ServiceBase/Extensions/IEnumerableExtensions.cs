// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Models;

    /// <summary>
    /// Offers extension methods for the <see cref="IEnumerable{T}" />
    /// interface
    /// </summary>
    public static partial class IEnumerableExtensions
    {
        #region Public Static Methods

        /// <summary>
        /// Counts the amount of items in the current enumerable.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable whose amount of items should be calculated.
        /// </param>
        /// <returns>
        /// The amount of items in the current enumerable.
        /// </returns>
        public static int Count(this IEnumerable enumerable)
        {
            int i = 0;

            IEnumerator e = enumerable.GetEnumerator();

            while (e.MoveNext())
            {
                i++;
            }

            return i;
        }

        /// <summary>
        /// Checks whether the current enumerable contains any item.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable which should be checked.
        /// </param>
        /// <returns>
        /// A flag indicating whether the current enumerable contains any
        /// items.
        /// </returns>
        public static bool Any(this IEnumerable enumerable)
        {
            foreach (object obj in enumerable)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the element at the specified index from the passed
        /// enumerable.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable from which the element at the specified index
        /// should be returned.
        /// </param>
        /// <param name="index">
        /// The index of the element which should be returned.
        /// </param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        public static object ElementAt(this IEnumerable enumerable, uint index)
        {
            IEnumerator e = enumerable.GetEnumerator();

            for (int i = 0; i < index; i++)
            {
                e.MoveNext();
            }

            return e.Current;
        }

        /// <summary>
        /// Returns the first element in the enumeration or null if the
        /// enumeration is empty.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable to iterate.
        /// </param>
        /// <returns>
        /// The first element or null.
        /// </returns>
        public static object FirstOrDefault(this IEnumerable enumerable)
        {
            foreach (object obj in enumerable)
            {
                return obj;
            }

            return default(object);
        }

        /// <summary>
        /// Concats the passed elements to the current enumerable.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="enumerable">
        /// The current enumerable which should be concatted with the passed
        /// elements.
        /// </param>
        /// <param name="elems">
        /// The elements which should be concatted.
        /// </param>
        /// <returns>
        /// The concatted enumerable.
        /// </returns>
        public static IEnumerable<T> Concat<T>(
            this IEnumerable<T> enumerable, params T[] elems)
        {
            return enumerable.Concat((IEnumerable<T>)elems);
        }

        /// <summary>
        /// Returns all elements in the passed enumerable except those
        /// specified in <see cref="P:elems" />.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="enumerable">
        /// </param>
        /// <param name="elems">
        /// The elements which shouldnt be returned.
        /// </param>
        /// <returns>
        /// All elements in <see cref="P:enumerable" /> which aren't specified
        /// in <see cref="P:elems" />.
        /// </returns>
        public static IEnumerable<T> Except<T>(
            this IEnumerable<T> enumerable, params T[] elems)
        {
            foreach (T item in enumerable)
            {
                if (!elems.Contains(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Executes the passed action for each element in the passed
        /// IEnumerable.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the elements in the IEnumerable.
        /// </typeparam>
        /// <param name="t">
        /// Current instance of the ienumerable.
        /// </param>
        /// <param name="action">
        /// The action to be executed for each element.
        /// </param>
        public static void ForEach<T>(this IEnumerable<T> t, Action<T> action)
        {
            foreach (T item in t)
            {
                action(item);
            }
        }

        /// <summary>
        /// Batches the current IEnumerable into multiple lists.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements in the IEnumerable.
        /// </typeparam>
        /// <param name="col">
        /// The current IEnumerable which should be splitted in smaller
        /// batches.
        /// </param>
        /// <param name="batchSize">
        /// BatchMode.ByBatchCount (default): The amount of batches the
        /// enumerable should be splitted into. /// BatchMode.ByElementCount:
        /// The amount of elements which should be placed into each batch.
        /// </param>
        /// <param name="mode">
        /// A flag indicating whether you want to batch the enumerable into n
        /// (=batchSize) own lists which would be BatchMode.ByBatchCount or in
        /// n (=unknown) lists where all consists of maximum m (=batchSize)
        /// elements.
        /// </param>
        /// <returns>
        /// A list of all batches.
        /// </returns>
        public static List<List<T>> Batch<T>(
            this IEnumerable<T> col,
            int batchSize,
            BatchMode mode = BatchMode.ByBatchCount)
        {
            List<List<T>> splitted = new List<List<T>>();
            List<T> currentList = null;
            int idx = 0;

            if (mode == BatchMode.ByBatchCount)
            {
                for (int i = 0; i < batchSize; i++)
                {
                    splitted.Add(new List<T>());
                }

                int listIdx = 0;
                int totalItems = col.Count();
                int elementsPerBatch = totalItems / batchSize;

                foreach (T item in col)
                {
                    if (idx >= elementsPerBatch)
                    {
                        idx = 0;
                        listIdx = listIdx + 1 < batchSize
                            ? listIdx + 1 : batchSize - 1;
                    }

                    currentList = splitted[listIdx];
                    currentList.Add(item);

                    idx++;
                }
            }
            else if (mode == BatchMode.ByElementCount)
            {
                foreach (T item in col)
                {
                    if (idx == 0 || idx == batchSize)
                    {
                        idx = 0;
                        currentList = new List<T>(batchSize);
                        splitted.Add(currentList);
                    }

                    currentList.Add(item);

                    idx++;
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return splitted;
        }

        /// <summary>
        /// Converts the current IEnumerable to a tree structure.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the nodes.
        /// </typeparam>
        /// <param name="enumerable">
        /// The IEnumerable from which a tree structure should be created.
        /// </param>
        /// <param name="parentSelector">
        /// A selector function which gets the parent of an element. (Return
        /// null in case of root element).
        /// </param>
        /// <returns>
        /// A tree structure containing elements from the current list.
        /// </returns>
        public static Tree<T> ToTree<T>(
            this IEnumerable<T> enumerable, Func<T, T> parentSelector)
        {
            NullableDictionary<T, List<T>> nodes =
                new NullableDictionary<T, List<T>>();

            List<T> list = enumerable.ToList();

            foreach (T element in list)
            {
                T parent = parentSelector(element);
                List<T> tmp;

                if (!nodes.TryGetValue(parent, out tmp))
                {
                    nodes[parent] = tmp = new List<T>();
                }

                tmp.Add(element);
            }

            return new Tree<T>(nodes);
        }

        /// <summary>
        /// Converts the current IEnumerable to a tree structure.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the nodes.
        /// </typeparam>
        /// <param name="enumerable">
        /// The IEnumerable from which a tree structure should be created.
        /// </param>
        /// <param name="comperator">
        /// A comperator function which gets the parent of an element. (Return
        /// null in case of root element).
        /// </param>
        /// <returns>
        /// A tree structure containing elements from the current list.
        /// </returns>
        public static Tree<T> ToTree<T>(
            this IEnumerable<T> enumerable, Func<T, T, bool> comperator)
        {
            NullableDictionary<T, List<T>> nodes =
                new NullableDictionary<T, List<T>>();

            List<T> list = enumerable.ToList();

            foreach (T element in list)
            {
                T parent = list.FirstOrDefault(x => comperator(element, x));
                List<T> tmp;

                if (!nodes.TryGetValue(parent, out tmp))
                {
                    nodes[parent] = tmp = new List<T>();
                }

                tmp.Add(element);
            }

            return new Tree<T>(nodes);
        }

        /// <summary>
        /// Gets all elements in the current enumerable distinct by a specific
        /// function.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="list">
        /// The current list which should be filtered distinctly.
        /// </param>
        /// <param name="keySelector">
        /// The function which gets the key.
        /// </param>
        /// <returns>
        /// A enumeration over all distinct (by the specific key) elements.
        /// </returns>
        public static IEnumerable<T> DistinctBy<T>(
            this IEnumerable<T> list,
            Func<T, dynamic> keySelector)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            Dictionary<dynamic, T> dict = new Dictionary<dynamic, T>();

            foreach (T item in list)
            {
                dict[keySelector(item)] = item;
            }

            foreach (T item in dict.Values)
            {
                yield return item;
            }
        }

        /// <summary>
        /// The index of the item matching the passed predicate or -1 if none
        /// matched.
        /// </summary>
        /// <remarks>
        /// Depending on the underlying type it is ***NOT*** guaranteed that
        /// the index is the same the second time you iterate that
        /// ienumerable.
        /// </remarks>
        /// <typeparam name="T">
        /// The type of the elements to iterate over.
        /// </typeparam>
        /// <param name="list">
        /// The current list from which a index should be retrieved.
        /// </param>
        /// <param name="predicate">
        /// The predicate which should return true for the element whose index
        /// should be returned.
        /// </param>
        /// <param name="offset">
        /// The offset to start looking from. (will not be substracted on
        /// return val)
        /// </param>
        /// <returns>
        /// The index of the first element to match the passed predicate or -1
        /// if none was found.
        /// </returns>
        public static int IndexOf<T>(
            this IEnumerable<T> list,
            Func<T, bool> predicate,
            int offset = 0)
        {
            int i = 0;

            foreach (T item in list)
            {
                if (i >= offset && predicate(item))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        /// <summary>
        /// Orders the passed enumerable by dependencies, so that you can
        /// iterate over the result without having to care about dependencies.
        /// </summary>
        /// <exception cref="CyclicReferenceException">
        /// There are dependencies depending on each other. "a requires b, b
        /// requires a. boom."
        /// </exception>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="source">
        /// The enumerable from which a ordered set should be created.
        /// </param>
        /// <param name="getDependencies">
        /// A function which gets the direct(non recursive) dependencies for
        /// the current elements.
        /// </param>
        /// <param name="throwCyclicReferenceException">
        /// A flag whether a <see cref="CyclicReferenceException" /> should be
        /// thrown in case of cyclic references or it should be ignored.
        /// </param>
        /// <returns>
        /// The ordered list where you dont have to care about dependencies.
        /// </returns>
        public static IEnumerable<T> TopologicalSort<T>(
            this IEnumerable<T> source,
            Func<T, IEnumerable<T>> getDependencies,
            bool throwCyclicReferenceException = true)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (getDependencies == null)
            {
                throw new ArgumentNullException("getDependencies");
            }

            List<T> sorted = new List<T>();
            List<T> visited = new List<T>();
            Action<T> visit = null;

            visit = x =>
            {
                if (!visited.Contains(x))
                {
                    visited.Add(x);
                    getDependencies(x).ForEach(visit);
                    sorted.Add(x);
                }
                else if (throwCyclicReferenceException && !sorted.Contains(x))
                {
                    throw new CyclicReferenceException(x);
                }
            };

            source.ForEach(visit);

            return sorted;
        }

        /// <summary>
        /// Expands a enumerable of types by resolving interfaces to all
        /// concrete implementations.
        /// </summary>
        /// <param name="enumerable">
        /// The collection of types where interfaces should be expanded.
        /// </param>
        /// <returns>
        /// A enumerable over all concrete types.
        /// </returns>
        public static IEnumerable<Type> ExpandInterfaces(
            this IEnumerable<Type> enumerable)
        {
            foreach (Type t in enumerable)
            {
                if (t.IsInterface || t.IsAbstract)
                {
                    IEnumerable<Type> concretTypes =
                        t.GetAllInheritingTypes(true, true, true);

                    foreach (Type subT in concretTypes)
                    {
                        yield return subT;
                    }
                }
                else
                {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// Compares to lists and returns a Tuple with removed,
        /// added and updated lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listMaster"></param>
        /// <param name="listupdated"></param>
        /// <returns></returns>
        public static (
            IEnumerable<TSource> removed,
            IEnumerable<TSource> added,
            IEnumerable<TSource> updated
        )
        Diff<TSource>(
            this IEnumerable<TSource> listMaster,
            IEnumerable<TSource> listUpdated
        )
        {
            var removed = new List<TSource>();
            var added = new List<TSource>();
            var updated = new List<TSource>();
            TSource defaultOfTSource = default(TSource);

            removed = listMaster.Select(s => s).ToList();
            added = listUpdated.Select(s => s).ToList();

            for (int i = removed.Count - 1; i >= 0; i--)
            {
                TSource item = removed[i];
                TSource itemUpdated = listUpdated
                    .FirstOrDefault(c => c.Equals(item));
                
                if (!Comparison<TSource>.Equals(defaultOfTSource, itemUpdated))
                {
                    updated.Add(itemUpdated);
                    added.Remove(item);
                    removed.Remove(item);
                }
            }

            return (removed, added, updated);
        }

        #endregion
    }
}
