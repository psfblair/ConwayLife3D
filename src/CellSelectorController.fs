namespace ConwayLife3D.Controllers

open UnityEngine
open ConwayLife3D.Unity.Utils

type CellSelectorController() = 
    inherit MonoBehaviour()

    member this.Start() =
        this.gameObject.SetActive(false)
