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
using OpenViewer.Framework;
using OpenViewer.UI;
using OpenViewer.Models;
using OpenViewer.Render;

namespace OpenViewer.UI.Console
{
    public class ConsoleUI : UIBase
    {
        private string m_loginURI;
        private string m_username;
        private string m_password;

        public ConsoleUI()
        {
        }

        public override string GetName()
        {
            return "console";
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
            if (!m_model.Login(m_loginURI, m_username, m_password))
            {
                System.Console.WriteLine("Login failed");
                return;
            }

            m_renderingEngine.Initialize(m_model);
            m_renderingEngine.Run();
        }
    }
}
