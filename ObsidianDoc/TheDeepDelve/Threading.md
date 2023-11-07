---
tags:
  - improvement
  - optimization
---

on start, determine how many threads user can reasonably support. Can likely safely assume at least 4, since no dual-thread cpus will run the game well.

queue of things to start on thread
when thread is doing something, lock it.
when thread done doing something unlock it
check if any thread unlocked, if so 
start queued thing on thread.

change the way we change voxels?

things that **SHOULD** be done on a thread
- generating voxel shit with noise
- generating mesh stuff
- calculating machine efficency - test this code in a non thread
- not much else

things that **SHOULD NOT** be done on a thread:
- changing voxels
- machines 