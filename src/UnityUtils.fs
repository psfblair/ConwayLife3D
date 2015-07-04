namespace ConwayLife3D.Unity

open ConwayLife3D.Life.Core

module Utils = 
    open UnityEngine

    let unityCoordinatesFrom cell =
        match cell with
            | (x, y, z)     -> new Vector3(float32 x, float32 y, float32 z)

    let cellFromUnityCoordinates (coordinates: Vector3): Cell =
        (int(coordinates.x), int(coordinates.y), int(coordinates.z))