using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class Utilities
{
    public static Vector2 xy(this Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.y);
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
}
