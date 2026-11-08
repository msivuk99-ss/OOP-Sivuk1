using System;
using System.Collections.Generic;

namespace lab8v8;

public static class CollectionHelper
{
    public static bool Any<T>(IEnumerable<T> collection, Predicate<T> predicate)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        foreach (var item in collection)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }
}