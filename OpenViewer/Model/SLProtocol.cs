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
using libsecondlife;
using OpenViewer.Framework;
using OpenViewer.Protocol;

namespace OpenViewer.Model
{
    public class SLProtocol : IProtocol
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SecondLife m_user;
        
        public event Chat OnChat;
        public event Connected OnConnected;
        public event LandPatch OnLandPatch;

        public SLProtocol()
        {
            m_user = new SecondLife();

            m_user.Network.OnConnected += new NetworkManager.ConnectedCallback(SLConnected);
            m_user.Terrain.OnLandPatch += new TerrainManager.LandPatchCallback(SLLandPatch);
            m_user.Self.OnChat += new AgentManager.ChatCallback(SLChat);
        }

        public string GetName()
        {
            return "SecondLife";
        }

        private void SLConnected(Object sender)
        {
            if (OnConnected != null)
                OnConnected(sender);
        }

        private void SLChat(string message, ChatAudibleLevel audible, libsecondlife.ChatType type, ChatSourceType sourcetype,
                            string fromName, LLUUID id, LLUUID ownerid, LLVector3 position)
        {
            // This is weird -- we get start/stop typing chats from
            // other avatars, and we get messages back that we sent.
            // (Tested on OpenSim r3187)
            // So we explicitly check for those cases here.
            if (OnChat != null && (int) type < 4 && id != m_user.Self.AgentID)
            {
                Vector3f pos = new Vector3f(position.X, position.Y, position.Z);

                OnChat(message, fromName, (ChatType) type, id.UUID, ownerid.UUID, pos);
            }
        }

        private void SLLandPatch(Simulator simulator, int x, int y, int width, float[] data)
        {
            if (OnLandPatch != null)
                OnLandPatch(x, y, width, data);
        }

        private void separateUsername(string username, out string fname, out string lname)
        {
            int index = username.IndexOf(" ");

            if (index == -1)
            {
                fname = username.Trim();
                lname = String.Empty;
            }
            else
            {
                fname = username.Substring(0, index).Trim();
                lname = username.Substring(index + 1).Trim();
            }
        }

        public bool Login(string loginURI, string username, string password)
        {
            string fname;
            string lname;

            separateUsername(username, out fname, out lname);

            LoginParams loginParams = m_user.Network.DefaultLoginParams(
                fname, lname, password, "OpenViewer", Constants.Version);

            loginParams.URI = loginURI;
            
            if (!m_user.Network.Login(loginParams))
            {
                log.Error("Login failed: " + m_user.Network.LoginMessage);
                return false;
            }

            return true;
        }

        public void Logout()
        {
            if (m_user.Network.Connected)
                m_user.Network.Logout();
        }

        public bool Connected
        {
            get { return m_user.Network.Connected; }
        }

        public void Say(string message)
        {
            m_user.Self.Chat(message, 0, libsecondlife.ChatType.Normal);
        }

        public void Shout(string message)
        {
            m_user.Self.Chat(message, 0, libsecondlife.ChatType.Shout);
        }
    }
}
