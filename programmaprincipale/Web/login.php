<?php
if (isset($_POST["Invio"])) {
    $_SESSION['user']="andrea";
    header('Location: index.php');
}
?>
<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <!-- The above 3 meta tags *must* come first in the head; any other head content must come *after* these tags -->
    <meta name="description" content="">
    <meta name="author" content="">
    <link rel="icon" href="favicon.ico">
    <title>Login</title>
    <link href="css/bootstrap.min.css" rel="stylesheet">
    <link href="grid.css" rel="stylesheet">
  </head>

  <body>
    <div class="container">

      <div class="page-header">
        <h1>Telecontrollo Cisar - Prego autenticarsi</h1>
        <p class="lead">Inserire nome utente e password</p>
      </div>
      <form action="login.php" method="POST">
        First name: <input type="text" name="login"><br>
        Last name: <input type="text" name="password"><br>
        <input type="submit" value="Invio">
      </form>   

     
    </div> <!-- /container -->
  </body>
</html>
