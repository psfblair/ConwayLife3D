namespace ConwayLife3D.Unity

module Utils = 
    open UnityEngine

    let unityCoordinatesFrom cell: Vector3 =
        match cell with
            | (x, y, z)     -> new Vector3(float32 x, float32 y, float32 z)

