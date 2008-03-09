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
using Gtk;
using OpenViewer.Framework;
using OpenViewer.UI;
using OpenViewer.Models;
using OpenViewer.Render;

namespace OpenViewer.UI.Gtk
{
    // TODO: Allow the user interface to be changed without code
    // changes

    public class GtkUI : UIBase
    {
        private string m_loginURI;
        private string m_username;
        private string m_password;

        public GtkUI()
        {
        }

        public override string GetName()
        {
            return "Gtk";
        }

        public override void Initialize(Model model, string renderingEngine, string loginURI, string username, string password)
        {
            m_model = model;
            m_loginURI = loginURI;
            m_username = username;
            m_password = password;

            RenderingEngineManager renderingEngineManager = new RenderingEngineManager();
            m_renderingEngine = renderingEngineManager.GetEngine(renderingEngine);
        }

        public override void Run()
        {
            Application.Init();

            Glade.XML gxml = new Glade.XML("OpenViewer.glade", "windowMain", null);
            gxml.Autoconnect(this);

            string[] names=m_username.Split(' ');
            entryFirstName.Text = names[0];
            entryLastName.Text = names[1];
            entryPassword.Text = m_password;
            //comboURL. = m_loginURI;

            Application.Run();
        }

        [Glade.Widget] Dialog dialogAbout = null;
        [Glade.Widget] Window windowMain = null;
        [Glade.Widget] Entry entryFirstName = null;
        [Glade.Widget] Entry entryLastName = null;
        [Glade.Widget] Entry entryPassword = null;
        [Glade.Widget] ComboBoxEntry comboURL = null;

        public void on_windowMain_delete_event(object obj, DeleteEventArgs args)
        {
            on_menuQuit_activate(obj, args);
        }

        public void on_menuQuit_activate(object o, EventArgs args)
        {
            Application.Quit();
        }

        public void on_menuAbout_activate(object o, EventArgs args)
        {
            try
            {
                Glade.XML aboutXml = new Glade.XML("openviewer.glade", "dialogAbout", null);
                aboutXml.Autoconnect(this);
                if (dialogAbout == null)
                    Console.WriteLine("dialogAbout is null (?)");
                else
                {
                    if (windowMain == null)
                        Console.WriteLine("windowMain is null (?)");
                    dialogAbout.TransientFor = windowMain;
                    dialogAbout.Run();
                    dialogAbout.Destroy();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not display the About dialog: {0}", e);
            }
        }

        public void on_buttonLogin_clicked(object o, EventArgs args)
        {
            m_username = entryFirstName.Text+" "+entryLastName.Text;
            m_password = entryPassword.Text;
            m_loginURI = comboURL.ActiveText;

            if (!m_model.Login(m_loginURI, m_username, m_password))
            {
                MessageBox.Show(windowMain, "Login failed");
                return;
            }

            m_renderingEngine.Initialize(m_model);
            m_renderingEngine.Run();
        }

        public void on_buttonLogout_clicked(object o, EventArgs args)
        {
        }

        // Utility functions
        private class MessageBox
        {
            public static void Show(Window parentWindow, string msg)
            {
                MessageDialog md = new MessageDialog(parentWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, msg);
                md.Run();
                md.Destroy();
            }
        }
    }
}
