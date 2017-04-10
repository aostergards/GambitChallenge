<%@ Page Title="TUF2000-M PARSER" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GambitChallenge._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        #ValuesPanel>div:not(:first-child)
        {
            margin-top: 5px;
        }
    </style>
    <div class="content-container">
        <h1>TUF-2000M <span style="color: green">PARSER</span></h1>
        <div class="label-wrapper green">
            <div>Reading date</div>
            <asp:TextBox ID="LogDateTimeTextBox" runat="server" Enabled="false"></asp:TextBox>
        </div>
        <asp:Panel ID="ValuesPanel" runat="server" Style="margin: 20px 0">

        </asp:Panel>
        <div style="text-align: center">
            <input type="submit" style="width: 200px; margin-bottom: 10px" value="Refresh" />
        </div>
    </div>
    <footer>
        Andreas Östergårds 2017
    </footer>
</asp:Content>
