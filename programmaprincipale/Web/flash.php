<?php
	$l=$_GET["linea"];
	$tempo=$_GET["tempo"];
	$cmd=$_GET["stato"];
	if ($cmd=="0") exec("/usr/telecontrollo/cl offon $l $tempo",$op); else exec("/usr/telecontrollo/cl onoff $l $tempo",$op);
	echo "$l $cmd ret: $op[0]";
?>