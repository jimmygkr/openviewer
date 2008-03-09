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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Mogre;

namespace OpenViewer.Render.MogrePlugin
{
    public partial class RenderingWindow : Form
    {
        protected MogreRenderingEngine m_engine;

        protected SceneManager m_sceneMgr;
        protected Camera m_camera;
        protected Viewport m_viewport;
        protected RenderWindow m_window;
        protected SceneNode m_testNode;

        private bool m_terrainCreated = false;
        private int mMoveCam = 0;
        private int mPitch = 0;
        private int mYaw = 0;

        public Panel RenderPanel
        {
            get { return panel1; }
        }

        public RenderingWindow(MogreRenderingEngine renderEngine)
        {
            m_engine = renderEngine;
            InitializeComponent();
        }

        public RenderingWindow()
        {
            InitializeComponent();
        }

        bool FrameStarted(FrameEvent e)
        {
            UpdateCamera(e.timeSinceLastFrame);

            if ((m_engine.Model.LandMaps.Count > 255) && (!m_terrainCreated))
            {
                string fileName = CreateHeightMap();
                m_terrainCreated = true;
                CreateTerrain(512, 512, fileName, "notused");
            }
            return true;
        }

        private string CreateHeightMap()
        {
            Console.WriteLine("creating new map image");
            System.Drawing.Bitmap ourMap = new Bitmap(256, 256);

            string fileName = "myterrain1.jpg";
            string path = Path.Combine("media", "materials");
            path = Path.Combine(path, "textures");
            path = Path.Combine(path, fileName);

            for (int patchy = 0; patchy < 16; patchy++)
            {
                for (int patchx = 0; patchx < 16; patchx++)
                {
                    float[] currentPatch = m_engine.Model.LandMaps[patchy * 16 + patchx];
                    int x = 0;
                    int y = 0;

                    for (int cy = 0; cy < 16; cy++)
                    {
                        for (int cx = 0; cx < 16; cx++)
                        {
                            x = cx + ((patchx) * 16);
                            y = cy + ((patchy) * 16);
                            float value = currentPatch[cy * 16 + cx];
                            value = value / 50;

                            if (value > 1.0f)
                                value = 1.0f;
                            if (value < 0.0f)
                                value = 0.0f;

                            int col = (int)(value * 255.0f);
                            ourMap.SetPixel(x, y, Color.FromArgb(col, col, col));

                        }
                    }
                }
            }

            ImageCodecInfo jpegEncoder = GetImageEncoder("JPEG");

            EncoderParameters parms = new EncoderParameters(1);
            parms.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 8L);

            Bitmap resize = new Bitmap(ourMap, new Size(513, 513));
            //resize.Save(path, jpegEncoder, parms); //can't save to 8 bit grey scale jpeg, so enabling this will cause a crash upon loading that heightmap

            return fileName;
        }

        public void CreateNewOgreScene(string name)
        {
            IntPtr hwnd = this.panel1.Handle;
            NameValuePairList misc = new NameValuePairList();
            misc["externalWindowHandle"] = hwnd.ToString();

            m_window = m_engine.Root.CreateRenderWindow(name, 0, 0, false, misc);
            TextureManager.Singleton.DefaultNumMipmaps = 6;
            ResourceGroupManager.Singleton.InitialiseAllResourceGroups();

            m_sceneMgr = m_engine.Root.CreateSceneManager(SceneType.ST_EXTERIOR_CLOSE, "SceneMgr" + name);
            m_sceneMgr.AmbientLight = new ColourValue(0.8f, 0.8f, 0.8f);

            m_camera = m_sceneMgr.CreateCamera(name + "Camera");
            m_camera.Position = new Vector3(0f, 250f, 700f);
            // Look back along -Z
            m_camera.LookAt(new Vector3(0f, 0f, -300f));
            m_camera.NearClipDistance = 1;

            m_viewport = m_window.AddViewport(m_camera);
            m_viewport.BackgroundColour = new ColourValue(0.0f, 0.0f, 0.0f, 1.0f);

            m_engine.Root.FrameStarted += FrameStarted;

            m_sceneMgr.SetSkyBox(true, "SkyBox");

            CreateTerrainMaterial("grass_1024.jpg", 0.5f, 2, "alpha_dirt.png", "bark-256x256.jpg", 0.1f, "alpha_dirt2.png", "Grass.jpg", 0.1f);
            //CreateTerrain(512, 512, "terrain3.jpg", "notused");

            CreatePlane("water", "Ocean2_Cg", 256, 20, 256, 512, 512, 50, 50, 1, 1);
        }

        public void Create3DScene()
        {
            m_testNode = m_sceneMgr.RootSceneNode.CreateChildSceneNode();
            CustomShape mShape = CreateTestObject();
            m_testNode.AttachObject(mShape.MogreObj);
        }

