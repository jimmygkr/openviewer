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
using System.IO;
using Nini.Config;
using OpenViewer.Framework;

namespace OpenViewer.Application
{
    class Program
    {
        private static IniConfigSource m_config;
        private static string m_loginURI;
        private static string m_uiEngine;
        private static string m_renderingEngine;
        private static string m_protocol;
        private static string m_username;
        private static string m_password;

        private static void LoadConfig(IConfigSource configSource)
        {
            m_config = new IniConfigSource();

            IConfig startupConfig = configSource.Configs["Startup"];
            string iniFilePath = startupConfig.GetString("inifile", "OpenViewer.ini");

            if (File.Exists(iniFilePath))
            {
                m_config.Merge(new IniConfigSource(iniFilePath));
            }
            else
            {
                SetDefaultConfig();
            }

            m_config.Merge(configSource);

            ReadConfigSettings();
        }

        private static void SetDefaultConfig()
        {
            if (m_config.Configs["Startup"] == null)
                m_config.AddConfig("Startup");
        }

        private static void ReadConfigSettings()
        {
            IConfig startupConfig = m_config.Configs["Startup"];
            if (startupConfig != null)
            {
                m_loginURI = startupConfig.GetString("loginuri", "http://localhost:9000/");
                m_username = startupConfig.GetString("username", "Test User");
                m_password = startupConfig.GetString("password", "test");
                m_uiEngine = startupConfig.GetString("ui_engine", "Gtk");
                m_renderingEngine = startupConfig.GetString("rendering_engine", "mogre");
                m_protocol = startupConfig.GetString("protocol", "SecondLife");
            }
        }

        private static void ShowVersion()
        {
            Console.WriteLine("OpenViewer " + Constants.Version + VersionInfo.svnRevision);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: OpenViewer.exe [options]");
            Console.WriteLine("Options:");
            Console.WriteLine("  -help             Show this message and exit.");
            Console.WriteLine("  -inifile <FILE>   Specify the file to read configuration from.");
            Console.WriteLine("  -loginuri <URI>   Use URI as the login server address.");
            Console.WriteLine("  -version          Show version information and exit.");
        }

        [STAThread]
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            ArgvConfigSource configSource = new ArgvConfigSource(args);

            configSource.AddSwitch("Startup", "help", "h");
            configSource.AddSwitch("Startup", "inifile");
            configSource.AddSwitch("Startup", "loginuri");
            configSource.AddSwitch("Startup", "version", "v");

            if (configSource.Configs["Startup"].Contains("help"))
            {
                ShowHelp();
                return;
            }

            if (configSource.Configs["Startup"].Contains("version"))
            {
                ShowVersion();
                return;
            }

            LoadConfig(configSource);

            OpenViewer viewer = new OpenViewer(m_loginURI, m_username, m_password, m_renderingEngine, m_uiEngine, m_protocol);

            if (viewer.Start())
            {
                viewer.Run();
            }
        }
    }
}
