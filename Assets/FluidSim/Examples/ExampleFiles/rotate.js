#pragma strict

		var rotateSpeed = 1.0;
		
private var rotateVector = Vector3.zero;

private var myTransform : Transform;

function Start()
{
	myTransform = transform;
}

//======

function Update()
{
	rotateVector.z = rotateSpeed * Time.deltaTime;
	
	myTransform.Rotate(rotateVector);
}