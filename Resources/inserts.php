<?php

	if (isset($_POST[email]))
	{
		$con = mysqli_connect ( "localhost", "user", "pass", "database" );
		
		// Check connection
		if (mysqli_connect_errno ()) {
			echo "Failed to connect to MySQL: " . mysqli_connect_error ();
		}
		
		
		 mysql_query("INSERT INTO Users(UserName, Email, PhoneNumber) VALUES 
		('$_POST[UserName]','$_POST[Email]','$_POST[PhoneNumber]')");
		$id1=mysql_insert_id();
		 mysql_query("INSERT INTO Device(DeviceName, ScreenSize, OS, OSVersion) VALUES 
		('$_POST[DeviceName]','$_POST[ScreenSize]','$_POST[OS]','$_POST[OSVersion]')");
		$id2=mysql_insert_id();
		 mysql_query("INSERT INTO UsersDevice(DeviceID, UsersID, UsersUserName,DeviceDeviceName, DeviceOS) VALUES 
		('$id2','$id1','$_POST[UserName]','$_POST[DeviceName]','$_POST[OS]')");
		if (! mysqli_query ( $con, $sql )) {
		die ( 'Error: ' . mysqli_error ( $con ) );
		}
		echo "<h2>Thank You For Registering, And Good Luck!</h2>";
		
		mysqli_close ( $con );
	}
	else echo "<h1>Under Construction, but coming soon...</h1><h2>Chess Games, SPSU Team Members Roster, and more...</h2>";
	
?>