        private CustomShape CreateTestObject()
        {
            CustomShape mShape = new CustomShape();
            mShape.AddPoint(new Vector3(0, 0, 0));
            mShape.AddTexturePoint(new Vector3(0, 0, 0));

            mShape.AddPoint(new Vector3(100, 0, 0));
            mShape.AddTexturePoint(new Vector3(1, 0, 0));

            mShape.AddPoint(new Vector3(100, 100, 0));
            mShape.AddTexturePoint(new Vector3(1, 1, 0));

            mShape.SetMaterial("Examples/EnvMappedRustySteel");
            mShape.Draw();
            return mShape;
        }

        public SceneNode CreatePlane(string name, string material, float x, float y, float z, float sizex, float sizez, int xseg, int yseg, float tileu, float tilev)
        {
            Plane plane1 = new Plane();
            plane1.normal = new Vector3(0, 1, 0);
            plane1.d = 0.1f;

            MeshPtr floorMesh = MeshManager.Singleton.CreatePlane(name + "1", "", plane1, sizex, sizez, xseg, yseg, true, 1, tileu, tilev, Vector3.UNIT_Z);
            Entity floorEntity = m_sceneMgr.CreateEntity(name, name + "1");
            floorEntity.SetMaterialName(material);
            SceneNode floorNode = (SceneNode)m_sceneMgr.RootSceneNode.CreateChildSceneNode();
            floorNode.AttachObject(floorEntity);
            floorNode.Position = new Vector3(x, y, z);

            return floorNode;
        }

        public void CreateTerrain(float x, float z, string heightmap, string material)
        {
            string terrainString;
            terrainString = "PageSource=Heightmap" + Environment.NewLine;
            terrainString += "Heightmap.image=" + heightmap + Environment.NewLine;
            // terrainString += "Heightmap.raw.size=513";
            // terrainString += "Heightmap.raw.bpp=2";
            terrainString += "PageSize=513" + Environment.NewLine;
            terrainString += "TileSize=65" + Environment.NewLine;
            terrainString += "MaxPixelError=3" + Environment.NewLine;
            terrainString += "PageWorldX=" + x.ToString() + Environment.NewLine;
            terrainString += "PageWorldZ=" + z.ToString() + Environment.NewLine;
            terrainString += "MaxHeight=60" + Environment.NewLine;
            terrainString += "MaxMipMapLevel=5" + Environment.NewLine;
            terrainString += "VertexNormals=yes" + Environment.NewLine;
            terrainString += "VertexProgramMorph=yes";
            terrainString += "LODMorphStart=0.2" + Environment.NewLine;
            terrainString += "CustomMaterialName=test_splat2";

            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(System.Text.Encoding.Default.GetBytes(terrainString));
            Mogre.ManagedDataStream managedStream = new Mogre.ManagedDataStream(memoryStream);

            Mogre.MemoryDataStream memoryData = new MemoryDataStream(managedStream);
            Mogre.DataStreamPtr dataStream = new Mogre.DataStreamPtr(memoryData);

            m_sceneMgr.SetWorldGeometry(dataStream);

            terrainString = null;
            dataStream.Dispose();
            dataStream = null;
        }

