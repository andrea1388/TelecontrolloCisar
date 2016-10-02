<?php
	$l=$_GET["linea"];
	$cmd=$_GET["stato"];
	exec("/usr/telecontrollo/cl $cmd $l",$op);
	echo "$l $cmd ret: $op[0]";
?>