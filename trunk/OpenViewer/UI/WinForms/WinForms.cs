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
using System.Drawing;
using System.Windows.Forms;
using OpenViewer.Framework;
using OpenViewer.UI;
using OpenViewer.Models;
using OpenViewer.Render;

namespace OpenViewer.UI.WinForms
{
    // TODO: Support XAML to enable the user interface to be changed
    // without code changes

    public class WinFormsUI : UIBase
    {
        private string m_loginURI;
        private string m_username;
        private string m_password;

        public WinFormsUI()
        {
        }

        public override string GetName()
        {
            return "WinForms";
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
            Application.Run(new MainForm(m_model, m_renderingEngine, m_loginURI, m_username, m_password));
        }
    }

    public class MainForm : Form
    {
        private string m_loginURI;
        private string m_username;
        private string m_password;

        private Model m_model;
        private IRenderingEngine m_renderingEngine;

        private TextBox usernameTextbox;
        private TextBox passwordTextbox;

        public MainForm(Model model, IRenderingEngine renderer, string loginURI, string username, string password)
        {
            m_loginURI = loginURI;
            m_username = username;
            m_password = password;

            m_model = model;
            m_renderingEngine = renderer;

            MainMenu mainMenu = new MainMenu();
            MenuItem File = mainMenu.MenuItems.Add("&File");
            MenuItem Help = mainMenu.MenuItems.Add("&Help");

            File.MenuItems.Add(new MenuItem("&Exit", new EventHandler(Menu_Exit)));
            Help.MenuItems.Add(new MenuItem("&About", new EventHandler(Menu_About)));

            this.Menu = mainMenu;

            Label usernameLabel = new Label();
            usernameLabel.UseMnemonic = true;
            usernameLabel.Text = "Username:";
            usernameLabel.Location = new Point(15, 15);
            usernameLabel.Size = new Size(usernameLabel.PreferredWidth, usernameLabel.PreferredHeight + 2);

            usernameTextbox = new TextBox();
            usernameTextbox.Text = m_username;
            usernameTextbox.Location = new Point(15 + usernameLabel.PreferredWidth + 5, 15);
            usernameTextbox.Size = new Size(160, 20);

            Label passwordLabel = new Label();
            passwordLabel.Text = "Password:";
            passwordLabel.Location = new Point(15, 15 + usernameLabel.PreferredHeight + 2);
            passwordLabel.Size = new Size(passwordLabel.PreferredWidth, passwordLabel.PreferredHeight + 2);

            passwordTextbox = new TextBox();
            passwordTextbox.UseSystemPasswordChar = true;
            passwordTextbox.Text = m_password;
            passwordTextbox.Location = new Point(15 + passwordLabel.PreferredWidth + 5, 15 + usernameLabel.PreferredHeight + 2);
            passwordTextbox.Size = new Size(160, 20);

            Button okButton = new Button();
            okButton.Text = "&OK";
            okButton.Location = new Point(15, 15 + usernameLabel.PreferredHeight + 2 + passwordLabel.PreferredHeight + 2);
            okButton.Size = new Size(50, 20);

            okButton.Click += new EventHandler(OK_Clicked);

            this.Controls.Add(usernameLabel);
            this.Controls.Add(usernameTextbox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextbox);

            this.Controls.Add(okButton);

            this.Text = "OpenViewer " + Constants.Version;
        }

        private void Menu_Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Menu_About(object sender, EventArgs e)
        {
            MessageBox.Show("OpenViewer " + Constants.Version + Environment.NewLine +
                "Copyright 2008 OpenViewer Team" + Environment.NewLine +
                "Released under the BSD license." + Environment.NewLine +
                Environment.NewLine +
                "http://openviewer.org/");
        }

        private void OK_Clicked(object sender, EventArgs e)
        {
            m_username = usernameTextbox.Text;
            m_password = passwordTextbox.Text;

            if (!m_model.Login(m_loginURI, m_username, m_password))
            {
                MessageBox.Show("Login failed");
                return;
            }

            m_renderingEngine.Initialize(m_model);
            m_renderingEngine.Run();
        }
    }
}