        public void CreateTerrainMaterial(string baseTextureImage, float baseTextureScale, int numberSplats, string texture1Alpha, string texture1Image, float texture1Scale, string texture2Alpha, string texture2Image, float texture2Scale)
        {

            MaterialPtr material = MaterialManager.Singleton.Create("test_splat2", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            TextureUnitState textureUnit = material.GetTechnique(0).GetPass(0).CreateTextureUnitState(baseTextureImage);   //("grass_1024.jpg");
            textureUnit.SetTextureScale(baseTextureScale, baseTextureScale);//(0.5f,0.5f);
            if (numberSplats > 0)
            {
                material.GetTechnique(0).GetPass(0).LightingEnabled = false;

                material.GetTechnique(0).CreatePass();
                material.GetTechnique(0).GetPass(1).DepthFunction = CompareFunction.CMPF_EQUAL;
                material.GetTechnique(0).GetPass(1).SetSceneBlending(SceneBlendType.SBT_TRANSPARENT_ALPHA);
                material.GetTechnique(0).GetPass(1).LightingEnabled = false;
                textureUnit = material.GetTechnique(0).GetPass(1).CreateTextureUnitState(texture1Alpha);//("alpha_dirt.png");
                textureUnit.SetAlphaOperation(LayerBlendOperationEx.LBX_SOURCE1, LayerBlendSource.LBS_TEXTURE, LayerBlendSource.LBS_TEXTURE);
                textureUnit.SetColourOperationEx(LayerBlendOperationEx.LBX_SOURCE2, LayerBlendSource.LBS_TEXTURE, LayerBlendSource.LBS_TEXTURE);
                textureUnit = material.GetTechnique(0).GetPass(1).CreateTextureUnitState(texture1Image);//("bark-256x256.jpg");
                textureUnit.SetTextureScale(texture1Scale, texture1Scale);//(0.1f,0.1f);
                textureUnit.SetColourOperationEx(LayerBlendOperationEx.LBX_BLEND_DIFFUSE_ALPHA, LayerBlendSource.LBS_TEXTURE, LayerBlendSource.LBS_CURRENT);
            }
            if (numberSplats > 1)
            {
                material.GetTechnique(0).CreatePass();
                material.GetTechnique(0).GetPass(2).DepthFunction = CompareFunction.CMPF_EQUAL;
                material.GetTechnique(0).GetPass(2).SetSceneBlending(SceneBlendType.SBT_TRANSPARENT_ALPHA);
                material.GetTechnique(0).GetPass(2).LightingEnabled = false;
                textureUnit = material.GetTechnique(0).GetPass(2).CreateTextureUnitState(texture2Alpha);//("alpha_dirt2.png");
                textureUnit.SetAlphaOperation(LayerBlendOperationEx.LBX_SOURCE1, LayerBlendSource.LBS_TEXTURE, LayerBlendSource.LBS_TEXTURE);
                textureUnit.SetColourOperationEx(LayerBlendOperationEx.LBX_SOURCE2, LayerBlendSource.LBS_TEXTURE, LayerBlendSource.LBS_TEXTURE);
                textureUnit = material.GetTechnique(0).GetPass(2).CreateTextureUnitState(texture2Image);//("Grass.jpg");
                textureUnit.SetTextureScale(texture2Scale, texture2Scale);//(0.1f,0.1f);
                textureUnit.SetColourOperationEx(LayerBlendOperationEx.LBX_BLEND_DIFFUSE_ALPHA, LayerBlendSource.LBS_TEXTURE, LayerBlendSource.LBS_CURRENT);
            }

            material.Dispose();
            material = null;
        }

        public void KeyboardDown(System.Windows.Forms.KeyEventArgs k)
        {
            switch (k.KeyCode)
            {
                case Keys.W:
                    mMoveCam |= 1;
                    break;
                case Keys.S:
                    mMoveCam |= 2;
                    break;
                case Keys.A:
                    mMoveCam |= 4;
                    break;
                case Keys.D:
                    mMoveCam |= 8;
                    break;
                case Keys.NumPad8:
                    mPitch |= 1;
                    break;
                case Keys.NumPad2:
                    mPitch |= 2;
                    break;
                case Keys.NumPad4:
                    mYaw |= 1;
                    break;
                case Keys.NumPad6:
                    mYaw |= 2;
                    break;
            }
        }

        public void KeyboardUp(System.Windows.Forms.KeyEventArgs k)
        {
            switch (k.KeyCode)
            {
                case Keys.W:
                    mMoveCam &= ~1;
                    break;
                case Keys.S:
                    mMoveCam &= ~2;
                    break;
                case Keys.A:
                    mMoveCam &= ~4;
                    break;
                case Keys.D:
                    mMoveCam &= ~8;
                    break;
                case Keys.NumPad8:
                    mPitch &= ~1;
                    break;
                case Keys.NumPad2:
                    mPitch &= ~2;
                    break;
                case Keys.NumPad4:
                    mYaw &= ~1;
                    break;
                case Keys.NumPad6:
                    mYaw &= ~2;
                    break;
                case Keys.T:
                    //m_camera.WriteContentsToFile("snapshot.jpg");
                    break;
            }
        }

        public void UpdateCamera(float t)
        {
            if ((mPitch & 1) > 0)
            {
                m_camera.Pitch(new Radian(3.0f * 1.0f * t));
            }
            if ((mPitch & 2) > 0)
            {
                m_camera.Pitch(new Radian(3.0f * -1.0f * t));
            }
            if ((mYaw & 1) > 0)
            {
                m_camera.Yaw(new Radian(3.0f * 1.0f * t));
            }
            if ((mYaw & 2) > 0)
            {
                m_camera.Yaw(new Radian(3.0f * -1.0f * t));
            }
            if (mMoveCam > 0)
            {//mMoveCam bits: 1=forward, 2=backward, 4=left, 8=right, 16=up, 32=down
                Vector3 vCamMove = Vector3.ZERO;
                float mvscale = 150.0f * t;

                if ((mMoveCam & 1) > 0)
                    vCamMove += Vector3.NEGATIVE_UNIT_Z;
                if ((mMoveCam & 2) > 0)
                    vCamMove += Vector3.UNIT_Z;
                if ((mMoveCam & 4) > 0)
                    vCamMove += Vector3.NEGATIVE_UNIT_X;
                if ((mMoveCam & 8) > 0)
                    vCamMove += Vector3.UNIT_X;
                if ((mMoveCam & 16) > 0)
                    vCamMove += Vector3.UNIT_Y;
                if ((mMoveCam & 32) > 0)
                    vCamMove += Vector3.NEGATIVE_UNIT_Y;

                vCamMove *= mvscale;
                m_camera.MoveRelative(vCamMove);
            }
        }

        private void RenderingWindow_KeyDown(object sender, KeyEventArgs e)
        {
            KeyboardDown(e);
        }

        private void RenderingWindow_KeyUp(object sender, KeyEventArgs e)
        {
            KeyboardUp(e);
        }

        static ImageCodecInfo GetImageEncoder(string imageType)
        {
            imageType = imageType.ToUpperInvariant();

            foreach (ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
            {
                if (info.FormatDescription == imageType)
                {
                    return info;
                }
            }
            return null;
        }
    }
}
