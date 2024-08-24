using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Linq;

public static class Utilities
{
    public static Vector2 xy(this Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.y);
    }

    public static Vector3 xyz(this Vector2 vec2)
    {
        return new Vector3(vec2.x, vec2.y, 0f);
    }

    public static string SplitCamelCaseLower(this string str)
    {
        return Regex.Replace(
            Regex.Replace(
                str,
                @"(\P{Ll})(\P{Ll}\p{Ll})",
                "$1$2"
            ),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        ).Replace("  ", " ").ToLower();
    }

    public static string SplitCamelCase(this string str)
    {
        return Regex.Replace(
            Regex.Replace(
                str,
                @"(\P{Ll})(\P{Ll}\p{Ll})",
                "$1$2"
            ),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        ).Replace("  ", " ");
    }

    public static void RemoveRange<T>(this List<T> list, List<T> other)
    {
        foreach (T item in other)
        {
            list.Remove(item);
        }
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.ShuffleIterator(new System.Random(Time.frameCount));
    }

    private static IEnumerable<T> ShuffleIterator<T>(
        this IEnumerable<T> source, System.Random rng)
    {
        var buffer = source.ToList();
        for (int i = 0; i < buffer.Count; i++)
        {
            int j = rng.Next(i, buffer.Count);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }
}
