<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendRaw.aspx.cs" Inherits="SendRaw.SendRaw" %>
<!-- 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
-->

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <br />
        Enter URI:</div>
    <asp:TextBox ID="TextBoxUri" runat="server" Width="666px"></asp:TextBox>
    <br />
    <br />
    Enter Value 1:<br />
    <asp:TextBox ID="TextBoxValue1" runat="server"></asp:TextBox>
    <br />
    <br />
    Enter Value 2:<br />
    <asp:TextBox ID="TextBoxValue2" runat="server"></asp:TextBox>
    <br />
    <br />
    <br />
    <asp:Button ID="ButtonSendRaw" runat="server" onclick="ButtonSendRaw_Click" 
        Text="Send Raw Notification" />
    <br />
    <br />
    Response:<br />
    <asp:TextBox ID="TextBoxResponse" runat="server" Height="78px" Width="199px"></asp:TextBox>
    </form>
</body>
</html>