/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Net;
using System.Windows;

namespace sdkMulticastCS
{
    /// <summary>
    /// This class holds all the commands a that can be passed between clients of the multicast. 
    /// The goal is to demonstrate the creation of a small protocol for the purpose of communicating ]
    /// and understanding each message. 
    /// </summary>
    public class GameCommands
    {
        public const string CommandDelimeter = "|";
        public const string Join = "J";
        public const string Leave = "L";
        public const string Challenge = "C";
        public const string AcceptChallenge = "AC";
        public const string RejectChallenge = "RC";
        public const string Play = "P";
        public const string Ready = "G";
        public const string NewGame = "N";
        public const string LeaveGame = "LG";

        public const string JoinFormat = Join + CommandDelimeter + "{0}";
        public const string LeaveFormat = Leave + CommandDelimeter + "{0}";
        public const string LeaveGameFormat = LeaveGame + CommandDelimeter + "{0}";
        public const string ChallengeFormat = Challenge + CommandDelimeter + "{0}";
        public const string AcceptChallengeFormat = AcceptChallenge + CommandDelimeter + "{0}";
        public const string NewGameFormat = NewGame + CommandDelimeter + "{0}";
        public const string RejectChallengeFormat = RejectChallenge + CommandDelimeter + "{0}";
        public const string PlayFormat = Play + CommandDelimeter + "{0}" + CommandDelimeter + "{1}";
        public const string ReadyFormat = Ready + CommandDelimeter + "{0}";

    }
}
