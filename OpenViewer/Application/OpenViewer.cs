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
using OpenViewer.UI;
using OpenViewer.Models;
using OpenViewer.Protocol;

namespace OpenViewer.Application
{
    class OpenViewer
    {
        private IUI m_ui;
        private UIManager m_uiManager;

        private IProtocol m_protocol;
        private ProtocolManager m_protocolManager;

        private Model m_model;
        private string m_loginURI;
        private string m_username;
        private string m_password;
        private string m_renderingEngine;

        public OpenViewer(string loginURI, string username, string password, string renderingEngine, string ui, string protocol)
        {
            m_loginURI = loginURI;
            m_username = username;
            m_password = password;

            m_renderingEngine = renderingEngine;

            m_uiManager = new UIManager();
            m_ui = m_uiManager.GetUI(ui);

            m_protocolManager = new ProtocolManager();
            m_protocol = m_protocolManager.GetProtocol(protocol);
        }

        public bool Start()
        {
            m_model = new Model(m_protocol);
            m_ui.Initialize(m_model, m_renderingEngine, m_loginURI, m_username, m_password);

            return true;
        }

        public void Run()
        {
            m_ui.Run();
        }
    }
}
