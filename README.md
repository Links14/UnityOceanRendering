# Procedural Ocean Simulation

A real-time procedural ocean simulation built in Unity using handwritten HLSL.

## Demo

<video src="Ocean-Ver-A.0.mp4" width="640" height="220" controls></video>

## Features

- Sum-of-sines procedural waves
    - Mimics the flowing nature of water waves
- GPU vertex displacement
    - Visualizes the mesh modifications in a high performance way
- Procedural normal calculation
    - Allows us to track face direction for shadow and reflective calculations
- Lambertian Diffuse surface lighting
    - Gives a believable shaded look to the waves
- Specular Highlighting with the use of Halfway vectors
    - Gives a believable look of the sunlight reflecting off of the water
- Fractional Brownian Motion used to step wave amplitude and frequency
    - This gives the waves a nice stepping effect where they do not appear to fight each other
- Domain Warping
    - This creates the effect of each wave's path having an effect on the subsequent waves
    - It gives believable intereactions between the waves and works well with fBm.
- Seeded random wave generation
    - Allows for random wave case creation with the ability to reproduce the results
- Configurable wave parameters
    - Allows for very tailored final results

## Technical Overview

In this project the waves are represented with a generated plane in Unity with a uniform spacing between all vertices. The wave offsets are created with a sum of sines algorithm, but instead of totally random sines, we use stepping amplitude and frequency values to create fractional brownian motion within the waves while also using a random direction. We also create shadows with a lambertian diffuse effect calculated with the normal of the vertex, and specular highlights with the camera direction and its bounce off the normal to the assumed light source position. The fragment shader result is multiplied by a static blue color to give it the recognizable ocean hue.

## What I Learned

This project has been a great excercise to help push the limits of my understanding of shaders as well as introduce myself to many common real-time visual effects and how they are implemented. Many of these tactics are deceptively simple in description but can vary greatly in complexity depending on implementation.
I also used this opportunity to play around with Unity's built in profiling tools to see where the compute was going during any given frame.

## References

- [GPU Gems - Chapter 1. Effective Water Simulation from Physical Models...](https://developer.nvidia.com/gpugems/gpugems/part-i-natural-effects/chapter-1-effective-water-simulation-physical-models)
- [Acerola - How Games Fake Water...](https://youtu.be/PH9q0HNBjT4)
