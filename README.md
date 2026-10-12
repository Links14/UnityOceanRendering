# Procedural Ocean Simulation

A real-time procedural ocean simulation built in Unity using handwritten HLSL.

## Demo

[video/player here]

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

Explain how the waves are represented and calculated...

## What I Learned

This project has been a great excercise to help push the limits of my understanding of shaders as well as introduce myself to many common real-time visual effects and how they are implemented. Many of these tactics are deceptively simple in description but can vary greatly in complexity depending on implementation.

Explain what you learned about vertex shaders, GPU processing,
normals, lighting, etc.

## References

- GPU Gems...
- Acerola...