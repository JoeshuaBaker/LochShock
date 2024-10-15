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

    public static void RemoveRange<T>(this List<T> list, IEnumerable<T> other)
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

    public static string AddColorToString(this string baseString, Color color)
    {
        string colorPrefix = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
        string colorPostfix = "</color>";

        return colorPrefix + baseString + colorPostfix;
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

    public static string[] SplitAfterFirstNumber(this string originalString)
    {
        var regex = new Regex(@"(?<=\d)(?=\D)");
        return regex.Split(originalString, 2);
    }

    public static Vector2 GetDirectionToPlayer(Vector3 position)
    {
        if(Player.activePlayer != null)
        {
            return GetDistanceVecToPlayer(position).normalized;
        }

        return Vector2.zero;
    }

    public static Vector2 GetDistanceVecToPlayer(Vector3 position)
    {
        if (Player.activePlayer != null)
        {
            return (Player.activePlayer.transform.position - position).xy();
        }

        return Vector2.zero;
    }

    public static float GetDistanceToPlayer(Vector3 position)
    {
        if (Player.activePlayer != null)
        {
            return (Player.activePlayer.transform.position - position).magnitude;
        }

        return 0f;
    }

    public static Vector2 RotateTowards(Vector2 current, Vector2 target, float maxRadiansDelta, float maxMagnitudeDelta = 0f)
    {
        if (current.x + current.y == 0)
            return target.normalized * maxMagnitudeDelta;

        float signedAngle = Vector2.SignedAngle(current, target);
        float stepAngle = Mathf.MoveTowardsAngle(0, signedAngle, maxRadiansDelta * Mathf.Rad2Deg) * Mathf.Deg2Rad;
        Vector2 rotated = new Vector2(
            current.x * Mathf.Cos(stepAngle) - current.y * Mathf.Sin(stepAngle),
            current.x * Mathf.Sin(stepAngle) + current.y * Mathf.Cos(stepAngle)
        );
        if (maxMagnitudeDelta == 0)
            return rotated;

        float magnitude = current.magnitude;
        float targetMagnitude = target.magnitude;
        targetMagnitude = Mathf.MoveTowards(magnitude, targetMagnitude, maxMagnitudeDelta);
        return rotated.normalized * targetMagnitude;
    }
}
