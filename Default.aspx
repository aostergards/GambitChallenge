<%@ Page Title="TUF2000-M PARSER" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GambitChallenge._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        #ValuesPanel
        {
            margin: 20px 0;
        }
        #ValuesPanel>div:not(:first-child)
        {
            margin-top: 5px;
        }
    </style>
    <div class="content-container">
        <h1>TUF-2000M <span style="color: green">PARSER</span></h1>
        <div class="label-wrapper green">
            <div>Log date</div>
            <asp:TextBox ID="LogDateTimeTextBox" runat="server" Enabled="false"></asp:TextBox>
        </div>
        <asp:Panel ID="ValuesPanel" runat="server">

        </asp:Panel>
        <!--
        <div style="text-align: center">
            <asp:Button ID="RefreshButton" runat="server" Width="200px" Text="Refresh" />
        </div>
        -->
    </div>
    <footer>
        Andreas Östergårds 2017
    </footer>
</asp:Content>
