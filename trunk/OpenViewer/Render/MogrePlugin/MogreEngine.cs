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
using System.Collections;
using System.Windows.Forms;
//using MogreFramework;
using Mogre;
using OpenViewer.Models;

namespace OpenViewer.Render.MogrePlugin
{
    /// <summary>
    /// 
    /// </summary>
    public class MogreRenderingEngine : IRenderingEngine
    {
        //  private MogreWindow m_window;
        private string m_rendersys = "OpenGL Rendering Subsystem";

        private Model m_model;
        public Root Root;

        protected bool m_running = true;
        protected RenderingWindow m_renderForm;

        public Model Model
        {
            get { return m_model; }
        }

        public MogreRenderingEngine()
        {
        }

        public string GetName()
        {
            return "mogre";
        }

        public void Initialize(Model model)
        {
            //  Application.EnableVisualStyles();
            //  Application.SetCompatibleTextRenderingDefault(false);

            m_renderForm = new RenderingWindow(this);
            InitialiseOgre();
            m_renderForm.Show();

            m_model = model;

            m_renderForm.CreateNewOgreScene("3d Render Window");
            //  m_renderForm.Create3DScene();

            // m_window = new MogreWindow();
            // m_window.Display();
        }

        public void Run()
        {
            while (m_running)
            {
                System.Windows.Forms.Application.DoEvents();
                Root.RenderOneFrame();
            }

            if (Root != null)
            {
                Root.Dispose();
                Root = null;
            }
        }

        public void InitialiseOgre()
        {
            Root = new Root();

            ConfigFile cf = new ConfigFile();
            cf.Load("resources.cfg", "\t:=", true);

            // Go through all sections & settings in the file
            ConfigFile.SectionIterator section = cf.GetSectionIterator();

            String secName, typeName, archName;

            // Normally we would use the foreach syntax, which enumerates the values, but in this case we need CurrentKey too;
            while (section.MoveNext())
            {
                secName = section.CurrentKey;
                ConfigFile.SettingsMultiMap settings = section.Current;
                foreach (KeyValuePair<string, string> pair in settings)
                {
                    typeName = pair.Key;
                    archName = pair.Value;
                    ResourceGroupManager.Singleton.AddResourceLocation(archName, typeName, secName);
                }
            }

            bool foundit = false;
            foreach (RenderSystem rs in Root.GetAvailableRenderers())
            {
                Root.RenderSystem = rs;
                String rname = Root.RenderSystem.Name;
                if (rname == this.m_rendersys)
                {
                    foundit = true;
                    break;
                }
            }

            if (!foundit)
                return; //we didn't find it... Raise exception?

            Root.RenderSystem.SetConfigOption("Full Screen", "No");
            Root.RenderSystem.SetConfigOption("Video Mode", "800 x 600@ 32-bit colour");

            Root.Initialise(false);
        }
    }
}
