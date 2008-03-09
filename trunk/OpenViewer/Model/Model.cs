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
using OpenViewer.Protocol;
using OpenViewer.Framework;

namespace OpenViewer.Models
{
    public class Model
    {
        private IProtocol m_protocol;

        public Dictionary<int, float[]> LandMaps = new Dictionary<int, float[]>();

        public Model(IProtocol protocol)
        {
            m_protocol = protocol;

            m_protocol.OnConnected += OnConnected;
            m_protocol.OnLandPatch += OnLandPatch;
            m_protocol.OnChat += ModelChat;
        }

        public bool Login(string loginURI, string username, string password)
        {
            return m_protocol.Login(loginURI, username, password);
        }

        public void Logout()
        {
            m_protocol.Logout();
        }

        public bool Connected
        {
            get { return m_protocol.Connected; }
        }

        public void Say(string message)
        {
            m_protocol.Say(message);
        }

        public void Shout(string message)
        {
            m_protocol.Shout(message);
        }

        private void OnConnected(Object sender)
        {
        }

        private void ModelChat(string message, string fromName, ChatType type, Guid id, Guid ownerid, Vector3f position)
        {
            if (OnChat != null)
            {
                OnChat(message, fromName, type, id, ownerid, position);
            }
        }

        public event Chat OnChat;

        private void OnLandPatch(int x, int y, int width, float[] data)
        {
            if (!LandMaps.ContainsKey((y * 16) + x))
            {
                LandMaps.Add((y * 16) + x, data);
            }
        }
    }
}
