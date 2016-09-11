<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="telecontrollo.Default" %>
<!DOCTYPE html>
<html lang="it" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>

    <title>Telecontrollo Web App</title>
    <link href="css/bootstrap.min.css" rel="stylesheet"/>

</head>
<body>
    <form id="form1" runat="server">
    <div class="container">
    
        <div id="jj" runat="server">
        </div>
        <asp:button runat="server" text="Buttone" class="btn btn-info" ID="bt1" OnClick="btn1_Click"/>
    </div>
    </form>
    <!-- jQuery (necessary for Bootstrap's JavaScript plugins) -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <!-- Include all compiled plugins (below), or include individual files as needed -->
    <script src="js/bootstrap.min.js"></script>
</body>
</html>
