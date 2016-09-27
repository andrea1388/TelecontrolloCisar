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
	  }
	  return $ret;
	 }
?>
  <body>
    <div class="container">

      <div class="page-header">
        <h1>Bootstrap grid examples</h1>
        <p class="lead">Basic grid layouts to get you familiar with building within the Bootstrap grid system.</p>
      </div>

      <h3>Three equal columns</h3>
      <p>Get three equal-width columns <strong>starting at desktops and scaling to large desktops</strong>. On mobile devices, tablets and below, the columns will automatically stack.</p>
      <div class="row">
	  <?php
	  for($i=0;$i<$numerolinee;$i++) {
	  								
	  ?>
    <div class="col-md-3">
		<div class="alert alert-success" role="alert">Linea <?php echo $i +1; ?></div>
		<button type="button" class="btn btn-primary">Accendi</button>
		<button type="button" class="btn btn-primary">Flash</button>
		</div>
	<?php
		 }
		?>
      </div>

     
    </div> <!-- /container -->
  </body>
</html>
