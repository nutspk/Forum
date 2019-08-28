<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestWS.aspx.cs" Inherits="Selectcon.TestWS" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Load
            <asp:TextBox ID="txtQ" runat="server" TextMode="MultiLine" Columns="40" Rows="10">

            </asp:TextBox>
            
            <asp:Button ID="btnSend" runat="server" OnClick="btnSend_Click" Text="bound" />


        </div>

        <div>   
            Cmd
            <asp:TextBox ID="txtExecu" runat="server" TextMode="MultiLine" Columns="40" Rows="10">

            </asp:TextBox>

            <asp:Button ID="btnExecu" runat="server" OnClick="btnExecu_Click" Text="bound" />
        </div>

        <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="true">
              
        </asp:GridView>

    </form>
</body>
</html>
