using System;
using UnityEngine;

namespace TestUnityPlugin
{
    internal class Renderer
    {
        public static void DrawString(Vector2 position, string label, bool centered = true)
        {
            var content = new GUIContent(label);
            var size = new GUIStyle(GUI.skin.label).CalcSize(content);
            var upperLeft = centered ? position - size / 2f : position;
            GUI.Label(new Rect(upperLeft, size), content);
        }
        public static void DrawColorString(Vector2 position, string label, Color color, float size, bool centered = true)
        {
            var content = new GUIContent(label);
            var style = new GUIStyle();
            style.fontSize = Mathf.RoundToInt(size);
            style.normal.textColor = color;

            var sizeVec = style.CalcSize(content);
            var upperLeft = centered ? position - sizeVec / 2f : position;

            GUI.Label(new Rect(upperLeft, sizeVec), content, style);
        }
        public static Vector2 CalcStringSize(string label, float size)
        {
            var content = new GUIContent(label);
            var style = new GUIStyle();
            style.fontSize = Mathf.RoundToInt(size);
            return style.CalcSize(content);
        }
        public static Texture2D lineTex;
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            Matrix4x4 matrix = GUI.matrix;
            if (!lineTex)
                lineTex = new Texture2D(1, 1);

            Color color2 = GUI.color;
            GUI.color = color;
            float num = Vector3.Angle(pointB - pointA, Vector2.right);

            if (pointA.y > pointB.y)
                num = -num;

            GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
            GUIUtility.RotateAroundPivot(num, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), lineTex);
            GUI.matrix = matrix;
            GUI.color = color2;
        }
    }
}
