﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;

/*
 * The official license for this file is shown next.
 * Unofficially, consider this e-postcardware as well:
 * if you find this module useful, let us know via e-mail, along with
 * where in the world you are and (if applicable) your website address.
 */

/* ***** BEGIN LICENSE BLOCK *****
 * Version: MIT License
 *
 * Copyright (c) 2010 Michael Sorens http://www.simple-talk.com/author/michael-sorens/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 * ***** END LICENSE BLOCK *****
 */

namespace HostSwitcher
{
    public class HostFileManager
    {
        //private static readonly string HostsFile = @"\usr\tmp\Hosts";
        private static readonly string HostsFile = @"\Windows\System32\Drivers\Etc\Hosts";
        private static readonly string HostsFileTemp = HostsFile + "-tmp";
        private static readonly string HostsBackup = HostsFile + ".BAK";

        public string HostsFileName { get { return HostsFile; } }

        private List<string> lines;
        
        public List<string> ReadFile()
        {
            lines = new List<string>(File.ReadAllLines(HostsFile));
            return lines;
        }

        public void ReplaceFile(List<string> newLines)
        {
            File.WriteAllLines(HostsFileTemp, newLines);
            CopySecurityInformation(HostsFile, HostsFileTemp);
            File.Replace(HostsFileTemp, HostsFile, HostsBackup);
        }

        public void SecureFile()
        {
            CopySecurityInformation(HostsFile, HostsFileTemp);
        }

        // From http://stackoverflow.com/questions/3118439/how-to-copy-ntfs-permissions
        //private static void CopySecurityInformation(String source, String dest)
        //{
        //    FileSecurity fileSecurity = File.GetAccessControl(source, AccessControlSections.All);
        //    fileSecurity.SetAccessRuleProtection(true, true);  // from http://www.codekeep.net/snippets/1dc00f8c-b338-4760-aecb-024fe5009ed6.aspx
        //    File.SetAccessControl(dest, fileSecurity);
        //    FileAttributes fileAttributes = File.GetAttributes(source);
        //    File.SetAttributes(dest, fileAttributes);
        //}

        // From http://msdn.microsoft.com/en-us/library/system.io.file.setaccesscontrol.aspx
        private static void CopySecurityInformation(String source, String dest)
        {
            FileSecurity sourceFileSecurity = File.GetAccessControl(source, AccessControlSections.All);
            FileSecurity destFileSecurity = new FileSecurity();
            string sourceDescriptor = sourceFileSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.All);
            destFileSecurity.SetSecurityDescriptorSddlForm(sourceDescriptor);
            File.SetAccessControl(dest, sourceFileSecurity);

            FileAttributes fileAttributes = File.GetAttributes(source);
            File.SetAttributes(dest, fileAttributes);
        }

    }
}
