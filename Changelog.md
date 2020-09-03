# [0.0.3] - 2020-09-03

## New Features

* Added UniformPath: has equal intervals between points.
* Added BezierPathProvider, that segments path as uniform path.
* Added LockAxis option to path provider.

## Changes

* Uniform and non uniform path serialized in PathComponent.
* Removed IsCyclic variable from StaticPath (now this variable kept only in path providers).

# [0.0.2] - 2020-07-09

## Changes

* StaticPathWalker splited into StaticPathFollower and MovingAlongPath components. StaticPathFollower sets the position on the path by DistancePassed value. MovingAlongPath implements moving along path.
* Renamed StaticPath Cyclic value to IsCyclic.

# [0.0.1] - 2020-07-08

## New Features

* Added WaypointsPathCreator for create and edit static path. The editor tool allows you to add a new point (hold Shift), delete (hold Ctrl) and drag existing points. 
* Added GizmosPathDrawer for display path waypoints in editor.
* Added StaticPathWalker for moving along any static path.
* Added StaticPathWalking scene in Samples~ folder.