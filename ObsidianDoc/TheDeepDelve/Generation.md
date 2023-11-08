
GenerationType is a class that is a variable in [[World]]
the base type will error if it is used in world

GenerationTypes that actually work must extend GenerationType and override the getVoxel (or whatever the name ends up being) function.

GenerationTypes so far:
[[GenerationDeepDelve]] - main generation type for the game world
[[GenerationEmpty]] - is empty
[[GenerationSimple]] - is just stone or regolith idk

