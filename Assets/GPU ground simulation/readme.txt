This project is compute shader based GPU side computed particle simulator.
We here are not talkig about particles used for VFX.
Particles in this project are simulated objects, elements of the virtual matter, 2d physical points, models of atoms.
They are interacting points in 2d environment. THey have physical properties like radius, mass, temperature, position and momentum.
And there are forces the particle affect each other with.
The puropse of this project is to have a model of matter, a kind of 2d ground made of particles simulated in real time.
It can be used for any kind of simulations. For example I myself building a game, you can check its first prototype video here:

https://www.youtube.com/watch?v=YSQC5eABhOA

But you can use it for whatever idea you have in mind. You can change particles' properties and the way they interact with each other.
Particles are being visualized as pixels on a texture, and you can assign a color to each particle, and change it on the fly.

Why is this project good? It's extremely optimized, and uses GPU to compute simulation, so it's a really fast way to get yourself a physically competent matter.


There are two main purposes of this project:

1. You can use it as a base for your own project.
2. And you can learn compute shaders with it.

There are three files in the project:

	camMaster.cs
		- contains a MonoBehavior class, that initializes calcs class, runs Update() and handles controls
	calcs.cs
		- contains the main cpu side code, where we:
			- create and initialize the shader
			- declare and initialize all data and pass it to the shader
			- run Dispatch() for the shader's kernels to do the actual gpu side computing
	particleInteraction.compute
		- the shader, which contains multiple kernels, that:
			- perform particle simulation step
			- visualize particles
			- fill the auxiliary arrays with data, that assist main calculations