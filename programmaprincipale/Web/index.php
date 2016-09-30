<?php
  //if (!isset($_SESSION['user'])) {
  //  header('Location: login.php');
//}
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
    <title>Telecontrollo Cisar</title>
    <link href="css/bootstrap.min.css" rel="stylesheet">
    <link href="grid.css" rel="stylesheet">
	  <script>
function refresh() {
        var xmlhttp = new XMLHttpRequest();
        xmlhttp.onreadystatechange = function() {
            if (this.readyState == 4 && this.status == 200) {
		    var valore=this.responseText;
		    var stato=parseInt("0x"+ valore);
		    var numerolinee= 4*valore.length; 
		    for(i=0;i<numerolinee;i++)
		    {
		    }
                	$("#tablerow").append(html);
            }
        };
        xmlhttp.open("GET", "gethint.php?q=" + str, true);
        xmlhttp.send();
}
	</script>
  </head>
<?php
	 exec("/usr/telecontrollo/cl stato" ,$op, $ret);
	 if ($ret==0) {
	 	$numerolinee=4*strlen($op[0]);
		$Statolinea=decodifica(base_convert($op[0], 16, 10),$numerolinee);
	 }
	 function decodifica($stato,$bits)
	 {
	  for($i=0;$i<$bits;$i++)
	  {
	      $ret[$i]=0;
	      if($stato & pow(2,$i)) $ret[$i]=1;
        //echo $ret[$i]. "<br>";
	  }
	  return $ret;
	 }
?>
  <body>
    <div class="container">

      <div class="page-header">
        <h1>Telecontrollo Cisar</h1>
        <p class="lead">Controllo remoto delle uscite</p>
      </div>

      <h3>Stato linee</h3>
      <p>Premere il pulsante per esercitare la funzione corrispondente</p>
      <div class="row">
	  <?php
	  for($i=0;$i<$numerolinee;$i++) {
	  								
	  ?>
    <div class="col-md-3">
		<div class="alert alert-success" role="alert">Linea <?php echo ($i +1); ?></div>
    <?php if($Statolinea[$i]==0) : ?>
		<button type="button" class="btn btn-success">Accendi</button>
		<button type="button" class="btn btn-primary">Flash</button>
    <?php else : ?>
		<button type="button" class="btn btn-danger">Spegni</button>
		<button type="button" class="btn btn-primary">Flash</button>
    <?php endif; ?>
		</div>
	<?php
		 }
		?>
      </div>

     
    </div> <!-- /container -->
  </body>
</html>
