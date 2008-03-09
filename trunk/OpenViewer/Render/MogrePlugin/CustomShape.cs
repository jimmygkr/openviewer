/*
 * Copyright (c) Contributors, http://openviewer.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenViewer Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mogre;

namespace OpenViewer.Render.MogrePlugin
{
    public class CustomShape
    {
        public ArrayList Points = null;
        protected ArrayList m_texturePoints = null;
        protected bool m_Drawn;
        private int m_count = 1;
        public Mogre.ManualObject MogreObj = null;
        public string MaterialName = "";
        public Mogre.RenderOperation.OperationTypes OperationType = Mogre.RenderOperation.OperationTypes.OT_TRIANGLE_LIST;
      
        #region constructor / destructor

        public CustomShape()
        {
            Points = new ArrayList();
            m_texturePoints = new ArrayList();
        }

        ~CustomShape()
        {
            this.Dispose();
        }

        public void Dispose()
        {

        }

        #endregion

        public void SetMaterial(string mname)
        {
            MaterialName = mname;
        }
        public void AddPoint(Vector3 p)
        {
            Points.Add(p);
        }
        public void AddTexturePoint(Vector3 p)
        {
            m_texturePoints.Add(p);
        }
        public Vector3 GetPoint(int index)
        {
            return (Vector3)Points[index];
        }

        public int GetNumPoints()
        {
            return Points.Count;
        }

        public void UpdatePoint(int index, Vector3 v)
        {
            Points[index] = v;
        }

        public void DeletePoint(int index)
        {
            Points.RemoveAt(index);
        }
        public void Clear()
        {
            Points.Clear();
            m_texturePoints.Clear();
        }

        public void InsertPoint(int index, Vector3 p)
        {
            Points.Insert(index, p);
        }

        public void DrawLine(Vector3 start, Vector3 end)
        {
            if (Points.Count > 0)
                Points.Clear();

            Points.Add(start);
            Points.Add(end);

            Draw();
        }

        public void DrawLinesList()
        {
            MogreObj = new ManualObject("tree" + m_count);
            MogreObj.Begin(this.MaterialName, Mogre.RenderOperation.OperationTypes.OT_LINE_LIST);
            for (int i = 0; i < this.Points.Count; i++)
            {
                MogreObj.Position((Vector3)this.Points[i]);

            }
            MogreObj.End();
            m_count++;
        }

        public void DrawLinesTriangles()
        {
            MogreObj = new ManualObject("tree" + m_count);
            MogreObj.Begin(this.MaterialName);
            for (int i = 0; i < this.Points.Count; i++)
            {
                MogreObj.Position((Vector3)this.Points[i]);
                MogreObj.TextureCoord((Vector3)this.m_texturePoints[i]);
            }
            MogreObj.End();
            m_count++;
        }

        public void DrawLinesNoTexture()
        {
            MogreObj = new ManualObject("tree" + m_count);
            MogreObj.Begin(this.MaterialName);
            for (int i = 0; i < this.Points.Count; i++)
            {
                MogreObj.Position((Vector3)this.Points[i]);
            }
            MogreObj.End();
            m_count++;
        }

        public void DrawLinesNoTextIndex()
        {
            MogreObj = new ManualObject("tree" + m_count);
            //System.Console.WriteLine("material name is " + this.MaterialName);
            MogreObj.Begin(this.MaterialName);
            for (int i = 0; i < this.Points.Count; i++)
            {
                Vector3 orig = new Vector3(30, 30, 30);
                MogreObj.Position(((Vector3)this.Points[i] - orig));
                MogreObj.TextureCoord(((Vector3)this.m_texturePoints[i]).x, ((Vector3)this.m_texturePoints[i]).y);
            }

            for (int i = 0; i < this.Points.Count; i++)
            {
                MogreObj.Index((ushort)i);
            }
            MogreObj.End();
            m_count++;
        }

        public void Draw()
        {
            // System.Console.WriteLine("drawing manual object");
            MogreObj = new ManualObject("tree" + m_count);
            MogreObj.Begin(this.MaterialName, Mogre.RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
            for (int i = 0; i < this.Points.Count; i++)
            {
                MogreObj.Position((Vector3)this.Points[i]);

                MogreObj.TextureCoord((Vector3)this.m_texturePoints[i]);
            }

            for (int i = 0; i < this.Points.Count; i++)
            {
                MogreObj.Index((ushort)i);
            }
            MogreObj.End();
            m_count++;
        }
    }
}
