# ecsExample
EntityGraphicsSystem is responsible for updating data and drawing textures.

SpriteAnimationSystem updates the animation index.

SpriteColorSystem, SpriteIndexSystem, and SpriteMatrixSystem refresh the rendering data for graphics.

Use the following interface to create a frame animation object:

```C#
	FrameAnimationHelper.CreateEntity(atlas, frameRate, scale, row, col);
```

Use the following interface to add frame animations to an object:

```C#
	FrameAnimationHelper.AddAnimation(entity, frameRate, scale, row, col);
```
