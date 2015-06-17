<?php
	// RECEIVE INPUT JSON
	$inputJSON = json_decode(file_get_contents('php://input'), true);
	
	// PROCESS INPUT JSON
	$originalMessage = $inputJSON["message"];
	$alteredMessage = strtoupper($originalMessage);
	$shuffledMessage = str_shuffle($originalMessage);
	
	// PREPARE OUTPUT JSON
	$outputJSON = array(
		'originalMessage' => $originalMessage,
		'shuffledMessage' => $shuffledMessage,
		'alteredMessage' => $alteredMessage
	);
	
	// SEND OUTPUT JSON
	header('Content-type: application/json');
	echo json_encode($outputJSON);
?>