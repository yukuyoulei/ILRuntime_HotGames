using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/TextSpacing")]
public class TextSpacing : BaseMeshEffect
{
    #region Struct

    public enum HorizontalAligmentType
    {
        Left,
        Center,
        Right
    }

    public class Line
    {
        // 起点索引
        public int StartVertexIndex { get { return _startVertexIndex; } }
        private int _startVertexIndex = 0;

        // 终点索引
        public int EndVertexIndex { get { return _endVertexIndex; } }
        private int _endVertexIndex = 0;

        // 该行占的点数目
        public int VertexCount { get { return _vertexCount; } }
        private int _vertexCount = 0;

        public Line(int startVertexIndex, int length)
        {
            _startVertexIndex = startVertexIndex;
            _endVertexIndex = length * 6 - 1 + startVertexIndex;
            _vertexCount = length * 6;
        }
    }

    #endregion

    public float Spacing = 1f;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0)
        {
            return;
        }

        var text = GetComponent<Text>();

        if (text == null)
        {
            Debug.LogError("Missing Text component");
            return;
        }

        // 水平对齐方式
        HorizontalAligmentType alignment;
        if (text.alignment == TextAnchor.LowerLeft || text.alignment == TextAnchor.MiddleLeft || text.alignment == TextAnchor.UpperLeft)
        {
            alignment = HorizontalAligmentType.Left;
        }
        else if (text.alignment == TextAnchor.LowerCenter || text.alignment == TextAnchor.MiddleCenter || text.alignment == TextAnchor.UpperCenter)
        {
            alignment = HorizontalAligmentType.Center;
        }
        else
        {
            alignment = HorizontalAligmentType.Right;
        }

        var vertexs = new List<UIVertex>();
        vh.GetUIVertexStream(vertexs);
        // var indexCount = vh.currentIndexCount;

        var lineTexts = text.text.Split('\n');

        var lines = new Line[lineTexts.Length];

        // 根据lines数组中各个元素的长度计算每一行中第一个点的索引，每个字、字母、空母均占6个点
        for (var i = 0; i < lines.Length; i++)
        {
            // 除最后一行外，vertexs对于前面几行都有回车符占了6个点
            if (i == 0)
            {
                lines[i] = new Line(0, lineTexts[i].Length + 1);
            }
            else if (i > 0 && i < lines.Length - 1)
            {
                lines[i] = new Line(lines[i - 1].EndVertexIndex + 1, lineTexts[i].Length + 1);
            }
            else
            {
                lines[i] = new Line(lines[i - 1].EndVertexIndex + 1, lineTexts[i].Length);
            }
        }

        UIVertex vt;

        for (var i = 0; i < lines.Length; i++)
        {
            for (var j = lines[i].StartVertexIndex; j <= lines[i].EndVertexIndex; j++)
            {
                if (j < 0 || j >= vertexs.Count)
                {
                    continue;
                }

                vt = vertexs[j];

                var charCount = lines[i].EndVertexIndex - lines[i].StartVertexIndex;
                if (i == lines.Length - 1)
                {
                    charCount += 6;
                }

                if (alignment == HorizontalAligmentType.Left)
                {
                    vt.position += new Vector3(Spacing * ((j - lines[i].StartVertexIndex) / 6), 0, 0);
                }
                else if (alignment == HorizontalAligmentType.Right)
                {
                    vt.position += new Vector3(Spacing * (-(charCount - j + lines[i].StartVertexIndex) / 6 + 1), 0, 0);
                }
                else if (alignment == HorizontalAligmentType.Center)
                {
                    var offset = (charCount / 6) % 2 == 0 ? 0.5f : 0f;
                    vt.position += new Vector3(Spacing * ((j - lines[i].StartVertexIndex) / 6 - charCount / 12 + offset), 0, 0);
                }

                vertexs[j] = vt;
                // 以下注意点与索引的对应关系
                if (j % 6 <= 2)
                {
                    vh.SetUIVertex(vt, (j / 6) * 4 + j % 6);
                }

                if (j % 6 == 4)
                {
                    vh.SetUIVertex(vt, (j / 6) * 4 + j % 6 - 1);
                }
            }
        }
    }
}


