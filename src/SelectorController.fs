namespace ConwayLife3D.Controllers

open UnityEngine
open ConwayLife3D.Unity.Utils

type SelectorController() = 
    inherit MonoBehaviour()

    [<SerializeField>] 
    let mutable moveWait: float32 = 1.0f

    [<SerializeField>] [<DefaultValue>] val mutable timeOfLastToggle: float32

    member this.Start() = this.timeOfLastToggle <- Time.time

    member this.Update() =

        (******* LEFT - RIGHT ***************************************************************)
        if Input.GetKey(KeyCode.F) && this.CanMove()
        then this.MoveTo(new Vector3(-1.0f, 0.0f, 0.0f))

        if Input.GetKey(KeyCode.H) && this.CanMove()
        then this.MoveTo(new Vector3(1.0f, 0.0f, 0.0f))
     
        (******* FORWARD - BACK *************************************************************)
        if Input.GetKey(KeyCode.T) && this.CanMove()
        then this.MoveTo(new Vector3(0.0f, 0.0f, 1.0f))
     
        if Input.GetKey(KeyCode.G) && this.CanMove()
        then this.MoveTo(new Vector3(0.0f, 0.0f, -1.0f))
     
        (******* UP - DOWN ******************************************************************)
        if Input.GetKey(KeyCode.R) && this.CanMove()
        then this.MoveTo(new Vector3(0.0f, -1.0f, 0.0f))
     
        if Input.GetKey(KeyCode.Y) && this.CanMove()
        then this.MoveTo(new Vector3(0.0f, 1.0f, 0.0f))

    member private this.CanMove() = pauseWaitElapsed moveWait this.timeOfLastToggle Time.time

    member private this.MoveTo(vector: Vector3) =
        this.timeOfLastToggle <- Time.time
        this.transform.Translate(vector, Space.World)