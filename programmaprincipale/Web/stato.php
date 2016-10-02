<?php
	 exec("/usr/telecontrollo/cl stato" ,$op, $ret);
	 if ($ret==0) echo $op[0];
?>